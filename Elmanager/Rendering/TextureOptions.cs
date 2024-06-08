using OpenTK.Graphics.OpenGL;

namespace Elmanager.Rendering;

internal record TextureOptions(
    TextureWrapMode WrapMode = TextureWrapMode.Repeat
);
