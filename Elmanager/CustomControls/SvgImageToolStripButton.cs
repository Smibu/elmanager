using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SharpVectors.Runtime;

namespace Elmanager.CustomControls
{
    internal class SvgImageToolStripButton : ToolStripButton
    {
        private byte[] _svgData;

        public byte[] SvgData
        {
            get => _svgData;
            set
            {
                _svgData = value;
                var s = new WpfDrawingSettings
                {
                    EnsureViewboxSize = true,
                    PixelWidth = 64,
                    PixelHeight = 64,
                    DpiScale = new DpiScale(100),
                    InteractiveMode = SvgInteractiveModes.None,
                    OptimizePath = false
                };
                var c = new StreamSvgConverter(s);
                using var dest = new MemoryStream();
                using var src = new MemoryStream(_svgData);
                if (!c.Convert(src, dest))
                {
                    throw new ArgumentException("Invalid SVG data.");
                }

                var bmp = Image.FromStream(dest);
                Image = bmp;
            }
        }
    }
}