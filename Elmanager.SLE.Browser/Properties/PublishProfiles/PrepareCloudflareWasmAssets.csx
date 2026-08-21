using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public sealed class PrepareCloudflareWasmAssets : Task
{
    [Required]
    public string PublishDirectory { get; set; } = string.Empty;

    [Required]
    public string HeadersPath { get; set; } = string.Empty;

    public override bool Execute()
    {
        if (!Directory.Exists(PublishDirectory))
        {
            Log.LogError($"Publish directory does not exist: {PublishDirectory}");
            return false;
        }

        if (!File.Exists(HeadersPath))
        {
            Log.LogError($"Cloudflare headers file does not exist: {HeadersPath}");
            return false;
        }

        var gzipFiles = Directory.GetFiles(PublishDirectory, "*.wasm.gz", SearchOption.AllDirectories);
        if (gzipFiles.Length > 0)
        {
            Log.LogError($"Cloudflare publish contains {gzipFiles.Length} gzip-compressed WebAssembly files.");
            return false;
        }

        var wasmFiles = Directory.GetFiles(PublishDirectory, "*.wasm", SearchOption.AllDirectories);
        if (wasmFiles.Length == 0)
        {
            Log.LogError($"Cloudflare publish contains no WebAssembly files: {PublishDirectory}");
            return false;
        }

        foreach (var wasmPath in wasmFiles)
        {
            var brotliPath = wasmPath + ".br";
            if (!File.Exists(brotliPath))
            {
                Log.LogError($"Missing Brotli file for {wasmPath}");
                continue;
            }

            File.Copy(brotliPath, wasmPath, true);
        }

        if (Log.HasLoggedErrors)
        {
            return false;
        }

        var headersPath = Path.Combine(PublishDirectory, "wwwroot", "_headers");
        File.Copy(HeadersPath, headersPath, true);

        foreach (var brotliPath in Directory.GetFiles(PublishDirectory, "*.br", SearchOption.AllDirectories))
        {
            File.Delete(brotliPath);
        }

        Log.LogMessage(
            MessageImportance.High,
            $"Prepared {wasmFiles.Length} Brotli-compressed WebAssembly files and Cloudflare Pages headers.");
        return true;
    }
}
