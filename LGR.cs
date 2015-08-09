using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Elmanager
{
    internal class Lgr : IDisposable
    {
        internal LgrImage[] LgrImages;
        internal ListedImage[] ListedImages;

        internal enum ImageType
        {
            Picture = 100,
            Texture = 101,
            Mask = 102
        }

        internal Lgr(string lgrFile)
        {
            byte[] lgrData = File.ReadAllBytes(lgrFile);
            if (Encoding.ASCII.GetString(lgrData, 0, 5) != "LGR12")
                throw (new Exception("The specified LGR file " + lgrFile + " is not valid!"));
            int numberOfPcXs = BitConverter.ToInt32(lgrData, 5);
            int numberOfOptPcXs = BitConverter.ToInt32(lgrData, 13);
            Array.Resize(ref ListedImages, numberOfOptPcXs);
            Array.Resize(ref LgrImages, numberOfPcXs);
            for (int i = 0; i < numberOfOptPcXs; i++)
            {
                ListedImages[i] = new ListedImage
                                      {
                                          Name = Utils.ReadNullTerminatedString(lgrData, i * 10 + 17, 10).ToLower(),
                                          Type =
                                              (ImageType)
                                              (BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 10 + i * 4)),
                                          Distance = BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 14 + i * 4),
                                          ClippingType =
                                              (Level.ClippingType)
                                              (BitConverter.ToInt32(lgrData, 17 + numberOfOptPcXs * 18 + i * 4))
                                      };
            }
            int sp = 17 + numberOfOptPcXs * 26;
            for (int i = 0; i < numberOfPcXs; i++)
            {
                string lgrImageName =
                    Path.GetFileNameWithoutExtension(Utils.ReadNullTerminatedString(lgrData, sp, 12)).ToLower();
                ImageType imgType = ImageType.Picture;
                int imgDistance = 500;
                Level.ClippingType imgClippingType = Level.ClippingType.Unclipped;
                foreach (ListedImage x in ListedImages)
                {
                    if (x.Name == lgrImageName)
                    {
                        imgType = x.Type;
                        imgClippingType = x.ClippingType;
                        imgDistance = x.Distance;
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
                        bmp.MakeTransparent(bmp.GetPixel(0, 0));
                    LgrImages[i] = new LgrImage(lgrImageName, bmp, imgType, imgDistance, imgClippingType);
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