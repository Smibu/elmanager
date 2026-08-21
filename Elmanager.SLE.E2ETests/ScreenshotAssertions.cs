using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;

namespace Elmanager.SLE.E2ETests;

public sealed class ToMatchScreenshotOptions
{
    public double Threshold { get; set; } = 0.0;

    public int MaxDiffPixels { get; set; }

    public double MaxDiffPixelRatio { get; set; }

    public bool FullPage { get; set; }

    public Clip? Clip { get; set; }

    public ScreenshotAnimations Animations { get; set; } = ScreenshotAnimations.Disabled;

    public string? SnapshotDirectory { get; set; }

    public bool? FailOnMissingBaseline { get; set; }
}

public static class ScreenshotAssertions
{
    public static async Task ExpectScreenshotAsync(
        this IPage page,
        string name,
        ToMatchScreenshotOptions? options = null,
        [CallerFilePath] string? sourceFile = null)
    {
        options ??= new ToMatchScreenshotOptions();
        var actual = await page.ScreenshotAsync(new PageScreenshotOptions
        {
            FullPage = options.FullPage,
            Clip = options.Clip,
            Animations = options.Animations,
        });
        CompareOrCreate(name, actual, options, sourceFile);
    }

    public static async Task ExpectScreenshotAsync(
        this ILocator locator,
        string name,
        ToMatchScreenshotOptions? options = null,
        [CallerFilePath] string? sourceFile = null)
    {
        options ??= new ToMatchScreenshotOptions();
        var actual = await locator.ScreenshotAsync(new LocatorScreenshotOptions
        {
            Animations = options.Animations,
        });
        CompareOrCreate(name, actual, options, sourceFile);
    }

    private static void CompareOrCreate(string name, byte[] actualBytes, ToMatchScreenshotOptions options, string? sourceFile)
    {
        var baselinePath = ResolveBaselinePath(name, options, sourceFile);
        Directory.CreateDirectory(Path.GetDirectoryName(baselinePath)!);

        if (!File.Exists(baselinePath))
        {
            if (ShouldFailOnMissingBaseline(options))
            {
                var written = WriteFailureArtifacts(baselinePath, actualBytes, diff: null);
                Assert.Fail(
                    $"Snapshot reference '{baselinePath}' does not exist and baseline creation is disabled " +
                    $"(running on CI). The captured image was written to:{Environment.NewLine}{written}");
                return;
            }

            File.WriteAllBytes(baselinePath, actualBytes);
            Console.WriteLine($"[snapshot] Created new reference image: {baselinePath}");
            return;
        }

        var expectedBytes = File.ReadAllBytes(baselinePath);

        using var expected = DecodeRgba(expectedBytes);
        using var actual = DecodeRgba(actualBytes);

        if (expected.Width != actual.Width || expected.Height != actual.Height)
        {
            var written = WriteFailureArtifacts(baselinePath, actualBytes, diff: null);
            Assert.Fail(
                $"Snapshot '{name}' size mismatch: reference is {expected.Width}x{expected.Height}, " +
                $"got {actual.Width}x{actual.Height}.{Environment.NewLine}{written}");
            return;
        }

        var expectedPixels = expected.GetPixelSpan().ToArray();
        var actualPixels = actual.GetPixelSpan().ToArray();

        var threshold = (int)Math.Round(Math.Clamp(options.Threshold, 0, 1) * 255);
        var (diffCount, diffPixels) = ComparePixels(expectedPixels, actualPixels, threshold);

        var pixelCount = (long)expected.Width * expected.Height;
        var allowed = Math.Max(options.MaxDiffPixels, (long)(Math.Clamp(options.MaxDiffPixelRatio, 0, 1) * pixelCount));
        if (diffCount > allowed)
        {
            var diffPng = EncodeRgbaToPng(diffPixels, expected.Width, expected.Height);
            var written = WriteFailureArtifacts(baselinePath, actualBytes, diffPng);
            Assert.Fail(
                $"Snapshot '{name}' differs from its reference image: {diffCount} pixel(s) changed " +
                $"(allowed {allowed}).{Environment.NewLine}{written}");
        }
    }

