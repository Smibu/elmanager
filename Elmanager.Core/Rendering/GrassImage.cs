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

    private unsafe void SetAlphaIgnore(int alphaValue)
    {
        var bmp = Image.Bmp;
        var pixels = (uint*)bmp.GetPixels();
        var width = bmp.Width;
        var height = bmp.Height;

        for (var x = 0; x < width; x++)
        {
            for (var y = height - 1; y >= 0; y--)
            {
                var pixel = pixels[y * width + x];
                if ((pixel & 0xFF000000) != 0)
                {
                    break;
                }

                // When rendering, we'll discard the bottom pixels with a magic alpha value.
                // This way it's easy to render qgrass at the top part in the shader.
                pixels[y * width + x] = (uint)(alphaValue << 24);
            }
        }
    }
}
