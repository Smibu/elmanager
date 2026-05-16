using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Elmanager.IO;
using SkiaSharp;

namespace Elmanager.Lgr;

public class Lgr : IDisposable
{
    public readonly Dictionary<string, LgrImage> LgrImages = new();
    private readonly Dictionary<string, ListedImage> _listedImages = new();
    internal readonly string Path;

    public static readonly HashSet<string> TransparencyIgnoreSet =
        new(Enumerable.Range(0, 19).SelectMany(TransparencyIgnoreHelper));

    private static IEnumerable<string> TransparencyIgnoreHelper(int i)
    {
        yield return $"qfood{i}";
        yield return $"qup_{i}";
        yield return $"qdown_{i}";
    }

    public IEnumerable<ListedImage> ListedImagesExcludingSpecial
    {
        get
        {
            foreach (var listedImage in _listedImages.Values)
            {
                if (!listedImage.IsSpecial)
                {
                    yield return listedImage;
                }
            }

            var grassImg = ImageFromName("qgrass");
            if (grassImg != null)
            {
                yield return new ListedImage(
                    new ImageMeta(grassImg.Name, ImageType.Texture, ClippingType.Ground, 500),
                    Transparency.Palette0);
            }
        }
    }

    internal Lgr(string lgrFile)
    {
        using var stream = File.OpenRead(lgrFile);
        Path = lgrFile;
        var lgr = new BinaryReader(stream, Encoding.ASCII);
        var lgrId = lgr.ReadString(5);
        if (lgrId != "LGR12")
        {
            if (lgrId == "LGR13")
            {
                throw new Exception("Steam Elma LGRs are not supported.");
            }
            else
            {
                throw new Exception($"Expected LGR version string 'LGR12', found '${lgrId}'.");
            }

        }
        int numberOfPcXs = lgr.ReadInt32();
        lgr.ReadInt32(); // pictures.lst version, not needed
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
            pcxTypes.Add((ImageType)lgr.ReadInt32());
        }

        for (int i = 0; i < numberOfOptPcXs; i++)
        {
            distances.Add(lgr.ReadInt32());
        }

        for (int i = 0; i < numberOfOptPcXs; i++)
        {
            clippingTypes.Add((ClippingType)lgr.ReadInt32());
        }

        for (int i = 0; i < numberOfOptPcXs; i++)
        {
            transparencies.Add((Transparency)lgr.ReadInt32());
        }

        for (int i = 0; i < numberOfOptPcXs; i++)
        {
            var name = pcxNames[i].ToLower();
            _listedImages[name] = new ListedImage(new ImageMeta(name, pcxTypes[i],
                Distance: distances[i],
                ClippingType: clippingTypes[i]), transparencies[i]);
        }

        for (int i = 0; i < numberOfPcXs; i++)
        {
            var lgrImageName = System.IO.Path.GetFileNameWithoutExtension(lgr.ReadNullTerminatedString(12)).ToLower();
            var isGrass = lgrImageName == "qgrass";
            var imgType = isGrass ? ImageType.Texture : ImageType.Picture;
            int imgDistance = 500;
            var imgClippingType = isGrass ? ClippingType.Ground : ClippingType.Unclipped;
            var transparency = Transparency.TopLeft;
            var imageData = new ImageMeta(lgrImageName, imgType, imgClippingType, imgDistance);
            if (_listedImages.TryGetValue(lgrImageName, out var x))
            {
                imageData = x.Data;
                if (!TransparencyIgnoreSet.Contains(x.Name))
                {
                    transparency = x.Transparency;
                }
            }

            lgr.ReadInt32(); // unknown, not used
            lgr.ReadInt32(); // unknown, not used
            lgr.ReadInt32(); // size of PCX
            SKBitmap bmp = new Pcx(stream).ToBitmap();
            if (imageData.Type != ImageType.Texture)
            {
                switch (transparency)
                {
                    case Transparency.NotTransparent:
                        break;
                    case Transparency.Palette0:
                        MakeTransparent(bmp, new SKColor(0, 0, 0)); // TODO get from palette index 0
                        break;
                    // If the transparency field value is invalid, we'll assume Transparency.TopLeft as it is the most common case.
                    default:
                        // case Transparency.TopLeft:
                        MakeTransparent(bmp, bmp.GetPixel(0, 0));
                        break;
                    case Transparency.TopRight:
                        MakeTransparent(bmp, bmp.GetPixel(bmp.Width - 1, 0));
                        break;
                    case Transparency.BottomLeft:
                        MakeTransparent(bmp, bmp.GetPixel(0, bmp.Height - 1));
                        break;
                    case Transparency.BottomRight:
                        MakeTransparent(bmp, bmp.GetPixel(bmp.Width - 1, bmp.Height - 1));
                        break;
                }
            }

            LgrImages[imageData.Name] = new LgrImage(imageData, bmp);
        }
    }

    public void Dispose()
    {
        foreach (var x in LgrImages.Values)
        {
            x.Bmp.Dispose();
        }
    }

    public LgrImage? ImageFromName(string name) => LgrImages.GetValueOrDefault(name);

    private static unsafe void MakeTransparent(SKBitmap bmp, SKColor transparentColor)
    {
        var pixels = (uint*)bmp.GetPixels();
        var count = bmp.Width * bmp.Height;
        var target = ((uint)transparentColor.Alpha << 24) | ((uint)transparentColor.Blue << 16) |
                     ((uint)transparentColor.Green << 8) | transparentColor.Red;
        for (var i = 0; i < count; i++)
        {
            if (pixels[i] == target)
            {
                pixels[i] = 0;
            }
        }
    }
}
