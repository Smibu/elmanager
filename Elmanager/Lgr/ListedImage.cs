using System.Collections.Generic;
using System.Linq;

namespace Elmanager.Lgr;

internal record ListedImage(
    ImageMeta Data,
    Transparency Transparency
)
{
    private static readonly string[] BodyPartNames =
        {"body", "thigh", "leg", "bike", "wheel", "susp1", "susp2", "forarm", "up_arm", "head"};

    private static IEnumerable<string> EnumSpecialNames()
    {
        for (int i = 1; i <= 2; i++)
        {
            foreach (var bodyPartName in BodyPartNames)
            {
                yield return $"q{i}{bodyPartName}";
            }
        }

        yield return "qflag";
        yield return "qkiller";
        yield return "qexit";
        yield return "qframe";
        yield return "qcolors";
        yield return "qgrass";
    }

    private static readonly HashSet<string> SpecialNames =
        new(EnumSpecialNames().Union(Lgr.TransparencyIgnoreSet));

    public bool IsSpecial => SpecialNames.Contains(Name);

    public string Name => Data.Name;

    public ImageType Type => Data.Type;

    public ClippingType ClippingType => Data.ClippingType;

    public int Distance => Data.Distance;
}