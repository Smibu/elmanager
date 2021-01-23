using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Elmanager
{
    internal class Level : ElmaObject
    {
        internal const double GlobalBodyDifferenceFromLeftWheelX = 0.85;
        internal const double GlobalBodyDifferenceFromLeftWheelY = 0.6;
        internal const double HeadDifferenceFromLeftWheelX = 0.7595;
        internal const double HeadDifferenceFromLeftWheelY = 1.67;
        internal const int MaximumObjectCount = 252;
        internal const int MaximumPolygonCount = 1200;
        internal const int MaximumPolygonVertexCount = 1000;
        internal const double MaximumSize = 188.0;
        internal const int MaximumGroundVertexCount = 5130;
        internal const int MaximumPictureTextureCount = 5000;
        internal const double RightWheelDifferenceFromLeftWheelX = 1.698;
        private const int EndOfDataMagicNumber = 0x67103A;
        private const int EndOfFileMagicNumber = 0x845D52;
        private const double MagicDouble = 0.4643643;
        private const double MagicDouble2 = 0.2345672;

        private const int Top10NameSize = 15;
        private const int Top10Entries = 10;

        internal static readonly string[] InternalTitles =
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

        internal bool AllPicturesFound = true;
        internal string GroundTextureName = "ground";
        private double[] Integrity = new double[4];
        private bool IsInternal;

        internal List<LevObject> Objects = new();
        internal List<Picture> Pictures = new();
        internal List<Polygon> Polygons = new();
        internal string SkyTextureName = "sky";
        internal readonly LevelTop10 Top10 = new();
        public int Identifier { get; private set; }
        private List<LevelFileTexture> _textureData = new();

        [Description("LGR")]
        public string LgrFile = "default";
        private string _title = "New level";
        private double _polygonXMin;
        private double _polygonYMin;
        private double _polygonXMax;
        private double _polygonYMax;
        public string LevStartMagic { get; private set; }

        [Description("Replays")]
        public int Replays;

        internal Level()
        {
        }

        private Level(Polygon startPolygon, Vector startPosition, Vector exitPosition)
        {
            Polygons.Add(startPolygon);
            Objects.Add(LevObject.StartObject(startPosition));
            Objects.Add(LevObject.ExitObject(exitPosition));
            UpdateBounds();
        }

        internal static Level FromPath(string levelPath)
        {
            using var stream = File.OpenRead(levelPath);
            var lev = FromStream(stream);
            lev.Size = (int)new FileInfo(levelPath).Length;
            lev.Path = levelPath;
            lev.IsInternal = IsInternalLevel(lev.FileName);
            lev.DateModified = File.GetLastWriteTime(levelPath);
            return lev;
        }

        internal static Level FromStream(Stream data)
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
                Integrity[i] = lev.ReadDouble();
            }

            Title = lev.ReadNullTerminatedString(IsAcrossLevel ? 59 : 51);
            if (!IsAcrossLevel)
            {
                LgrFile = lev.ReadNullTerminatedString(16);
                GroundTextureName = lev.ReadNullTerminatedString(10).ToLower();
                SkyTextureName = lev.ReadNullTerminatedString(10).ToLower();
            }

            var polygonCount = (int) Math.Round(lev.ReadDouble() - MagicDouble);
            var objectCount = -1;
            if (IsLeb)
            {
                objectCount = (int) Math.Round(lev.ReadDouble() - MagicDouble);
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
                objectCount = (int) Math.Round(lev.ReadDouble() - MagicDouble);
            }

            var startFound = false;
            for (var i = 0; i < objectCount; i++)
            {
                var x = lev.ReadDouble();
                var y = -lev.ReadDouble();
                var objectType = (ObjectType) lev.ReadInt32();
                if (objectType == ObjectType.Start)
                {
                    startFound = true;
                }

                var appleType = AppleType.Normal;
                var animNum = 0;
                if (!IsAcrossLevel)
                {
                    appleType = (AppleType) lev.ReadInt32();
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
                var numberOfPicturesPlusTextures = (int) Math.Round(lev.ReadDouble() - MagicDouble2);
                for (var i = 0; i < numberOfPicturesPlusTextures; i++)
                {
                    var pictureName = lev.ReadNullTerminatedString(10);
                    var textureName = lev.ReadNullTerminatedString(10);
                    var maskName = lev.ReadNullTerminatedString(10);
                    var x = lev.ReadDouble();
                    var y = -lev.ReadDouble();
                    var distance = lev.ReadInt32();
                    var clipping = (ClippingType) lev.ReadInt32();
                    if (pictureName == "")
                        _textureData.Add(new LevelFileTexture(textureName,
                            maskName, new Vector(x, y),
                            distance,
                            clipping));
                    else
                        _textureData.Add(new LevelFileTexture(pictureName,
                            null, new Vector(x, y),
                            distance,
                            clipping));
                }
            }

            HandleLevEndRead(lev);
            UpdateBounds();
        }

        private void HandleLevEndRead(BinaryReader lev)
        {
            int endOfData;
            try
            {
                endOfData = lev.ReadInt32();
            }
            catch (EndOfStreamException)
            {
                return;
            }

            if (endOfData != EndOfDataMagicNumber)
            {
                throw new BadFileException($"Wrong end of data marker: {endOfData}");
            }

            try
            {
                var top10data = lev.ReadBytes(688);
                CryptTop10(top10data, 0);
                Top10.SinglePlayer = ReadTop10Part(top10data, 0, (a, b, t) => new Top10EntrySingle(a, b, t));
                Top10.MultiPlayer = ReadTop10Part(top10data, 0 + 344, (a, b, t) => new Top10EntryMulti(a, b, t));
            }
            catch (IndexOutOfRangeException)
            {
                throw new BadFileException(
                    "Top 10 list is corrupted. The list will be cleared if you save the level.");
            }
        }

        private Level(Level lev)
        {
            Path = lev.Path;
            IsInternal = lev.IsInternal;
            LevStartMagic = lev.LevStartMagic;
            foreach (var x in lev.Objects)
                Objects.Add(x.Clone());
            foreach (var x in lev.Polygons)
                Polygons.Add(x.Clone());
            XMin = lev.XMin;
            XMax = lev.XMax;
            YMin = lev.YMin;
            YMax = lev.YMax;
            for (var i = 0; i <= 3; i++)
                Integrity[i] = lev.Integrity[i];
            _title = lev._title;
            LgrFile = lev.LgrFile;
            GroundTextureName = lev.GroundTextureName;
            SkyTextureName = lev.SkyTextureName;
            foreach (var z in lev.Pictures)
            {
                Pictures.Add(z.Clone());
            }
        }

        [Description("Single 1st")]
        public ElmaTime? BestSingleTime => Top10.SinglePlayer.Count > 0 ? Top10.SinglePlayer[0].TimeInSecs : (ElmaTime?) null;

        [Description("Multi 1st")]
        public ElmaTime? BestMultiTime => Top10.MultiPlayer.Count > 0 ? Top10.MultiPlayer[0].TimeInSecs : (ElmaTime?)null;

        [Description("Times")]
        public int SingleTimesCount => Top10.SinglePlayer.Count;

        [Description("Times (m)")]
        public int MultiTimesCount => Top10.MultiPlayer.Count;

        [Description("Apples")] public int AppleObjectCount => Objects.Count(x => x.Type == ObjectType.Apple);

        [Description("Grav.")]
        public int GravApples => Objects.Count(x => x.Type == ObjectType.Apple && x.AppleType != AppleType.Normal);

        [Description("Flowers")] public int ExitObjectCount => Objects.Count(x => x.Type == ObjectType.Flower);

        [Description("Killers")]
        public int KillerObjectCount => Objects.Count(x => x.Type == ObjectType.Killer);

        [Description("Polys")] public int PolygonCount => Polygons.Count;

        [Description("Grass ps")] public int GrassPolygonCount => Polygons.Count(x => x.IsGrass);

        [Description("Grass vs")] public int GrassVertexCount =>
            Polygons.Where(x => x.IsGrass).Sum(x => x.Count);

        [Description("Ground ps")] public int GroundPolygonCount => Polygons.Count(x => !x.IsGrass);

        [Description("Ground vs")]
        public int GroundVertexCount => Polygons.Where(x => !x.IsGrass).Sum(x => x.Count);

        internal bool HasTooLargePolygons
        {
            get
            {
                var foundTooLarge = false;
                foreach (var x in Polygons.Where(x => x.Count > MaximumPolygonVertexCount))
                {
                    x.Mark = PolygonMark.Erroneous;
                    foundTooLarge = true;
                }

                return foundTooLarge;
            }
        }

        internal List<Vector> GetTooShortEdges()
        {
            var result = new List<Vector>();
            foreach (var p in Polygons)
            {
                for (var i = 0; i < p.Count; i++)
                {
                    if ((p[i] - p[i + 1]).Length < 0.00000001)
                    {
                        result.Add(p[i]);
                    }
                }
            }

            return result;
        }

        internal int GetGravityAppleCount(AppleType t)
        {
            return Objects.Count(o => o.Type == ObjectType.Apple && o.AppleType == t);
        }

        internal bool HasTooManyObjects => Objects.Count > MaximumObjectCount;

        internal bool HasTooManyPolygons => Polygons.Count > MaximumPolygonCount;

        internal bool HasTooManyVertices => GroundVertexCount > MaximumGroundVertexCount;

        internal bool HasTopologyErrors => HasTooLargePolygons || HasTooManyObjects || HasTooFewObjects ||
                                           HasTooManyPolygons || HasTooManyVertices || HasTooManyPictures ||
                                           WheelLiesOnEdge || HasTexturesOutOfBounds || HeadTouchesGround || TooTall ||
                                           TooWide || GetIntersectionPoints().Count > 0 || GetTooShortEdges().Count > 0;

        internal bool HasTooManyPictures => PictureTextureCount > MaximumPictureTextureCount;

        internal bool HeadTouchesGround
        {
            get
            {
                var head = (from x in Objects
                    where x.Type == ObjectType.Start
                    select
                        new Vector(x.Position.X + HeadDifferenceFromLeftWheelX,
                            x.Position.Y + HeadDifferenceFromLeftWheelY)).FirstOrDefault();
                return Polygons.Where(poly => !poly.IsGrass).Any(x => x.DistanceFromPoint(head) < Constants.HeadRadius);
            }
        }

        internal bool WheelLiesOnEdge
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

        internal bool HasTexturesOutOfBounds
        {
            get
            {
                var padding = 11.898;
                return Pictures.Where(p => !p.IsPicture).Any(p =>
                    p.Position.X < _polygonXMin - padding ||
                    p.Position.X > _polygonXMax + padding ||
                    p.Position.Y < _polygonYMin - padding ||
                    p.Position.Y > _polygonYMax + padding);
            }
        }

        [Description("Width")]
        public double Width => XMax - XMin;

        [Description("Height")]
        public double Height => YMax - YMin;

        public int TextureCount => Pictures.Count(texture => !texture.IsPicture);

        public int PictureCount => Pictures.Count(texture => texture.IsPicture);

        [Description("Textures")]
        public int LevelFileTextureCount => _textureData.Count(texture => !texture.IsPicture);

        [Description("Pictures")]
        public int LevelFilePictureCount => _textureData.Count(texture => texture.IsPicture);

        internal int PictureTextureCount => Pictures.Count;

        [Description("Title")]
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

        internal bool TooTall => _polygonYMax - _polygonYMin >= MaximumSize;

        internal bool TooWide => _polygonXMax - _polygonXMin >= MaximumSize;

        internal int VertexCount => Polygons.Sum(x => x.Count);

        internal double XMax { get; private set; }

        internal double XMin { get; private set; }

        internal double YMax { get; private set; }

        internal double YMin { get; private set; }

        private double ObjectSum => Objects.Sum(x => x.Position.X - x.Position.Y + (int) x.Type);

        private double PictureSum => _textureData.Sum(x => x.Position.X - x.Position.Y);

        private double PolygonSum => Polygons.Sum(x => x.Vertices.Sum(v => v.X - v.Y));

        internal bool IsElmaLevel => LevStartMagic == "POT14";

        internal bool IsAcrossLevel => LevStartMagic == "POT06";

        internal bool IsLeb => LevStartMagic == "@@^!@";

        internal bool HasTooFewObjects => Objects.Count < 2;

        internal static string GetPossiblyInternal(string level)
        {
            if (IsInternalLevel(level))
            {
                var index = int.Parse(level.Substring(6, 2));
                return index + " - " + InternalTitles[index - 1];
            }

            return System.IO.Path.GetFileNameWithoutExtension(level);
        }

        internal static bool IsInternalLevel(string levStr) =>
            levStr.Length == 12 && levStr.Substring(0, 6).CompareWith("QWQUU0");

        internal Level Clone() => new(this);

        internal void DecomposeGroundPolygons()
        {
            foreach (var x in Polygons)
            {
                x.UpdateDecomposition();
            }
        }

        internal List<LevObject> GetApplesAndFlowersInsideGround()
        {
            return
                Objects.FindAll(
                    x => (x.Type == ObjectType.Apple || x.Type == ObjectType.Flower) && IsObjectInsideGround(x));
        }

        internal List<Vector> GetIntersectionPoints() => GeometryUtils.GetIntersectionPoints(Polygons);

        internal void Import(Level other)
        {
            UpdateBounds();
            other.UpdateBounds();

            var xDiff = XMax - other.XMin + 5;
            var yDiff = YMax - other.YMax;

            var diffV = new Vector(xDiff, yDiff);

            other.Polygons.ForEach(poly => poly.Move(diffV));
            other.Objects.ForEach(obj => obj.Position += diffV);
            other.Objects.RemoveAll(obj => obj.Type == ObjectType.Start);
            other.Pictures.ForEach(pic => pic.Position += diffV);
            other.DecomposeGroundPolygons();

            Polygons.AddRange(other.Polygons);
            Objects.AddRange(other.Objects);
            Pictures.AddRange(other.Pictures);

            SaveImages();

            UpdateBounds();
        }

        internal bool IsObjectInsideGround(LevObject o)
        {
            return !Polygons.Any(polygon => polygon.DistanceFromPoint(o.Position) < ElmaRenderer.ObjectRadius) &&
                   IsPointInGround(o.Position);
        }

        internal bool IsPointInGround(Vector p)
        {
            var clipping = Polygons.Count(x => !x.IsGrass && x.AreaHasPoint(p));
            return clipping % 2 == 0;
        }

        internal bool IsSky(Polygon poly)
        {
            var clipping = Polygons.Count(x => !x.Equals(poly) && !x.IsGrass && x.AreaHasPoint(poly.Vertices[0]));
            return clipping % 2 == 0;
        }

        internal void MirrorAll()
        {
            var mirrorMatrix = Matrix.Identity;
            mirrorMatrix.Translate(-(XMax + XMin) / 2, -(YMax + YMin) / 2);
            mirrorMatrix.Scale(-1.0, 1.0);
            mirrorMatrix.Translate((XMax + XMin) / 2, (YMax + YMin) / 2);
            Transform(mirrorMatrix, v => true);
        }

        internal void MirrorSelected(MirrorOption mirrorOption)
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

            foreach (var x in Pictures.Where(p => p.Position.Mark == VectorMark.Selected))
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

        internal void Transform(Matrix matrix, Func<Vector, bool> selector)
        {
            foreach (var x in Polygons)
            {
                foreach (var t in x.Vertices.Where(selector))
                    t.Transform(matrix);
                x.UpdateDecomposition();
            }

            foreach (var t in Objects.Where(o => selector(o.Position)))
            {
                var z = t.Position;
                if (t.Type == ObjectType.Start)
                {
                    var fix = new Vector(RightWheelDifferenceFromLeftWheelX / 2, 0);
                    t.Position.SetPosition((z + fix) * matrix - fix);
                }
                else
                    t.Position.Transform(matrix);
            }

            foreach (var z in Pictures.Where(p => selector(p.Position)))
            {
                var fix = new Vector(z.Width / 2, -z.Height / 2);
                z.Position.SetPosition((z.Position + fix) * matrix - fix);
            }
        }

        internal void Save(string savePath, bool saveAsFresh = true)
        {
            Path = savePath;
            var levelFile = new List<byte>();
            SaveImages();
            if (saveAsFresh)
            {
                Top10.Clear();
            }

            levelFile.AddRange(Encoding.ASCII.GetBytes("POT14"));
            var rnd = new Random();
            if (saveAsFresh)
            {
                Identifier = rnd.Next();
            }
            levelFile.Add(BitConverter.GetBytes(Identifier)[0]);
            levelFile.Add(BitConverter.GetBytes(Identifier)[1]);
            levelFile.AddRange(BitConverter.GetBytes(Identifier));
            var sum = (PictureSum + ObjectSum + PolygonSum) * 3247.764325643;
            Integrity[0] = sum;
            Integrity[1] = rnd.Next(5871) + 11877 - sum;
            if (HasTopologyErrors)
                Integrity[2] = rnd.Next(4982) + 20961 - sum;
            else
                Integrity[2] = rnd.Next(5871) + 11877 - sum;
            Integrity[3] = rnd.Next(6102) + 12112 - sum;
            foreach (var x in Integrity)
                levelFile.AddRange(BitConverter.GetBytes(x));
            levelFile.AddRange(GetByteArrayFromString(Title, 51));
            levelFile.AddRange(GetByteArrayFromString(LgrFile, 16));
            levelFile.AddRange(GetByteArrayFromString(GroundTextureName, 10));
            levelFile.AddRange(GetByteArrayFromString(SkyTextureName, 10));
            levelFile.AddRange(BitConverter.GetBytes(Polygons.Count + MagicDouble));
            foreach (var x in Polygons)
            {
                if (!(x.IsCounterClockwise ^ IsSky(x)))
                    x.ChangeOrientation();
                levelFile.AddRange(BitConverter.GetBytes(x.IsGrass));
                for (var i = 1; i <= 3; i++)
                    levelFile.Add(0);
                levelFile.AddRange(BitConverter.GetBytes(x.Vertices.Count));
                foreach (var z in x.Vertices)
                {
                    levelFile.AddRange(BitConverter.GetBytes(z.X));
                    levelFile.AddRange(BitConverter.GetBytes(-z.Y));
                }
            }

            levelFile.AddRange(BitConverter.GetBytes(Objects.Count + MagicDouble));
            foreach (var x in Objects)
            {
                levelFile.AddRange(BitConverter.GetBytes(x.Position.X));
                levelFile.AddRange(BitConverter.GetBytes(-x.Position.Y));
                levelFile.AddRange(BitConverter.GetBytes((int) x.Type));
                levelFile.AddRange(BitConverter.GetBytes((int) x.AppleType));
                levelFile.AddRange(BitConverter.GetBytes(x.AnimationNumber - 1));
            }

            levelFile.AddRange(BitConverter.GetBytes(_textureData.Count + MagicDouble2));
            foreach (var x in _textureData)
            {
                if (x.MaskName == null)
                {
                    levelFile.AddRange(GetByteArrayFromString(x.Name, 10));
                    for (var i = 1; i <= 20; i++)
                        levelFile.Add(0);
                }
                else
                {
                    for (var i = 1; i <= 10; i++)
                        levelFile.Add(0);
                    levelFile.AddRange(GetByteArrayFromString(x.Name, 10));
                    levelFile.AddRange(GetByteArrayFromString(x.MaskName, 10));
                }

                levelFile.AddRange(BitConverter.GetBytes(x.Position.X));
                levelFile.AddRange(BitConverter.GetBytes(-x.Position.Y));
                levelFile.AddRange(BitConverter.GetBytes(x.Distance));
                levelFile.AddRange(BitConverter.GetBytes((int) x.Clipping));
            }

            levelFile.AddRange(BitConverter.GetBytes(EndOfDataMagicNumber));
            WriteTop10Part(levelFile, Top10.SinglePlayer);
            WriteTop10Part(levelFile, Top10.MultiPlayer);
            CryptTop10(levelFile, levelFile.Count - 688);
            levelFile.AddRange(BitConverter.GetBytes(EndOfFileMagicNumber));
            File.WriteAllBytes(savePath, levelFile.ToArray());
        }

        internal void SortPictures()
        {
            Pictures = Pictures.OrderBy(p => p.Clipping == ClippingType.Unclipped ? 1 : 0).ToList();
        }

        internal void UpdateBounds()
        {
            var first = Polygons.First();
            XMin = first.XMin;
            XMax = first.XMax;
            YMin = first.YMin;
            YMax = first.YMax;
            foreach (var x in Polygons)
            {
                XMin = Math.Min(XMin, x.XMin);
                XMax = Math.Max(XMax, x.XMax);
                YMax = Math.Max(YMax, x.YMax);
                YMin = Math.Min(YMin, x.YMin);
            }

            _polygonXMin = XMin;
            _polygonYMin = YMin;
            _polygonXMax = XMax;
            _polygonYMax = YMax;
            foreach (var x in Objects)
            {
                XMin = Math.Min(XMin, x.Position.X);
                XMax = Math.Max(XMax, x.Position.X);
                YMax = Math.Max(YMax, x.Position.Y);
                YMin = Math.Min(YMin, x.Position.Y);
            }

            foreach (var x in Pictures)
            {
                XMin = Math.Min(XMin, x.Position.X);
                XMax = Math.Max(XMax, x.Position.X + x.Width);
                YMax = Math.Max(YMax, x.Position.Y + x.Height);
                YMin = Math.Min(YMin, x.Position.Y);
            }
        }

        /// <summary>
        ///     This method should only be called once (when loading level).
        /// </summary>
        /// <param name="lgrImages"></param>
        internal void UpdateImages(List<DrawableImage> lgrImages)
        {
            Pictures = new List<Picture>();
            AllPicturesFound = true;
            foreach (var fileTexture in _textureData)
            {
                var pictureFound = false;
                if (fileTexture.MaskName == null)
                {
                    foreach (var z in lgrImages)
                    {
                        if (z.Type == Lgr.ImageType.Picture &&
                            fileTexture.Name.Equals(z.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Pictures.Add(new Picture(z, fileTexture.Position, fileTexture.Distance,
                                fileTexture.Clipping));
                            pictureFound = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var texture in lgrImages)
                    {
                        if (texture.Type == Lgr.ImageType.Texture &&
                            fileTexture.Name.Equals(texture.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            pictureFound = true;
                            foreach (var mask in lgrImages)
                            {
                                if (mask.Type == Lgr.ImageType.Mask && mask.Name.Equals(fileTexture.MaskName,
                                        StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Pictures.Add(new Picture(fileTexture.Clipping, fileTexture.Distance,
                                        fileTexture.Position, texture, mask));
                                    break;
                                }
                            }

                            break;
                        }
                    }
                }

                if (!pictureFound)
                    AllPicturesFound = false;
            }

            SortPictures();
        }

        private static void CryptTop10(IList<byte> level, int top10Offset)
        {
            var eax = 21;
            var ecx = 9783;
            const int ebp = 3389;
            for (var j = 0; j <= 687; j++)
            {
                level[top10Offset + j] = (byte) (level[top10Offset + j] ^ BitConverter.GetBytes(eax)[0]);
                ecx += (((short) eax) % ebp) * ebp;
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
                list.Add(factory(Utils.ReadNullTerminatedString(level, startIndex + sizeSize + timeSize * Top10Entries + j * Top10NameSize, Top10NameSize),
                    Utils.ReadNullTerminatedString(level, startIndex + sizeSize + timeSize * Top10Entries + Top10NameSize * Top10Entries + j * Top10NameSize, Top10NameSize),
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

        private void SaveImages()
        {
            _textureData = new List<LevelFileTexture>();
            foreach (var x in Pictures)
            {
                _textureData.Add(x.IsPicture
                    ? new LevelFileTexture(x.Name, null, x.Position, x.Distance, x.Clipping)
                    : new LevelFileTexture(x.TextureName, x.Name, x.Position, x.Distance, x.Clipping));
            }
        }

        internal static Level FromDimensions(double width, double height)
        {
            return new(
                Polygon.Rectangle(new Vector(), width,
                    height),
                new Vector(width / 2,
                    height / 2),
                new Vector(width * 3 / 4,
                    height / 2));
        }
    }
}