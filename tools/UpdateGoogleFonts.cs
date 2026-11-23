using System.Net;
using System.Text;
using System.Text.RegularExpressions;

const string DataFile = "Elmanager.SLE/Resources/googlefonts.tsv";
const string UrlPrefix = "https://fonts.gstatic.com/s/";

var path = Path.Combine(FindRepoRoot(), DataFile);
var families = File.ReadLines(path)
    .Where(line => line.Length > 0)
    .Select(line => line.Split('\t')[0].Trim())
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .Order(StringComparer.Ordinal)
    .ToList();

using var http = new HttpClient();
http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Elmanager)");

var rows = new string[families.Count][];
var done = 0;
await Parallel.ForEachAsync(Enumerable.Range(0, families.Count), new ParallelOptions { MaxDegreeOfParallelism = 8 },
    async (i, ct) =>
    {
        rows[i] = await ResolveAsync(families[i], ct);
        var count = Interlocked.Increment(ref done);
        if (count % 200 == 0 || count == families.Count)
        {
            Console.WriteLine($"{count}/{families.Count}");
        }
    });

var output = new StringBuilder();
foreach (var (family, styles) in families.Zip(rows))
{
    output.Append(family);
    foreach (var style in styles)
    {
        output.Append('\t').Append(style);
    }

    output.Append('\n');
}

File.WriteAllText(path, output.ToString());
var missing = rows.Count(r => r.All(s => s.Length == 0));
Console.WriteLine($"Wrote {families.Count} families to {DataFile} ({missing} without any link).");

async Task<string[]> ResolveAsync(string family, CancellationToken ct)
{
    var url = "https://fonts.googleapis.com/css?family=" + family.Replace(' ', '+') + ":400,700,400italic,700italic";
    for (var attempt = 1; ; attempt++)
    {
        using var response = await http.GetAsync(url, ct);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            return ["", "", "", ""];
        }

        if (response.IsSuccessStatusCode)
        {
            return ParseCss(family, await response.Content.ReadAsStringAsync(ct));
        }

        if (attempt == 3)
        {
            throw new HttpRequestException($"'{family}': HTTP {(int)response.StatusCode} from {url}");
        }

        await Task.Delay(TimeSpan.FromSeconds(attempt * 2), ct);
    }
}

static string[] ParseCss(string family, string css)
{
    var styles = new[] { "", "", "", "" };
    foreach (Match block in Regex.Matches(css, @"@font-face\s*\{(.*?)\}", RegexOptions.Singleline))
    {
        var body = block.Groups[1].Value;
        var italic = Regex.Match(body, @"font-style:\s*(\w+)").Groups[1].Value == "italic";
        var weight = Regex.Match(body, @"font-weight:\s*(\d+)").Groups[1].Value;
        var src = Regex.Match(body, @"src:\s*url\(([^)]+)\)").Groups[1].Value;
        if (!src.StartsWith(UrlPrefix) || !(src.EndsWith(".ttf") || src.EndsWith(".otf")))
        {
            throw new InvalidDataException($"'{family}': unexpected font URL '{src}'");
        }

        var index = (weight, italic) switch
        {
            ("400", false) => 0,
            ("700", false) => 1,
            ("400", true) => 2,
            ("700", true) => 3,
            _ => -1,
        };
        if (index >= 0)
        {
            styles[index] = src[UrlPrefix.Length..];
        }
    }

    return styles;
}

static string FindRepoRoot()
{
    for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Elmanager.sln")))
        {
            return dir.FullName;
        }
    }

    throw new InvalidOperationException("Run this from inside the Elmanager repository.");
}
