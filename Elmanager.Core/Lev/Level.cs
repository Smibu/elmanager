using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elmanager.ElmaPrimitives;
using Elmanager.Geometry;
using Elmanager.IO;
using Elmanager.Lgr;
using Elmanager.Rendering;
using Elmanager.Utilities;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Polygonize;

namespace Elmanager.Lev;

public class Level
{
    private enum RingOrientation
    {
        Clockwise,
        CounterClockwise
    }

    internal const double GlobalBodyDifferenceFromLeftWheelX = 0.85;
    internal const double GlobalBodyDifferenceFromLeftWheelY = 0.6;
    public const double HeadDifferenceFromLeftWheelX = 0.7595;
    public const double HeadDifferenceFromLeftWheelY = 1.67;
    public const int MaximumObjectCount = 252;
    public const int MaximumPolygonCount = 1200;
    private const int MaximumPolygonVertexCount = 1000;
    public const double MaximumSize = 188.0;
    public const int MaximumGroundVertexCount = 20000;
    public const int MaximumPictureTextureCount = 5000;
    public const double RightWheelDifferenceFromLeftWheelX = 1.698;
    private const uint EndOfDataMagicNumber = 0x67103A;
    private const uint EndOfDataMagicNumber2 = 0xB76A0515;
    private const uint EndOfFileMagicNumber = 0x845D52;
    private const double MagicDouble = 0.4643643;
    private const double MagicDouble2 = 0.2345672;

    private const int Top10NameSize = 15;
    private const int Top10Entries = 10;

    public static readonly string[] InternalTitles =
    {
        "Warm Up", "Flat Track", "Twin Peaks", "Over and Under",
        "Uphill Battle", "Long Haul", "Hi Flyer", "Tag",
        "Tunnel Terror", "The Steppes", "Gravity Ride",
        "Islands in the Sky", "Hill Legend", "Loop-de-Loop",
        "Serpents Tale", "New Wave", "Labyrinth", "Spiral",
        "Turnaround", "Upside Down", "Hangman", "Slalom",
        "Quick Round", "Ramp Frenzy", "Precarious", "Circuitous",
        "Shelf Life", "Bounce Back", "Headbanger", "Pipe",
        "Animal Farm", "Steep Corner", "Zig-Zag", "Bumpy Journey",
        "Labyrinth Pro", "Fruit in the Den", "Jaws", "Curvaceous",
        "Haircut", "Double Trouble", "Framework", "Enduro",
        "He He",
        "Freefall", "Sink", "Bowling", "Enigma", "Downhill",
        "What the Heck", "Expert System", "Tricks Abound",
        "Hang Tight", "Hooked", "Apple Harvest", "More Levels"
    };

    public string GroundTextureName = "ground";
    private readonly double[] _integrity = new double[4];

    public List<LevObject> Objects = new();
    public List<GraphicElement> GraphicElements = new();
    public List<Polygon> Polygons = new();
    public string SkyTextureName = "sky";
    public readonly LevelTop10 Top10 = new();
    public int Identifier { get; private set; }

    public string LgrFile = "default";
    private string _title = "New level";
    private Bounds _polygonBounds;
    private string LevStartMagic { get; set; } = "POT14";

    public Level()
    {
    }

    private Level(Polygon startPolygon, Vector startPosition, Vector exitPosition)
    {
        Polygons.Add(startPolygon);
        Objects.Add(LevObject.StartObject(startPosition));
        Objects.Add(LevObject.ExitObject(exitPosition));
        UpdateBounds();
    }

    public static ElmaFileObject<Level> FromPath(string levelPath)
    {
        using var stream = File.OpenRead(levelPath);
        var lev = FromStream(stream);
        return ElmaFileObject<Level>.FromPath(levelPath, lev);
    }

    public static Level FromStream(Stream data)
    {
        var lvl = new Level();
        try
        {
            lvl.LoadFromStream(data);
        }
        catch (EndOfStreamException)
        {
            throw new BadFileException("Level file is corrupted");
        }
        return lvl;
    }