    private static SKBitmap DecodeRgba(byte[] pngBytes)
    {
        using var data = SKData.CreateCopy(pngBytes);
        using var codec = SKCodec.Create(data);
        if (codec is null)
        {
            throw new InvalidOperationException("Failed to decode snapshot PNG: not a valid image.");
        }

        var info = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        var bitmap = new SKBitmap(info);
        var result = codec.GetPixels(info, bitmap.GetPixels());
        if (result is not SKCodecResult.Success and not SKCodecResult.IncompleteInput)
        {
            bitmap.Dispose();
            throw new InvalidOperationException($"Failed to decode snapshot PNG: {result}.");
        }

        return bitmap;
    }

    private static byte[] EncodeRgbaToPng(byte[] rgbaPixels, int width, int height)
    {
        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul);
        using var bitmap = new SKBitmap(info);
        System.Runtime.InteropServices.Marshal.Copy(rgbaPixels, 0, bitmap.GetPixels(), rgbaPixels.Length);
        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }

    private static (long DiffCount, byte[] DiffPixels) ComparePixels(byte[] expected, byte[] actual, int threshold)
    {
        var diff = new byte[expected.Length];
        long count = 0;
        for (var i = 0; i < expected.Length; i += 4)
        {
            var dr = Math.Abs(expected[i] - actual[i]);
            var dg = Math.Abs(expected[i + 1] - actual[i + 1]);
            var db = Math.Abs(expected[i + 2] - actual[i + 2]);
            var da = Math.Abs(expected[i + 3] - actual[i + 3]);
            var maxDelta = Math.Max(Math.Max(dr, dg), Math.Max(db, da));

            if (maxDelta > threshold)
            {
                count++;
                diff[i] = 255;
                diff[i + 1] = 0;
                diff[i + 2] = 0;
                diff[i + 3] = 255;
            }
            else
            {
                diff[i] = (byte)(expected[i] / 4 + 191);
                diff[i + 1] = (byte)(expected[i + 1] / 4 + 191);
                diff[i + 2] = (byte)(expected[i + 2] / 4 + 191);
                diff[i + 3] = 255;
            }
        }

        return (count, diff);
    }

    private static string ResolveBaselinePath(string name, ToMatchScreenshotOptions options, string? sourceFile)
    {
        var directory = options.SnapshotDirectory;
        if (string.IsNullOrEmpty(directory))
        {
            var sourceDir = sourceFile is not null ? Path.GetDirectoryName(sourceFile) : null;
            directory = sourceDir is not null && Directory.Exists(sourceDir)
                ? Path.Combine(sourceDir, "Snapshots")
                : Path.Combine(AppContext.BaseDirectory, "Snapshots");
        }

        var fileName = Path.HasExtension(name) ? name : name + ".png";
        return Path.GetFullPath(Path.Combine(directory, fileName));
    }

    private static string WriteFailureArtifacts(string baselinePath, byte[] actual, byte[]? diff)
    {
        var dir = Path.GetDirectoryName(baselinePath)!;
        var name = Path.GetFileNameWithoutExtension(baselinePath);
        Directory.CreateDirectory(dir);

        var lines = new List<string>();
        if (File.Exists(baselinePath))
        {
            lines.Add($"  expected: {baselinePath}");
        }

        var actualPath = Path.Combine(dir, $"{name}.actual.png");
        File.WriteAllBytes(actualPath, actual);
        lines.Add($"  actual:   {actualPath}");

        if (diff is not null)
        {
            var diffPath = Path.Combine(dir, $"{name}.diff.png");
            File.WriteAllBytes(diffPath, diff);
            lines.Add($"  diff:     {diffPath}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static bool ShouldFailOnMissingBaseline(ToMatchScreenshotOptions options)
    {
        if (options.FailOnMissingBaseline.HasValue)
        {
            return options.FailOnMissingBaseline.Value;
        }

        return IsTruthy(Environment.GetEnvironmentVariable("CI"))
            || IsTruthy(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"))
            || IsTruthy(Environment.GetEnvironmentVariable("TF_BUILD"));

        static bool IsTruthy(string? value) =>
            !string.IsNullOrEmpty(value) &&
            !value.Equals("0", StringComparison.OrdinalIgnoreCase) &&
            !value.Equals("false", StringComparison.OrdinalIgnoreCase);
    }
}
