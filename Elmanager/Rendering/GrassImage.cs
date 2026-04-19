using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Elmanager.Lgr;

namespace Elmanager.Rendering;

internal class GrassImage
{
    private const int GrassIgnoreAlpha = 0x7F;

    public GrassImage(LgrImage image, int delta)
    {
        Image = image;
        Delta = delta;
        SetAlphaIgnore(GrassIgnoreAlpha);
    }

    public LgrImage Image { get; }
    public int Delta { get; }

    private void SetAlphaIgnore(int alphaValue)
    {
        var bmp = Image.Bmp;
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
                // This way it's easy to render qgrass at the top part in the shader.
                data[y * perRow + x] = alphaValue << 24;
            }
        }

        Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
        bmp.UnlockBits(bmpData);
    }
}