    private void LoadFromStream(Stream data)
    {
        var lev = new BinaryReader(data, Encoding.ASCII);
        LevStartMagic = lev.ReadString(5);
        if (!IsElmaLevel && !IsAcrossLevel && !IsLeb)
        {
            throw new BadFileException(
                "Unknown file type. This is neither an Elma level, an Across level nor a LEB file.");
        }

        if (!IsAcrossLevel)
        {
            lev.ReadInt16();
        }

        Identifier = lev.ReadInt32();
        for (var i = 0; i < 4; i++)
        {
            _integrity[i] = lev.ReadDouble();
        }

        Title = lev.ReadNullTerminatedString(IsAcrossLevel ? 59 : 51);
        if (!IsAcrossLevel)
        {
            LgrFile = lev.ReadNullTerminatedString(16);
            GroundTextureName = lev.ReadNullTerminatedString(10).ToLower();
            SkyTextureName = lev.ReadNullTerminatedString(10).ToLower();
        }

        var polygonCount = (int)Math.Round(lev.ReadDouble() - MagicDouble);
        var objectCount = -1;
        if (IsLeb)
        {
            objectCount = (int)Math.Round(lev.ReadDouble() - MagicDouble);
        }

        Polygons = new List<Polygon>();
        for (var i = 0; i < polygonCount; i++)
        {
            var isGrassPolygon = false;
            if (!IsAcrossLevel)
            {
                isGrassPolygon = lev.ReadInt32() == 1;
            }

            var numVertice = lev.ReadInt32();
            var poly = new Polygon();
            for (var j = 0; j < numVertice; j++)
            {
                var x = lev.ReadDouble();
                var y = -lev.ReadDouble();
                poly.Add(new Vector(x, y));
            }

            poly.IsGrass = isGrassPolygon;
            Polygons.Add(poly);
        }

        if (!IsLeb)
        {
            objectCount = (int)Math.Round(lev.ReadDouble() - MagicDouble);
        }

        var startFound = false;
        for (var i = 0; i < objectCount; i++)
        {
            var x = lev.ReadDouble();
            var y = -lev.ReadDouble();
            var objectType = (ObjectType)lev.ReadInt32();
            if (objectType == ObjectType.Start)
            {
                startFound = true;
            }

            var appleType = AppleType.Normal;
            var animNum = 0;
            if (!IsAcrossLevel)
            {
                appleType = (AppleType)lev.ReadInt32();
                animNum = lev.ReadInt32();
            }

            var objectToAdd = new LevObject(new Vector(x, y), objectType, appleType, animNum + 1);
            Objects.Add(objectToAdd);
        }

        if (!startFound)
        {
            Objects.Add(new LevObject(new Vector(0, 0), ObjectType.Start, AppleType.Normal));
        }

        if (!IsAcrossLevel)
        {
            var numberOfPicturesPlusTextures = (int)Math.Round(lev.ReadDouble() - MagicDouble2);
            for (var i = 0; i < numberOfPicturesPlusTextures; i++)
            {
                var pictureName = lev.ReadNullTerminatedString(10);
                var textureName = lev.ReadNullTerminatedString(10);
                var maskName = lev.ReadNullTerminatedString(10);
                var x = lev.ReadDouble();
                var y = -lev.ReadDouble();
                var distance = lev.ReadInt32();
                var clipping = (ClippingType)lev.ReadInt32();
                if (pictureName == "")
                    GraphicElements.Add(GraphicElement.MissingText(textureName,
                        maskName,
                        clipping,
                        distance,
                        new Vector(x, y)
                    ));
                else
                    GraphicElements.Add(GraphicElement.MissingPic(pictureName,
                        clipping,
                        distance,
                        new Vector(x, y)
                    ));
            }
        }

        HandleLevEndRead(lev);
        UpdateBounds();
    }

    private void HandleLevEndRead(BinaryReader lev)
    {
        uint endOfData;
        try
        {
            endOfData = lev.ReadUInt32();
        }
        catch (EndOfStreamException)
        {
            return;
        }

        if (endOfData != EndOfDataMagicNumber && endOfData != EndOfDataMagicNumber2)
        {
            throw new BadFileException($"Wrong end of data marker: {endOfData:X}");
        }

        if (lev.PeekChar() == -1)
        {
            return;
        }

        try
        {
            var top10data = lev.ReadBytes(688);
            CryptTop10(top10data, 0);
            Top10.SinglePlayer = ReadTop10Part(top10data, 0, (a, b, t) => new Top10EntrySingle(a, b, t));
            Top10.MultiPlayer = ReadTop10Part(top10data, 0 + 344, (a, b, t) => new Top10EntryMulti(a, b, t));
        }
        catch (Exception)
        {
            // just ignore corrupted top10 for now
        }
    }

