using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering;

internal record TextureOptions(
    RotateFlipType Flip = RotateFlipType.RotateNoneFlipNone,
    TextureWrapMode WrapMode = TextureWrapMode.Repeat
);
