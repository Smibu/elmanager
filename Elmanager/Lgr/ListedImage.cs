using System.Collections.Generic;
using System.Linq;

namespace Elmanager.Lgr
{
    internal struct ListedImage
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

        internal ClippingType ClippingType;
        internal int Distance;
        internal string Name;
        internal ImageType Type;
        internal Transparency Transparency;

        public bool IsSpecial => SpecialNames.Contains(Name);
    }
}