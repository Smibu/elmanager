using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.E2ETests;

[TestClass]
public static class BrowserAppFixture
{
    private static readonly HttpClient HttpClient = new() { Timeout = TimeSpan.FromSeconds(10) };
    private static Process? _serverProcess;
    private static volatile string? _detectedUrl;

    public static string BaseUrl { get; private set; } = "";

    [AssemblyInitialize]
    public static async Task Setup(TestContext context)
    {
        var external = Environment.GetEnvironmentVariable("SLE_E2E_BASEURL");
        if (!string.IsNullOrWhiteSpace(external))
        {
            BaseUrl = external.TrimEnd('/');
            await WaitUntilReachableAsync(BaseUrl, () => null, TimeSpan.FromMinutes(2));
            return;
        }

        var browserProject = LocateBrowserProject();
        var expectedUrl = ReadApplicationUrl(browserProject);
        StartServer(browserProject);

        BaseUrl = await WaitUntilReachableAsync(expectedUrl, () => _detectedUrl, TimeSpan.FromMinutes(5));
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        if (_serverProcess is { HasExited: false })
        {
            try
            {
                _serverProcess.Kill(entireProcessTree: true);
                _serverProcess.WaitForExit(10_000);
            }
            catch
            {
            }
        }

        _serverProcess?.Dispose();
        _serverProcess = null;
    }

    private static void StartServer(string browserProjectPath)
    {
        var dotnetExe = ResolveDotnetExecutable();
        var startInfo = new ProcessStartInfo
        {
            FileName = dotnetExe,
            Arguments = $"""run --project "{browserProjectPath}" -c Debug""",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Path.GetDirectoryName(browserProjectPath)!,
        };

        foreach (var key in new[]
                 {
                     "DOTNET_ROOT", "DOTNET_ROOT(x86)", "DOTNET_ROOT_X64", "DOTNET_ROOTX64",
                     "DOTNET_MULTILEVEL_LOOKUP", "DOTNET_STARTUP_HOOKS",
                     "MSBuildSDKsPath", "MSBuildExtensionsPath", "MSBuildExtensionsPath32",
                     "MSBuildExtensionsPath64", "MSBUILD_EXE_PATH", "MSBuildLoadMicrosoftTargetsReadOnly",
                 })
        {
            startInfo.Environment.Remove(key);
        }

        var dotnetDir = Path.GetDirectoryName(dotnetExe);
        if (!string.IsNullOrEmpty(dotnetDir))
        {
            startInfo.Environment["DOTNET_ROOT"] = dotnetDir;
        }

        var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        var listeningRegex = new Regex(@"listening on:\s*(http://\S+)", RegexOptions.IgnoreCase);

        void OnOutput(object? _, DataReceivedEventArgs e)
        {
            if (e.Data is null)
            {
                return;
            }

            Console.WriteLine($"[sle-browser] {e.Data}");
            var match = listeningRegex.Match(e.Data);
            if (match.Success)
            {
                _detectedUrl = match.Groups[1].Value.TrimEnd('/');
            }
        }

        process.OutputDataReceived += OnOutput;
        process.ErrorDataReceived += OnOutput;

        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start the SLE browser app dev server.");
        }

        _serverProcess = process;
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    private static async Task<string> WaitUntilReachableAsync(string expectedUrl, Func<string?> detectedUrl, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        Exception? last = null;
        while (DateTime.UtcNow < deadline)
        {
            if (_serverProcess is { HasExited: true } exited)
            {
                throw new InvalidOperationException(
                    $"The SLE browser app process exited with code {exited.ExitCode} before it became reachable.");
            }

            var url = detectedUrl() ?? expectedUrl;
            try
            {
                using var response = await HttpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return url;
                }
            }
            catch (Exception ex)
            {
                last = ex;
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"The SLE browser app at {expectedUrl} did not become reachable in time.", last);
    }

    private static string ReadApplicationUrl(string browserProjectPath)
    {
        var launchSettings = Path.Combine(
            Path.GetDirectoryName(browserProjectPath)!, "Properties", "launchSettings.json");

        using var doc = JsonDocument.Parse(File.ReadAllText(launchSettings));
        var profiles = doc.RootElement.GetProperty("profiles");

        foreach (var profile in profiles.EnumerateObject())
        {
            if (profile.Value.TryGetProperty("applicationUrl", out var url) &&
                url.GetString() is { Length: > 0 } value)
            {
                foreach (var candidate in value.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (candidate.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    {
                        return candidate.TrimEnd('/');
                    }
                }

                return value.Split(';', StringSplitOptions.RemoveEmptyEntries)[0].TrimEnd('/');
            }
        }

        throw new InvalidOperationException("Could not find an application URL in launchSettings.json.");
    }

    private static string LocateBrowserProject()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Elmanager.sln")))
        {
            dir = dir.Parent;
        }

        if (dir is null)
        {
            throw new InvalidOperationException(
                "Could not locate the repository root (Elmanager.sln) from the test output directory.");
        }

        var project = Path.Combine(dir.FullName, "Elmanager.SLE.Browser", "Elmanager.SLE.Browser.csproj");
        if (!File.Exists(project))
        {
            throw new FileNotFoundException("Could not find the SLE browser project.", project);
        }

        return project;
    }

    private static string ResolveDotnetExecutable()
    {
        static bool HasSdk(string dotnetExe)
        {
            try
            {
                var dir = Path.GetDirectoryName(dotnetExe);
                if (string.IsNullOrEmpty(dir))
                {
                    return false;
                }

                var sdkDir = Path.Combine(dir, "sdk");
                return Directory.Exists(sdkDir) && Directory.EnumerateDirectories(sdkDir).Any();
            }
            catch
            {
                return false;
            }
        }

        var exeName = OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet";
        var candidates = new List<string?>
        {
            Environment.GetEnvironmentVariable("DOTNET_HOST_PATH"),
        };

        var pathVar = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dir in pathVar.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            candidates.Add(Path.Combine(dir.Trim(), exeName));
        }

        foreach (var root in new[]
                 {
                     Environment.GetEnvironmentVariable("ProgramW6432"),
                     Environment.GetEnvironmentVariable("ProgramFiles"),
                     Environment.GetEnvironmentVariable("ProgramFiles(x86)"),
                 })
        {
            if (!string.IsNullOrEmpty(root))
            {
                candidates.Add(Path.Combine(root, "dotnet", exeName));
            }
        }

        var localAppData = Environment.GetEnvironmentVariable("LOCALAPPDATA");
        if (!string.IsNullOrEmpty(localAppData))
        {
            candidates.Add(Path.Combine(localAppData, "Microsoft", "dotnet", exeName));
        }

        foreach (var candidate in candidates)
        {
            if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate) && HasSdk(candidate))
            {
                return candidate;
            }
        }

        foreach (var candidate in candidates)
        {
            if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
            {
                return candidate;
            }
        }

        return exeName;
    }
}
