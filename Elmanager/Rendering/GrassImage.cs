using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Elmanager.Lgr;

namespace Elmanager.Rendering;

internal record GrassImage(LgrImage Image, int Delta)
{
    public GrassImage SetAlphaIgnore(int alphaValue, int heightExtension)
    {
        var bmp = new Bitmap(Image.Bmp.Width, Image.Bmp.Height + heightExtension, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bmp))
        {
            g.DrawImage(Image.Bmp, 0, heightExtension);
        }

        var bmpData = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format32bppArgb
        );
        var size = bmpData.Stride * bmpData.Height;
        var data = new int[size / 4];
        var perRow = bmpData.Stride / 4;
        Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

        for (var x = 0; x < bmp.Width; x++)
        {
            for (var y = bmp.Height - 1; y >= 0; y--)
            {
                var pixel = data[y * perRow + x];
                if ((pixel & 0xFF000000) != 0)
                {
                    break;
                }

                // When rendering, we'll discard the bottom pixels with a magic alpha value.
                // This way it's easy to render qgrass at the top part using a suitable blend function.
                data[y * perRow + x] = alphaValue << 24;
            }
        }

        Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
        bmp.UnlockBits(bmpData);
        return this with { Image = Image with { Bmp = bmp } };
    }
}
