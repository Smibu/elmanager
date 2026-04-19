using System;
using Elmanager.Rendering.OpenGL;

namespace Elmanager.Rendering;

internal record BikeTextures(
    Texture Wheel,
    Texture Head,
    Texture Bike,
    Texture Body,
    Texture Thigh,
    Texture Leg,
    Texture Forarm,
    Texture UpArm,
    Texture Susp1,
    Texture Susp2
) : IDisposable
{
    public void Dispose()
    {
        Wheel.Dispose();
        Head.Dispose();
        Bike.Dispose();
        Body.Dispose();
        Thigh.Dispose();
        Leg.Dispose();
        Forarm.Dispose();
        UpArm.Dispose();
        Susp1.Dispose();
        Susp2.Dispose();
    }
}
