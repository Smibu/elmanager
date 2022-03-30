using System;
using System.ComponentModel;
using System.Linq;
using Elmanager.ElmaPrimitives;
using Elmanager.IO;
using Elmanager.Lev;

namespace Elmanager.LevelManager;

internal record LevelItem(
    ElmaFileObject<Level> Efo,
    [property: Description("Replays")] int Replays
    ) : IElmaFileObject
{
    public Level Lev => Efo.Obj;

    [Description("LGR")] public string LgrFile => Lev.LgrFile;

    [Description("Single 1st")]
    public ElmaTime? BestSingleTime =>
        Lev.Top10.SinglePlayer.Count > 0 ? Lev.Top10.SinglePlayer[0].TimeInSecs : null;

    [Description("Multi 1st")]
    public ElmaTime? BestMultiTime =>
        Lev.Top10.MultiPlayer.Count > 0 ? Lev.Top10.MultiPlayer[0].TimeInSecs : null;

    [Description("Times")] public int SingleTimesCount => Lev.Top10.SinglePlayer.Count;

    [Description("Times (m)")] public int MultiTimesCount => Lev.Top10.MultiPlayer.Count;

    [Description("Grav.")]
    public int GravApples => Lev.Objects.Count(x => x.Type == ObjectType.Apple && x.AppleType != AppleType.Normal);

    [Description("Textures")] public int TextureCount => Lev.GraphicElementFileItems.Count(texture => !texture.IsPicture);

    [Description("Pictures")] public int PictureCount => Lev.GraphicElementFileItems.Count(texture => texture.IsPicture);

    [Description("Apples")] public int AppleObjectCount => Lev.AppleObjectCount;

    [Description("Flowers")] public int ExitObjectCount => Lev.ExitObjectCount;

    [Description("Killers")] public int KillerObjectCount => Lev.KillerObjectCount;

    [Description("Polys")] public int PolygonCount => Lev.PolygonCount;

    [Description("Grass ps")] public int GrassPolygonCount => Lev.GrassPolygonCount;

    [Description("Grass vs")] public int GrassVertexCount => Lev.GrassVertexCount;

    [Description("Ground ps")] public int GroundPolygonCount => Lev.GroundPolygonCount;

    [Description("Ground vs")] public int GroundVertexCount => Lev.GroundVertexCount;

    [Description("Width")] public double Width => Lev.Width;

    [Description("Height")] public double Height => Lev.Height;

    [Description("Title")] public string Title => Lev.Title;

    [Description("File name")] public string FileNameNoExt => Efo.File.FileNameNoExt;

    [Description("Date modified")] public DateTime DateModified => Efo.File.DateModified;

    [Description("Size (kB)")] public double SizeInKb => Efo.File.SizeInKb;

    public string Path => Efo.File.Path;
}