    private Level(Level lev)
    {
        LevStartMagic = lev.LevStartMagic;
        foreach (var x in lev.Objects)
            Objects.Add(x.Clone());
        foreach (var x in lev.Polygons)
            Polygons.Add(x.Clone());
        Bounds = lev.Bounds;
        for (var i = 0; i <= 3; i++)
            _integrity[i] = lev._integrity[i];
        _title = lev._title;
        LgrFile = lev.LgrFile;
        GroundTextureName = lev.GroundTextureName;
        SkyTextureName = lev.SkyTextureName;
        foreach (var z in lev.GraphicElements)
        {
            GraphicElements.Add(z with { });
        }
    }


    public int AppleObjectCount => Objects.Count(x => x.Type == ObjectType.Apple);

    public int ExitObjectCount => Objects.Count(x => x.Type == ObjectType.Flower);

    public int KillerObjectCount => Objects.Count(x => x.Type == ObjectType.Killer);

    public int PolygonCount => Polygons.Count;

    public int GrassPolygonCount => Polygons.Count(x => x.IsGrass);

    public int GrassVertexCount =>
        Polygons.Where(x => x.IsGrass).Sum(x => x.Vertices.Count);

    public int GroundPolygonCount => Polygons.Count(x => !x.IsGrass);

    public int GroundVertexCount => Polygons.Where(x => !x.IsGrass).Sum(x => x.Vertices.Count);

    public bool HasTooLargePolygons
    {
        get
        {
            var foundTooLarge = false;
            foreach (var x in Polygons.Where(x => x.Vertices.Count > MaximumPolygonVertexCount))
            {
                x.Mark = PolygonMark.Erroneous;
                foundTooLarge = true;
            }

            return foundTooLarge;
        }
    }

    public List<Vector> GetTooShortEdges()
    {
        var result = new List<Vector>();
        foreach (var p in Polygons)
        {
            for (var i = 0; i < p.Vertices.Count; i++)
            {
                if ((p[i] - p[i + 1]).Length < 0.00000001)
                {
                    result.Add(p[i]);
                }
            }
        }

        return result;
    }

    public int GetGravityAppleCount(AppleType t)
    {
        return Objects.Count(o => o.Type == ObjectType.Apple && o.AppleType == t);
    }

    public bool HasTooManyObjects => Objects.Count > MaximumObjectCount;

    public bool HasTooManyPolygons => Polygons.Count > MaximumPolygonCount;

    public bool HasTooManyVertices => GroundVertexCount > MaximumGroundVertexCount;

    private bool HasTopologyErrors => HasTooLargePolygons || HasTooManyObjects || HasTooFewObjects ||
                                      HasTooManyPolygons || HasTooManyVertices || HasTooManyPictures ||
                                      WheelLiesOnEdge || HasTexturesOutOfBounds || HeadTouchesGround || TooTall ||
                                      TooWide || GetIntersectionPoints().Count > 0 || GetTooShortEdges().Count > 0;

    public bool HasTooManyPictures => PictureTextureCount > MaximumPictureTextureCount;

    public bool HeadTouchesGround
    {
        get
        {
            var head = (from x in Objects
                        where x.Type == ObjectType.Start
                        select
                            new Vector(x.Position.X + HeadDifferenceFromLeftWheelX,
                                x.Position.Y + HeadDifferenceFromLeftWheelY)).FirstOrDefault();
            return Polygons.Where(poly => !poly.IsGrass).Any(x => x.DistanceFromPoint(head) < ElmaConstants.HeadRadius);
        }
    }

    public bool WheelLiesOnEdge
    {
        get
        {
            var leftWheel = Objects.First(x => x.Type == ObjectType.Start).Position;
            var rightWheel = new Vector(leftWheel.X + RightWheelDifferenceFromLeftWheelX, leftWheel.Y);
            const double wheelTolerance = 1e-6;
            return Polygons.Where(poly => !poly.IsGrass).Any(x =>
                x.DistanceFromPoint(leftWheel) < wheelTolerance ||
                x.DistanceFromPoint(rightWheel) < wheelTolerance);
        }
    }

    public bool HasTexturesOutOfBounds
    {
        get
        {
            var padding = 11.898;
            return GraphicElements.Where(p => p is GraphicElement.Texture).Any(p =>
                p.Position.X < _polygonBounds.XMin - padding ||
                p.Position.X > _polygonBounds.XMax + padding ||
                p.Position.Y < _polygonBounds.YMin - padding ||
                p.Position.Y > _polygonBounds.YMax + padding);
        }
    }

    public double Width => Bounds.XMax - Bounds.XMin;

    public double Height => Bounds.YMax - Bounds.YMin;

