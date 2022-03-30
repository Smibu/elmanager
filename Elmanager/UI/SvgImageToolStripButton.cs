using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Runtime;

namespace Elmanager.UI;

internal class SvgImageToolStripButton : ToolStripButton
{
    private byte[]? _svgData;
    private static readonly StreamSvgConverter SvgConverter = new(new WpfDrawingSettings
    {
        EnsureViewboxSize = true,
        PixelWidth = 64,
        PixelHeight = 64,
        DpiScale = new DpiScale(100),
        InteractiveMode = SvgInteractiveModes.None,
        OptimizePath = false
    });

    public byte[]? SvgData
    {
        get => _svgData;
        set
        {
            _svgData = value;
            if (_svgData is null)
            {
                return;
            }
            using var dest = new MemoryStream();
            using var src = new MemoryStream(_svgData);
            if (!SvgConverter.Convert(src, dest))
            {
                throw new ArgumentException("Invalid SVG data.");
            }

            var bmp = Image.FromStream(dest);
            Image = bmp;
        }
    }
}