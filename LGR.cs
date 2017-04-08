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
        internal readonly List<LgrImage> LgrImages = new List<LgrImage>();
        internal readonly List<ListedImage> ListedImages = new List<ListedImage>();

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

        internal IEnumerable<ListedImage> ListedImagesIncludingGrass
        {
            get
            {
                foreach (var listedImage in ListedImages)
                {
                    if (listedImage.Name[0] != 'q')
                    {
                        yield return listedImage;
                    }
                }
                var grassImg = GetImage("QGRASS");
                if (grassImg != null)
                {
                    yield return new ListedImage
                    {
                        ClippingType = Level.ClippingType.Ground,
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
            byte[] lgrData = File.ReadAllBytes(lgrFile);
            if (Encoding.ASCII.GetString(lgrData, 0, 5) != "LGR12")
                throw (new Exception("The specified LGR file " + lgrFile + " is not valid!"));
            int numberOfPcXs = BitConverter.ToInt32(lgrData, 5);
            int numberOfOptPcXs = BitConverter.ToInt32(lgrData, 13);
            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                ListedImages.Add(new ListedImage
                                      {
                                          Name = Utils.ReadNullTerminatedString(lgrData, i * 10 + 17, 10).ToLower(),
                                          Type =
                                              (ImageType)
                                              BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 10 + i * 4),
                                          Distance = BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 14 + i * 4),
                                          ClippingType =
                                              (Level.ClippingType)
                                              BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 18 + i * 4),
                                          Transparency =
                                              (Transparency)
                                              BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 22 + i * 4)
                });
            }
            int sp = 17 + numberOfOptPcXs * 26;
            for (int i = 0; i < numberOfPcXs; i++)
            {
                string lgrImageName =
                    Path.GetFileNameWithoutExtension(Utils.ReadNullTerminatedString(lgrData, sp, 12)).ToLower();
                var isGrass = lgrImageName == "qgrass";
                ImageType imgType = isGrass ? ImageType.Texture : ImageType.Picture;
                int imgDistance = 500;
                Level.ClippingType imgClippingType = isGrass? Level.ClippingType.Ground : Level.ClippingType.Unclipped;
                var transparency = Transparency.TopLeft;
                foreach (ListedImage x in ListedImages)
                {
                    if (x.Name == lgrImageName)
                    {
                        imgType = x.Type;
                        imgClippingType = x.ClippingType;
                        imgDistance = x.Distance;
                        transparency = x.Transparency;
                    }
                }
                sp += 24;
                int sizeOfPcx = BitConverter.ToInt32(lgrData, sp - 4);
                byte[] data = new byte[sizeOfPcx];
                Array.ConstrainedCopy(lgrData, sp, data, 0, sizeOfPcx);
                MemoryStream memStream = new MemoryStream(data);
                try
                {
                    Bitmap bmp = new Pcx(memStream).ToBitmap();
                    if (imgType != ImageType.Texture)
                    {
                        switch (transparency)
                        {
                            case Transparency.NotTransparent:
                                break;
                            case Transparency.Palette0:
                                bmp.MakeTransparent(Color.Black); // TODO get from palette index 0
                                break;
                            case Transparency.TopLeft:
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
                            default:
                                throw new ArgumentOutOfRangeException(nameof(transparency));
                        }
                    }
                    LgrImages.Add(new LgrImage(lgrImageName, bmp, imgType, imgDistance, imgClippingType));
                }
                finally
                {
                    memStream.Close();
                }
                sp += sizeOfPcx;
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
            internal Level.ClippingType ClippingType;
            internal int Distance;
            internal string Name;
            internal ImageType Type;
            internal Transparency Transparency;
        }

        internal class LgrImage
        {
            internal Bitmap Bmp;
            internal Level.ClippingType ClippingType;
            internal int Distance;
            internal string Name;
            internal ImageType Type;

            internal LgrImage(string name, Bitmap bmp, ImageType type, int dist, Level.ClippingType ctype)
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