    public int TextureCount => GraphicElements.Count(texture => texture is GraphicElement.Texture or GraphicElement.MissingTexture);

    public int PictureCount => GraphicElements.Count(texture => texture is GraphicElement.Picture or GraphicElement.MissingPicture);

    public int PictureTextureCount => GraphicElements.Count;

    public string Title
    {
        get => _title;
        set
        {
            if (value.Length > 50)
                throw (new ArgumentException("The specified level title is too long!"));
            _title = value;
        }
    }

    public bool TooTall => _polygonBounds.YMax - _polygonBounds.YMin >= MaximumSize;

    public bool TooWide => _polygonBounds.XMax - _polygonBounds.XMin >= MaximumSize;

    public int VertexCount => Polygons.Sum(x => x.Vertices.Count);

    private double ObjectSum => Objects.Sum(x => x.Position.X - x.Position.Y + (int)x.Type);

    private double PictureSum => GraphicElements.Sum(x => x.Position.X - x.Position.Y);

    private double PolygonSum => Polygons.Sum(x => x.Vertices.Sum(v => v.X - v.Y));

    private bool IsElmaLevel => LevStartMagic == "POT14";

    internal bool IsAcrossLevel => LevStartMagic == "POT06";

    private bool IsLeb => LevStartMagic == "@@^!@";

    public bool HasTooFewObjects => Objects.Count < 2;

    public static string GetPossiblyInternal(string level)
    {
        if (IsInternalLevel(level))
        {
            var index = int.Parse(level.Substring(6, 2));
            return index + " - " + InternalTitles[index - 1];
        }

        return Path.GetFileNameWithoutExtension(level);
    }

    internal static bool IsInternalLevel(string levStr) =>
        levStr.Length == 12 && levStr.Substring(0, 6).EqualsIgnoreCase("QWQUU0");

    public Level Clone() => new(this);

    public void UpdateGrass(double grassZoom)
    {
        var groundBounds = GetGroundBounds();
        foreach (var x in Polygons)
        {
            x.UpdateGrassSlopeInfo(groundBounds, grassZoom);
        }
    }

    public List<LevObject> GetApplesAndFlowersInsideGround()
    {
        return
            Objects.FindAll(
                x => (x.Type == ObjectType.Apple || x.Type == ObjectType.Flower) && IsObjectInsideGround(x));
    }

    public List<Vector> GetIntersectionPoints() => GeometryUtils.GetIntersectionPoints(Polygons);

    public void Import(Level other)
    {
        UpdateBounds();
        other.UpdateBounds();

        var xDiff = Bounds.XMax - other.Bounds.XMin + 5;
        var yDiff = Bounds.YMax - other.Bounds.YMax;

        var diffV = new Vector(xDiff, yDiff);

        other.Polygons.ForEach(poly => poly.Move(diffV));
        other.Objects.ForEach(obj => obj.Position += diffV);
        other.Objects.RemoveAll(obj => obj.Type == ObjectType.Start);
        other.GraphicElements.ForEach(pic => pic.Position += diffV);

        Polygons.AddRange(other.Polygons);
        Objects.AddRange(other.Objects);
        GraphicElements.AddRange(other.GraphicElements);

        UpdateBounds();
    }

    private bool IsObjectInsideGround(LevObject o)
    {
        return !Polygons.Any(polygon => polygon.DistanceFromPoint(o.Position) < OpenGlLgr.ObjectRadius) &&
               IsPointInGround(o.Position);
    }

    private bool IsPointInGround(Vector p)
    {
        var clipping = Polygons.Count(x => !x.IsGrass && x.AreaHasPoint(p));
        return clipping % 2 == 0;
    }

    public bool IsSky(Polygon poly)
    {
        var counter = new RayCrossingCounter(poly.Vertices[0]);
        foreach (var p in Polygons.Where(p => !p.IsGrass && !p.Equals(poly)))
        {
            for (var i = 0; i < p.Vertices.Count - 1; i++)
            {
                counter.CountSegment(p.Vertices[i], p.Vertices[i + 1]);
            }
            counter.CountSegment(p.Vertices[^1], p.Vertices[0]);
        }

        return counter.Location == Location.Exterior;
    }

