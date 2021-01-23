using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Elmanager
{
    internal class Lgr : IDisposable
    {
        internal readonly List<LgrImage> LgrImages = new();
        internal readonly List<ListedImage> ListedImages = new();
        internal readonly string Path;

        private static readonly HashSet<string> TransparencyIgnoreSet =
            new(Enumerable.Range(0, 18).SelectMany(TransparencyIgnoreHelper));

        internal enum ImageType
        {
            Picture = 100,
            Texture = 101,
            Mask = 102
        }

        internal enum Transparency
        {
            NotTransparent = 10,
            Palette0 = 11,
            TopLeft = 12,
            TopRight = 13,
            BottomLeft = 14,
            BottomRight = 15
        }

        private static IEnumerable<string> TransparencyIgnoreHelper(int i)
        {
            yield return $"qfood{i}";
            yield return $"qup_{i}";
            yield return $"qdown_{i}";
        }

        internal IEnumerable<ListedImage> ListedImagesExcludingSpecial
        {
            get
            {
                foreach (var listedImage in ListedImages)
                {
                    if (!listedImage.IsSpecial)
                    {
                        yield return listedImage;
                    }
                }

                var grassImg = GetImage("QGRASS");
                if (grassImg != null)
                {
                    yield return new ListedImage
                    {
                        ClippingType = ClippingType.Ground,
                        Distance = 500,
                        Name = grassImg.Name,
                        Transparency = Transparency.Palette0,
                        Type = ImageType.Texture
                    };
                }
            }
        }

        private LgrImage GetImage(string name)
        {
            return LgrImages.FirstOrDefault(img => img.Name == name.ToLower());
        }

        internal Lgr(string lgrFile)
        {
            using var stream = File.OpenRead(lgrFile);
            Path = lgrFile;
            var lgr = new BinaryReader(stream, Encoding.ASCII);
            if (lgr.ReadString(5) != "LGR12")
                throw new Exception($"The LGR file {lgrFile} is not valid (magic start string not found)");
            int numberOfPcXs = lgr.ReadInt32();
            int picturesLstVersion = lgr.ReadInt32(); // unused
            int numberOfOptPcXs = lgr.ReadInt32();
            var pcxNames = new List<string>();
            var pcxTypes = new List<ImageType>();
            var distances = new List<int>();
            var clippingTypes = new List<ClippingType>();
            var transparencies = new List<Transparency>();
            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                pcxNames.Add(lgr.ReadNullTerminatedString(10));
            }

            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                pcxTypes.Add((ImageType) lgr.ReadInt32());
            }

            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                distances.Add(lgr.ReadInt32());
            }

            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                clippingTypes.Add((ClippingType) lgr.ReadInt32());
            }

            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                transparencies.Add((Transparency) lgr.ReadInt32());
            }

            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                ListedImages.Add(new ListedImage
                {
                    Name = pcxNames[i].ToLower(),
                    Type = pcxTypes[i],
                    Distance = distances[i],
                    ClippingType = clippingTypes[i],
                    Transparency = transparencies[i]
                });
            }

            for (int i = 0; i < numberOfPcXs; i++)
            {
                string lgrImageName = System.IO.Path.GetFileNameWithoutExtension(lgr.ReadNullTerminatedString(12)).ToLower();
                var isGrass = lgrImageName == "qgrass";
                ImageType imgType = isGrass ? ImageType.Texture : ImageType.Picture;
                int imgDistance = 500;
                ClippingType imgClippingType = isGrass ? ClippingType.Ground : ClippingType.Unclipped;
                var transparency = Transparency.TopLeft;
                foreach (ListedImage x in ListedImages)
                {
                    if (x.Name == lgrImageName)
                    {
                        imgType = x.Type;
                        imgClippingType = x.ClippingType;
                        imgDistance = x.Distance;
                        if (!TransparencyIgnoreSet.Contains(x.Name))
                        {
                            transparency = x.Transparency;
                        }
                    }
                }

                lgr.ReadInt32(); // unknown, not used
                lgr.ReadInt32(); // unknown, not used
                int sizeOfPcx = lgr.ReadInt32();
                Bitmap bmp = new Pcx(stream).ToBitmap();
                if (imgType != ImageType.Texture)
                {
                    switch (transparency)
                    {
                        case Transparency.NotTransparent:
                            break;
                        case Transparency.Palette0:
                            bmp.MakeTransparent(Color.Black); // TODO get from palette index 0
                            break;
                        // If the transparency field value is invalid, we'll assume Transparency.TopLeft as it is the most common case.
                        default:
                            // case Transparency.TopLeft:
                            bmp.MakeTransparent(bmp.GetPixel(0, 0));
                            break;
                        case Transparency.TopRight:
                            bmp.MakeTransparent(bmp.GetPixel(bmp.Width - 1, 0));
                            break;
                        case Transparency.BottomLeft:
                            bmp.MakeTransparent(bmp.GetPixel(0, bmp.Height - 1));
                            break;
                        case Transparency.BottomRight:
                            bmp.MakeTransparent(bmp.GetPixel(bmp.Width - 1, bmp.Height - 1));
                            break;
                    }
                }

                LgrImages.Add(new LgrImage(lgrImageName, bmp, imgType, imgDistance, imgClippingType));
            }
        }

        public void Dispose()
        {
            foreach (LgrImage x in LgrImages)
            {
                x.Bmp.Dispose();
                x.Bmp = null;
            }
        }

        internal LgrImage ImageFromName(string name)
        {
            return LgrImages.FirstOrDefault(x => x.Name == name);
        }

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
                new(EnumSpecialNames().Union(TransparencyIgnoreSet));

            internal ClippingType ClippingType;
            internal int Distance;
            internal string Name;
            internal ImageType Type;
            internal Transparency Transparency;

            public bool IsSpecial => SpecialNames.Contains(Name);
        }

        internal class LgrImage
        {
            internal Bitmap Bmp;
            internal ClippingType ClippingType;
            internal int Distance;
            internal string Name;
            internal ImageType Type;

            internal LgrImage(string name, Bitmap bmp, ImageType type, int dist, ClippingType ctype)
            {
                Name = name;
                Bmp = bmp;
                Type = type;
                Distance = dist;
                ClippingType = ctype;
            }
        }
    }
}