    public void MirrorSelected(MirrorOption mirrorOption)
    {
        var xMin = double.PositiveInfinity;
        var xMax = double.NegativeInfinity;
        var yMin = double.PositiveInfinity;
        var yMax = double.NegativeInfinity;
        foreach (var x in Polygons.Where(p => p.Vertices.Any(v => v.Mark == VectorMark.Selected)))
        {
            xMin = Math.Min(xMin,
                x.Vertices.Where(v => v.Mark == VectorMark.Selected).Select(v => v.X).Min());
            xMax = Math.Max(xMax,
                x.Vertices.Where(v => v.Mark == VectorMark.Selected).Select(v => v.X).Max());
            yMax = Math.Max(yMax,
                x.Vertices.Where(v => v.Mark == VectorMark.Selected).Select(v => v.Y).Max());
            yMin = Math.Min(yMin,
                x.Vertices.Where(v => v.Mark == VectorMark.Selected).Select(v => v.Y).Min());
        }

        foreach (var x in Objects.Where(o => o.Position.Mark == VectorMark.Selected))
        {
            xMin = Math.Min(xMin, x.Position.X);
            xMax = Math.Max(xMax, x.Position.X);
            yMax = Math.Max(yMax, x.Position.Y);
            yMin = Math.Min(yMin, x.Position.Y);
        }

        foreach (var x in GraphicElements.Where(p => p.Position.Mark == VectorMark.Selected))
        {
            xMin = Math.Min(xMin, x.Position.X);
            xMax = Math.Max(xMax, x.Position.X + x.Width);
            yMax = Math.Max(yMax, x.Position.Y + x.Height);
            yMin = Math.Min(yMin, x.Position.Y);
        }

        var mirrorMatrix = Matrix.Identity;
        mirrorMatrix.Translate(-(xMax + xMin) / 2, -(yMax + yMin) / 2);
        switch (mirrorOption)
        {
            case MirrorOption.Horizontal:
                mirrorMatrix.Scale(-1.0, 1.0);
                break;
            case MirrorOption.Vertical:
                mirrorMatrix.Scale(1.0, -1.0);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mirrorOption), mirrorOption, null);
        }

        mirrorMatrix.Translate((xMax + xMin) / 2, (yMax + yMin) / 2);
        Transform(mirrorMatrix, v => v.Mark == VectorMark.Selected);
    }

    private void Transform(Matrix matrix, Func<Vector, bool> selector)
    {
        foreach (var x in Polygons)
        {
            for (var i = 0; i < x.Vertices.Count; i++)
            {
                var v = x.Vertices[i];
                if (!selector(v))
                {
                    continue;
                }
                x.Vertices[i] = v.Transform(matrix);
            }

        }

        foreach (var t in Objects.Where(o => selector(o.Position)))
        {
            var z = t.Position;
            if (t.Type == ObjectType.Start)
            {
                var fix = new Vector(RightWheelDifferenceFromLeftWheelX / 2, 0);
                t.Position = t.Position.WithPosition((z + fix) * matrix - fix);
            }
            else
                t.Position = t.Position.Transform(matrix);
        }

        foreach (var z in GraphicElements.Where(p => selector(p.Position)))
        {
            var fix = new Vector(z.Width / 2, -z.Height / 2);
            z.Position = z.Position.WithPosition((z.Position + fix) * matrix - fix);
        }
    }

    public byte[] GetBytes(bool saveAsFresh = true)
    {
        var levelFile = new List<byte>();
        if (saveAsFresh)
        {
            Top10.Clear();
        }

        levelFile.AddRange("POT14"u8.ToArray());
        var rnd = new Random();
        if (saveAsFresh)
        {
            Identifier = rnd.Next();
        }
        levelFile.Add(BitConverter.GetBytes(Identifier)[0]);
        levelFile.Add(BitConverter.GetBytes(Identifier)[1]);
        levelFile.AddRange(BitConverter.GetBytes(Identifier));
        var sum = (PictureSum + ObjectSum + PolygonSum) * 3247.764325643;
        _integrity[0] = sum;
        _integrity[1] = rnd.Next(5871) + 11877 - sum;
        var hasTopologyErrors = HasTopologyErrors;
        if (hasTopologyErrors)
            _integrity[2] = rnd.Next(4982) + 20961 - sum;
        else
            _integrity[2] = rnd.Next(5871) + 11877 - sum;
        _integrity[3] = rnd.Next(6102) + 12112 - sum;
        foreach (var x in _integrity)
            levelFile.AddRange(BitConverter.GetBytes(x));
        levelFile.AddRange(GetByteArrayFromString(Title, 51));
        levelFile.AddRange(GetByteArrayFromString(LgrFile, 16));
        levelFile.AddRange(GetByteArrayFromString(GroundTextureName, 10));
        levelFile.AddRange(GetByteArrayFromString(SkyTextureName, 10));
        levelFile.AddRange(BitConverter.GetBytes(Polygons.Count + MagicDouble));
        WritePolygons(levelFile, hasTopologyErrors);

        levelFile.AddRange(BitConverter.GetBytes(Objects.Count + MagicDouble));
        foreach (var x in Objects)
        {
            levelFile.AddRange(BitConverter.GetBytes(x.Position.X));
            levelFile.AddRange(BitConverter.GetBytes(-x.Position.Y));
            levelFile.AddRange(BitConverter.GetBytes((int)x.Type));
            levelFile.AddRange(BitConverter.GetBytes((int)x.AppleType));
            levelFile.AddRange(BitConverter.GetBytes(x.AnimationNumber - 1));
        }

        levelFile.AddRange(BitConverter.GetBytes(GraphicElements.Count + MagicDouble2));
        foreach (var x in GraphicElements)
        {
            switch (x)
            {
                case GraphicElement.Picture p:
                    levelFile.AddRange(GetByteArrayFromString(p.PictureInfo.Name, 10));
                    levelFile.AddRange(Enumerable.Repeat<byte>(0, 20));
                    break;
                case GraphicElement.MissingPicture p:
                    levelFile.AddRange(GetByteArrayFromString(p.Name, 10));
                    levelFile.AddRange(Enumerable.Repeat<byte>(0, 20));
                    break;
                case GraphicElement.Texture t:
                    levelFile.AddRange(Enumerable.Repeat<byte>(0, 10));
                    levelFile.AddRange(GetByteArrayFromString(t.TextureInfo.Name, 10));
                    levelFile.AddRange(GetByteArrayFromString(t.MaskInfo.Name, 10));
                    break;
                case GraphicElement.MissingTexture t:
                    levelFile.AddRange(Enumerable.Repeat<byte>(0, 10));
                    levelFile.AddRange(GetByteArrayFromString(t.TextureName, 10));
                    levelFile.AddRange(GetByteArrayFromString(t.MaskName, 10));
                    break;
            }

            levelFile.AddRange(BitConverter.GetBytes(x.Position.X));
            levelFile.AddRange(BitConverter.GetBytes(-x.Position.Y));
            levelFile.AddRange(BitConverter.GetBytes(x.Distance));
            levelFile.AddRange(BitConverter.GetBytes((int)x.Clipping));
        }

        levelFile.AddRange(BitConverter.GetBytes(EndOfDataMagicNumber));
        WriteTop10Part(levelFile, Top10.SinglePlayer);
        WriteTop10Part(levelFile, Top10.MultiPlayer);
        CryptTop10(levelFile, levelFile.Count - 688);
        levelFile.AddRange(BitConverter.GetBytes(EndOfFileMagicNumber));
        return levelFile.ToArray();
    }

    private void WritePolygons(List<byte> levelFile, bool hasTopologyErrors)
    {
        if (hasTopologyErrors)
        {
            foreach (var polygon in Polygons)
                WritePolygon(levelFile, polygon);
            return;
        }

        var polygonizer = new Polygonizer(extractOnlyPolygonal: true)
        {
            IsCheckingRingsValid = false
        };
        foreach (var polygon in Polygons.Where(polygon => !polygon.IsGrass))
            polygonizer.Add(polygon.ToIPolygon().Shell);

        var groundPolygons = polygonizer.GetPolygons()
            .Cast<NetTopologySuite.Geometries.Polygon>()
            .ToList();
        var groundRingCount = groundPolygons.Sum(polygon => polygon.NumInteriorRings + 1);
        if (groundRingCount != GroundPolygonCount)
            throw new InvalidOperationException("Polygonization changed the number of ground polygons.");

        foreach (var polygon in groundPolygons)
        {
            WritePolygon(levelFile, polygon.Shell, RingOrientation.Clockwise);
            foreach (var hole in polygon.Holes)
                WritePolygon(levelFile, hole, RingOrientation.CounterClockwise);
        }

        foreach (var polygon in Polygons.Where(polygon => polygon.IsGrass))
            WritePolygon(levelFile, polygon);
    }

    private static void WritePolygon(List<byte> levelFile, Polygon polygon)
    {
        WritePolygonHeader(levelFile, polygon.IsGrass, polygon.Vertices.Count);
        foreach (var vertex in polygon.Vertices)
        {
            levelFile.AddRange(BitConverter.GetBytes(vertex.X));
            levelFile.AddRange(BitConverter.GetBytes(-vertex.Y));
        }
    }

    private static void WritePolygon(List<byte> levelFile, LinearRing ring, RingOrientation orientation)
    {
        var sequence = ring.CoordinateSequence;
        WritePolygonHeader(levelFile, false, sequence.Count - 1);
        var isCounterClockwise = Orientation.IsCCW(sequence);
        var writeForward = orientation switch
        {
            RingOrientation.Clockwise => !isCounterClockwise,
            RingOrientation.CounterClockwise => isCounterClockwise,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
        var start = writeForward ? 0 : sequence.Count - 2;
        var end = writeForward ? sequence.Count - 1 : -1;
        var step = writeForward ? 1 : -1;
        for (var i = start; i != end; i += step)
        {
            levelFile.AddRange(BitConverter.GetBytes(sequence.GetX(i)));
            levelFile.AddRange(BitConverter.GetBytes(-sequence.GetY(i)));
        }
    }

    private static void WritePolygonHeader(List<byte> levelFile, bool isGrass, int vertexCount)
    {
        levelFile.AddRange(BitConverter.GetBytes(isGrass));
        for (var i = 1; i <= 3; i++)
            levelFile.Add(0);
        levelFile.AddRange(BitConverter.GetBytes(vertexCount));
    }

    public void SaveToStream(Stream stream, bool saveAsFresh = true)
    {
        var bytes = GetBytes(saveAsFresh);
        stream.Write(bytes);
    }

    public async Task SaveToStreamAsync(Stream stream, bool saveAsFresh = true)
    {
        var bytes = GetBytes(saveAsFresh);
        await stream.WriteAsync(bytes);
    }

    public ElmaFile Save(string savePath, bool saveAsFresh = true)
    {
        var bytes = GetBytes(saveAsFresh);
        File.WriteAllBytes(savePath, bytes);
        return new ElmaFile(savePath);
    }

    public void UpdateBounds()
    {
        var b = Polygons.Skip(1).Aggregate(Polygons.First().Bounds, (current, x) => current.Max(x.Bounds));

        _polygonBounds = b;

        var groundBounds = GetGroundBounds();

        b = b.Max(groundBounds);

        foreach (var x in Objects)
        {
            b.XMin = Math.Min(b.XMin, x.Position.X);
            b.XMax = Math.Max(b.XMax, x.Position.X);
            b.YMax = Math.Max(b.YMax, x.Position.Y);
            b.YMin = Math.Min(b.YMin, x.Position.Y);
        }

        foreach (var x in GraphicElements)
        {
            b.XMin = Math.Min(b.XMin, x.Position.X);
            b.XMax = Math.Max(b.XMax, x.Position.X + x.Width);
            b.YMax = Math.Max(b.YMax, x.Position.Y);
            b.YMin = Math.Min(b.YMin, x.Position.Y - x.Height);
        }

        Bounds = b;
        GroundBounds = groundBounds;
    }

    private Bounds GetGroundBounds()
    {
        Bounds? groundBounds = null;

        foreach (var polygon in Polygons.Where(p => !p.IsGrass))
        {
            if (groundBounds is { } bp)
            {
                groundBounds = bp.Max(polygon.Bounds);
            }
            else
            {
                groundBounds = polygon.Bounds;
            }
        }

        return groundBounds!.Value;
    }

    public Bounds GroundBounds { get; set; }

    public Bounds Bounds { get; private set; }

    public void UpdateImages(Dictionary<string, DrawableImage> lgrImages)
    {
        var oldElements = GraphicElements.Select(p => p.ToFileData()).ToList();
        GraphicElements = new List<GraphicElement>();
        foreach (var fileItem in oldElements)
        {
            if (fileItem is GraphicElementFileItem.PictureFileItem pItem)
            {
                if (lgrImages.TryGetValue(pItem.PictureName.ToLower(), out var p) && p.Type == ImageType.Picture)
                {
                    GraphicElements.Add(GraphicElement.Pic(p, fileItem.Position, fileItem.Distance, fileItem.Clipping));
                }
                else
                {
                    GraphicElements.Add(GraphicElement.MissingPic(pItem.PictureName, fileItem.Clipping, fileItem.Distance, fileItem.Position));
                }
            }
            else if (fileItem is GraphicElementFileItem.TextureFileItem tItem)
            {
                if (lgrImages.TryGetValue(tItem.TextureName.ToLower(), out var t) && t.Type == ImageType.Texture)
                {
                    if (lgrImages.TryGetValue(tItem.MaskName.ToLower(), out var mask) && mask.Type == ImageType.Mask)
                    {
                        GraphicElements.Add(GraphicElement.Text(fileItem.Clipping, fileItem.Distance, fileItem.Position, t, mask));
                    }
                    else
                    {
                        GraphicElements.Add(GraphicElement.MissingText(tItem.TextureName, tItem.MaskName, fileItem.Clipping, fileItem.Distance, fileItem.Position));
                    }
                }
                else
                {
                    GraphicElements.Add(GraphicElement.MissingText(tItem.TextureName, tItem.MaskName, fileItem.Clipping, fileItem.Distance, fileItem.Position));
                }
            }
        }
    }

    private static void CryptTop10(IList<byte> level, int top10Offset)
    {
        var eax = 21;
        var ecx = 9783;
        const int ebp = 3389;
        for (var j = 0; j <= 687; j++)
        {
            level[top10Offset + j] = (byte)(level[top10Offset + j] ^ BitConverter.GetBytes(eax)[0]);
            ecx += (((short)eax) % ebp) * ebp;
            eax = ecx * 31 + ebp;
        }
    }

    private static IEnumerable<byte> GetByteArrayFromString(string str, int length)
    {
        var bytes = new List<byte>(length);
        bytes.AddRange(Encoding.ASCII.GetBytes(str));
        var count = length - str.Length;
        for (var i = 1; i <= count; i++)
            bytes.Add(0);
        return bytes.ToArray();
    }

    private static List<T> ReadTop10Part<T>(byte[] level, int startIndex, Func<string, string, int, T> factory)
    {
        var list = new List<T>();
        const int sizeSize = 4;
        const int timeSize = 4;
        for (var j = 0; j < level[startIndex]; j++)
            list.Add(factory(FileUtils.ReadNullTerminatedString(level, startIndex + sizeSize + timeSize * Top10Entries + j * Top10NameSize, Top10NameSize),
                FileUtils.ReadNullTerminatedString(level, startIndex + sizeSize + timeSize * Top10Entries + Top10NameSize * Top10Entries + j * Top10NameSize, Top10NameSize),
                BitConverter.ToInt32(level, startIndex + sizeSize + j * timeSize)));
        return list;
    }

    private static void WriteTop10Part(List<byte> levelFile, IReadOnlyList<Top10Entry> entries)
    {
        levelFile.AddRange(BitConverter.GetBytes(entries.Count));
        for (var i = 0; i < Top10Entries; i++)
        {
            levelFile.AddRange(BitConverter.GetBytes(i < entries.Count ? entries[i].Time : 0));
        }
        for (var i = 0; i < Top10Entries; i++)
        {
            levelFile.AddRange(GetByteArrayFromString(i < entries.Count ? entries[i].PlayerA : "", Top10NameSize));
        }
        for (var i = 0; i < Top10Entries; i++)
        {
            levelFile.AddRange(GetByteArrayFromString(i < entries.Count ? entries[i].PlayerB : "", Top10NameSize));
        }
    }

    public static Level FromDimensions(double width, double height)
    {
        return new(
            Polygon.Rectangle(new Vector(), width,
                height),
            new Vector(width / 2,
                height / 2),
            new Vector(width * 3 / 4,
                height / 2));
    }

    public bool EqualsIgnoringTop10(Level other)
    {
        if (Polygons.Count != other.Polygons.Count)
        {
            return false;
        }

        if (Objects.Count != other.Objects.Count)
        {
            return false;
        }

        if (GraphicElements.Count != other.GraphicElements.Count)
        {
            return false;
        }

        if (Title != other.Title)
        {
            return false;
        }

        if (LgrFile != other.LgrFile)
        {
            return false;
        }

        if (GroundTextureName != other.GroundTextureName)
        {
            return false;
        }

        if (SkyTextureName != other.SkyTextureName)
        {
            return false;
        }

        foreach (var (p1, p2) in Polygons.Zip(other.Polygons))
        {
            if (!p1.VerticesEqual(p2))
            {
                return false;
            }
        }

        foreach (var (o1, o2) in Objects.Zip(other.Objects))
        {
            if (!o1.Equals(o2))
            {
                return false;
            }
        }

        foreach (var (o1, o2) in GraphicElements.Zip(other.GraphicElements))
        {
            if (o1.ToFileData() != o2.ToFileData())
            {
                return false;
            }
        }

        return true;
    }
}
