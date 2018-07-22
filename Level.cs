using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Valid;

namespace Elmanager
{
    [Serializable]
    internal class Level
    {
        internal const double GlobalBodyDifferenceFromLeftWheelX = 0.85;
        internal const double GlobalBodyDifferenceFromLeftWheelY = 0.6;
        internal const double HeadDifferenceFromLeftWheelX = 0.7595;
        internal const double HeadDifferenceFromLeftWheelY = -1.67;
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
            "Animal Farm", "Steep Corner", "Zig-Zag", "Bumpy Journey"
            ,
            "Labyrinth Pro", "Fruit in the Den", "Jaws", "Curvaceous"
            ,
            "Haircut", "Double Trouble", "Framework", "Enduro",
            "He He",
            "Freefall", "Sink", "Bowling", "Enigma", "Downhill",
            "What the Heck", "Expert System", "Tricks Abound",
            "Hang Tight", "Hooked", "Apple Harvest", "More Levels"
        };

        internal bool AllPicturesFound = true;
        internal List<Object> Apples;
        internal string GroundTextureName = "ground";
        internal double[] Integrity = new double[4];
        internal bool IsInternal;
        internal string LgrFile = "default";
        internal List<Object> Objects = new List<Object>();
        internal List<Picture> Pictures = new List<Picture>();
        internal List<Polygon> Polygons = new List<Polygon>();
        internal string SkyTextureName = "sky";
        internal LevelTop10 Top10;
        private string _fileName;
        private string _path;
        private int _randomNumber;
        private List<LevelFileTexture> _textureData = new List<LevelFileTexture>();
        private string _title = "New level";
        private double polygonXMin;
        private double polygonYMin;
        private double polygonXMax;
        private double polygonYMax;
        private string identifier;

        internal Level()
        {
            
        }

        internal Level(Polygon startPolygon, Vector startPosition, Vector exitPosition)
        {
            Polygons.Add(startPolygon);
            Objects.Add(Object.StartObject(startPosition));
            Objects.Add(Object.ExitObject(exitPosition));
            UpdateBounds();
        }

        internal void LoadFromPath(string levelPath)
        {
            byte[] level = File.ReadAllBytes(levelPath);
            Path = levelPath;
            IsInternal = IsInternalLevel(_fileName);
            identifier = Encoding.UTF8.GetString(level, 0, 5);
            if (!IsElmaLevel && !IsAcrossLevel && !IsLeb)
            {
                throw new LevelException("Unknown file type. This is neither an Elma level, an Across level nor a LEB file.");
            }

            int sp = 7;
            if (IsAcrossLevel)
                sp -= 2;
            _randomNumber = BitConverter.ToInt32(level, sp);
            sp += 4;
            for (int i = 0; i <= 3; i++)
            {
                Integrity[i] = BitConverter.ToDouble(level, sp);
                sp += 8;
            }
            Title = Utils.ReadNullTerminatedString(level, sp, 60);
            if (IsAcrossLevel)
                sp = 100;
            else
            {
                sp = 94;
                LgrFile = Utils.ReadNullTerminatedString(level, sp, 16);
                sp += 16;
                GroundTextureName = Utils.ReadNullTerminatedString(level, sp, 12).ToLower();
                sp += 10;
                SkyTextureName = Utils.ReadNullTerminatedString(level, sp, 12).ToLower();
                sp += 10;
            }
            int polygonCount = (int) Math.Round(BitConverter.ToDouble(level, sp) - MagicDouble);
            sp += 8;
            int objectCount = -1;
            if (IsLeb)
            {
                objectCount = (int)Math.Round(BitConverter.ToDouble(level, sp) - MagicDouble);
                sp += 8;
            }
            bool isGrassPolygon = false;
            Polygons = new List<Polygon>();
            for (int i = 0; i < polygonCount; i++)
            {
                int numVertice;
                if (IsAcrossLevel)
                {
                    numVertice = level[sp] + 256 * level[sp + 1];
                    sp += 4;
                }
                else
                {
                    numVertice = level[sp + 4] + 256 * level[sp + 5];
                    sp += 8;
                    isGrassPolygon = level[sp - 8] == 1;
                }
                var poly = new Polygon();
                for (int j = 0; j < numVertice; j++)
                {
                    double x = BitConverter.ToDouble(level, sp);
                    double y = BitConverter.ToDouble(level, sp + 8);
                    poly.Add(new Vector(x, y));
                    sp += 16;
                }
                poly.IsGrass = isGrassPolygon;
                Polygons.Add(poly);
            }
            if (!IsLeb)
            {
                objectCount = (int) Math.Round(BitConverter.ToDouble(level, sp) - MagicDouble);
                sp += 8;
            }
            Apples = new List<Object>();
            bool startFound = false;
            for (int i = 0; i < objectCount; i++)
            {
                double x = BitConverter.ToDouble(level, sp);
                double y = BitConverter.ToDouble(level, sp + 8);
                var objectType = (ObjectType) (level[sp + 16]);
                if (objectType == ObjectType.Start)
                {
                    startFound = true;
                }
                AppleTypes appleType = AppleTypes.Normal;
                int animNum = 0;
                if (!IsAcrossLevel)
                {
                    appleType = (AppleTypes) (BitConverter.ToInt32(level, sp + 20));
                    animNum = BitConverter.ToInt32(level, sp + 24);
                    sp += 28;
                }
                else
                    sp += 20;
                var objectToAdd = new Object(new Vector(x, y), objectType, appleType, animNum + 1);
                Objects.Add(objectToAdd);
                if (objectType == ObjectType.Apple)
                    Apples.Add(objectToAdd);
            }
            if (!startFound)
            {
                Objects.Add(new Object(new Vector(0, 0), ObjectType.Start, AppleTypes.Normal));
            }
            if (!IsAcrossLevel)
            {
                int numberOfPicturesPlusTextures = (int) Math.Round(BitConverter.ToDouble(level, sp) - MagicDouble2);
                sp += 8;
                for (int i = 0; i < numberOfPicturesPlusTextures; i++)
                {
                    if (level[sp] == 0)
                        _textureData.Add(new LevelFileTexture(Utils.ReadNullTerminatedString(level, sp + 10, 10),
                            Utils.ReadNullTerminatedString(level, sp + 20, 10),
                            new Vector(BitConverter.ToDouble(level, sp + 30),
                                BitConverter.ToDouble(level, sp + 38)),
                            BitConverter.ToInt32(level, sp + 46),
                            ((ClippingType) (BitConverter.ToInt32(level, sp + 50)))));
                    else
                        _textureData.Add(new LevelFileTexture(Utils.ReadNullTerminatedString(level, sp, 10), null,
                            new Vector(BitConverter.ToDouble(level, sp + 30),
                                BitConverter.ToDouble(level, sp + 38)),
                            BitConverter.ToInt32(level, sp + 46),
                            ((ClippingType) (BitConverter.ToInt32(level, sp + 50)))));
                    sp += 54;
                }
            }
            if (sp != level.Length)
            {
                sp += 4; //Skip end of data magic number
                CryptTop10(level, sp);
                try
                {
                    Top10.SinglePlayer = ReadTop10Part(level, sp);
                    Top10.MultiPlayer = ReadTop10Part(level, sp + 344);
                }
                catch (IndexOutOfRangeException)
                {
                    Top10.SinglePlayer = new List<Top10Entry>();
                    Top10.MultiPlayer = new List<Top10Entry>();
                    throw new LevelException("Top 10 list is corrupted. The list will be cleared if you save the level.");
                }
            }
            UpdateBounds();
        }

        private Level(Level lev)
        {
            Path = lev.Path;
            _fileName = lev._fileName;
            IsInternal = lev.IsInternal;
            identifier = lev.identifier;
            foreach (Object x in lev.Objects)
                Objects.Add(x.Clone());
            foreach (Polygon x in lev.Polygons)
                Polygons.Add(x.Clone());
            XMin = lev.XMin;
            XMax = lev.XMax;
            YMin = lev.YMin;
            YMax = lev.YMax;
            for (int i = 0; i <= 3; i++)
                Integrity[i] = lev.Integrity[i];
            _title = lev._title;
            LgrFile = lev.LgrFile;
            GroundTextureName = lev.GroundTextureName;
            SkyTextureName = lev.SkyTextureName;
            foreach (Picture z in lev.Pictures)
            {
                Pictures.Add(z.Clone());
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                _path = System.IO.Path.GetDirectoryName(_path) + '\\' + _fileName;
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                _fileName = System.IO.Path.GetFileName(_path);
            }
        }

        internal int AppleObjectCount
        {
            get { return Objects.Count(x => x.Type == ObjectType.Apple); }
        }

        internal int ExitObjectCount
        {
            get { return Objects.Count(x => x.Type == ObjectType.Flower); }
        }

        internal string FileNameWithoutExtension
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(_fileName); }
        }

        internal int PolygonCount
        {
            get { return Polygons.Count; }
        }

        internal int GrassPolygonCount
        {
            get { return Polygons.Count(x => x.IsGrass); }
        }

        internal int GrassVertexCount
        {
            get { return Polygons.Where(x => x.IsGrass).Sum(x => x.Count); }
        }

        internal int GroundPolygonCount
        {
            get { return Polygons.Count(x => !x.IsGrass); }
        }

        internal int GroundVertexCount
        {
            get { return Polygons.Where(x => !x.IsGrass).Sum(x => x.Count); }
        }

        internal bool HasTooLargePolygons
        {
            get
            {
                bool foundTooLarge = false;
                foreach (Polygon x in Polygons.Where(x => x.Count > MaximumPolygonVertexCount))
                {
                    x.Mark = PolygonMark.Erroneous;
                    foundTooLarge = true;
                }
                return foundTooLarge;
            }
        }

        internal bool HasTooManyObjects
        {
            get { return Objects.Count > MaximumObjectCount; }
        }

        internal bool HasTooManyPolygons
        {
            get { return Polygons.Count > MaximumPolygonCount; }
        }

        internal bool HasTooManyVertices
        {
            get { return GroundVertexCount > MaximumGroundVertexCount; }
        }

        internal bool HasTopologyErrors
        {
            get
            {
                return HasTooLargePolygons || HasTooManyObjects || HasTooFewObjects || HasTooManyPolygons || HasTooManyVertices || HasTooManyPictures ||
                       WheelLiesOnEdge || HasTexturesOutOfBounds || HeadTouchesGround || TooTall || TooWide || GetIntersectionPoints().Count > 0;
            }
        }

        internal bool HasTooManyPictures { get { return PictureTextureCount > MaximumPictureTextureCount; } }

        internal bool HeadTouchesGround
        {
            get
            {
                Vector head = (from x in Objects
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
                Vector leftWheel = Objects.First(x => x.Type == ObjectType.Start).Position;
                Vector rightWheel = new Vector(leftWheel.X + RightWheelDifferenceFromLeftWheelX, leftWheel.Y);
                const double wheelTolerance = 1e-6;
                return Polygons.Where(poly => !poly.IsGrass).Any(x => x.DistanceFromPoint(leftWheel) < wheelTolerance || x.DistanceFromPoint(rightWheel) < wheelTolerance);
            }
        }

        internal bool HasTexturesOutOfBounds
        {
            get
            {
                double padding = 11.898;
                return Pictures.Where(p => !p.IsPicture).Any(p => 
                p.Position.X < polygonXMin - padding ||
                p.Position.X > polygonXMax + padding ||
                p.Position.Y < polygonYMin - padding ||
                p.Position.Y > polygonYMax + padding);
            }
        }

        internal double Height
        {
            get { return YMax - YMin; }
        }

        internal int KillerObjectCount
        {
            get { return Objects.Count(x => x.Type == ObjectType.Killer); }
        }

        internal int TextureCount
        {
            get { return Pictures.Count(texture => !texture.IsPicture); }
        }

        internal int PictureCount
        {
            get { return Pictures.Count(texture => texture.IsPicture); }
        }

        internal int PictureTextureCount
        {
            get
            {
                return Pictures.Count;
            }
        }

        internal string Title
        {
            get { return _title; }
            set
            {
                if (value.Length > 50)
                    throw (new ArgumentException("The specified level title is too long!"));
                _title = value;
            }
        }

        internal bool TooTall
        {
            get { return polygonYMax - polygonYMin >= MaximumSize; }
        }

        internal bool TooWide
        {
            get { return polygonXMax - polygonXMin >= MaximumSize; }
        }

        internal int VertexCount
        {
            get { return Polygons.Sum(x => x.Count); }
        }

        internal double Width
        {
            get { return XMax - XMin; }
        }

        internal double XMax { get; private set; }

        internal double XMin { get; private set; }

        internal double YMax { get; private set; }

        internal double YMin { get; private set; }

        private double ObjectSum
        {
            get { return Objects.Sum(x => x.Position.X + x.Position.Y + (int) x.Type); }
        }

        private double PictureSum
        {
            get { return _textureData.Sum(x => x.Position.X + x.Position.Y); }
        }

        private double PolygonSum
        {
            get
            {
                double sum = 0;
                foreach (Polygon x in Polygons)
                {
                    for (int i = 0; i < x.Count; i++)
                        sum += x.Vertices[i].X + x.Vertices[i].Y;
                }
                return sum;
            }
        }

        internal bool IsElmaLevel => identifier == "POT14";

        internal bool IsAcrossLevel => identifier == "POT06";

        internal bool IsLeb => identifier == "@@^!@";

        internal bool HasTooFewObjects => Objects.Count < 2;

        internal static string GetPossiblyInternal(string level)
        {
            if (IsInternalLevel(level))
            {
                int index = int.Parse(level.Substring(6, 2));
                return index + " - " + InternalTitles[index - 1];
            }
            return System.IO.Path.GetFileNameWithoutExtension(level);
        }

        internal static bool IsInternalLevel(string levStr)
        {
            return levStr.Length == 12 && Utils.CompareWith(levStr.Substring(0, 6), "QWQUU0");
        }

        internal Level Clone()
        {
            return new Level(this);
        }

        internal void DecomposeGroundPolygons()
        {
            foreach (Polygon x in Polygons)
            {
                x.UpdateDecomposition();
            }
        }

        internal List<Object> GetApplesAndFlowersInsideGround()
        {
            return
                Objects.FindAll(
                    x => (x.Type == ObjectType.Apple || x.Type == ObjectType.Flower) && IsObjectInsideGround(x));
        }

        internal List<Vector> GetIntersectionPoints()
        {
            return Geometry.GetIntersectionPoints(Polygons);
        }

        internal void Import(Level other)
        {
            UpdateBounds();
            other.UpdateBounds();
            
            double xDiff = XMax - other.XMin + 5;
            double yDiff = YMax - other.YMax;

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

        internal bool IsObjectInsideGround(Object o)
        {
            return !Polygons.Any(polygon => polygon.DistanceFromPoint(o.Position) < ElmaRenderer.ObjectRadius) &&
                   IsPointInGround(o.Position);
        }

        internal bool IsPointInGround(Vector p)
        {
            int clipping = Polygons.Count(x => !x.IsGrass && x.AreaHasPoint(p));
            return clipping % 2 == 0;
        }

        internal bool IsSky(Polygon poly)
        {
            int clipping = Polygons.Count(x => !x.Equals(poly) && !x.IsGrass && x.AreaHasPoint(poly.Vertices[0]));
            return clipping % 2 == 0;
        }

        internal void MirrorAll()
        {
            Matrix mirrorMatrix = Matrix.Identity;
            mirrorMatrix.Translate(-(XMax + XMin) / 2, -(YMax + YMin) / 2);
            mirrorMatrix.Scale(-1.0, 1.0);
            mirrorMatrix.Translate((XMax + XMin) / 2, (YMax + YMin) / 2);
            Transform(mirrorMatrix, v => true);
        }

        internal enum MirrorOption
        {
            Horizontal,
            Vertical
        }

        internal void MirrorSelected(MirrorOption mirrorOption)
        {
            double xMin = double.PositiveInfinity;
            double xMax = double.NegativeInfinity;
            double yMin = double.PositiveInfinity;
            double yMax = double.NegativeInfinity;
            foreach (Polygon x in Polygons.Where(p => p.Vertices.Any(v => v.Mark == Geometry.VectorMark.Selected)))
            {
                xMin = Math.Min(xMin, x.Vertices.Where(v => v.Mark == Geometry.VectorMark.Selected).Select(v => v.X).Min());
                xMax = Math.Max(xMax, x.Vertices.Where(v => v.Mark == Geometry.VectorMark.Selected).Select(v => v.X).Max());
                yMax = Math.Max(yMax, x.Vertices.Where(v => v.Mark == Geometry.VectorMark.Selected).Select(v => v.Y).Max());
                yMin = Math.Min(yMin, x.Vertices.Where(v => v.Mark == Geometry.VectorMark.Selected).Select(v => v.Y).Min());
            }
            foreach (Object x in Objects.Where(o => o.Position.Mark == Geometry.VectorMark.Selected))
            {
                xMin = Math.Min(xMin, x.Position.X);
                xMax = Math.Max(xMax, x.Position.X);
                yMax = Math.Max(yMax, x.Position.Y);
                yMin = Math.Min(yMin, x.Position.Y);
            }
            foreach (Picture x in Pictures.Where(p => p.Position.Mark == Geometry.VectorMark.Selected))
            {
                xMin = Math.Min(xMin, x.Position.X);
                xMax = Math.Max(xMax, x.Position.X + x.Width);
                yMax = Math.Max(yMax, x.Position.Y + x.Height);
                yMin = Math.Min(yMin, x.Position.Y);
            }
            Matrix mirrorMatrix = Matrix.Identity;
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
            Transform(mirrorMatrix, v => v.Mark == Geometry.VectorMark.Selected);
        }

        internal void Transform(Matrix matrix, Func<Vector, bool> selector)
        {
            foreach (Polygon x in Polygons)
            {
                foreach (Vector t in x.Vertices.Where(selector))
                    t.Transform(matrix);
                x.UpdateDecomposition();
            }
            foreach (Object t in Objects.Where(o => selector(o.Position)))
            {
                Vector z = t.Position;
                if (t.Type == ObjectType.Start)
                {
                    var fix = new Vector(RightWheelDifferenceFromLeftWheelX / 2, 0);
                    t.Position.SetPosition((z + fix) * matrix - fix);
                }
                else
                    t.Position.Transform(matrix);
            }
            foreach (Picture z in Pictures.Where(p => selector(p.Position)))
            {
                var fix = new Vector(z.Width / 2, z.Height / 2);
                z.Position.SetPosition((z.Position + fix) * matrix - fix);
            }
        }

        internal void Save()
        {
            Save(Path);
        }

        internal void Save(string savePath)
        {
            Path = savePath;
            var levelFile = new List<byte>();
            SaveImages();
            Top10.Clear();
            levelFile.AddRange(Encoding.ASCII.GetBytes("POT14"));
            var rnd = new Random();
            _randomNumber = rnd.Next();
            levelFile.Add(BitConverter.GetBytes(_randomNumber)[0]);
            levelFile.Add(BitConverter.GetBytes(_randomNumber)[1]);
            levelFile.AddRange(BitConverter.GetBytes(_randomNumber));
            double sum = (PictureSum + ObjectSum + PolygonSum) * 3247.764325643;
            Integrity[0] = sum;
            Integrity[1] = rnd.Next(5871) + 11877 - sum;
            if (HasTopologyErrors)
                Integrity[2] = rnd.Next(4982) + 20961 - sum;
            else
                Integrity[2] = rnd.Next(5871) + 11877 - sum;
            Integrity[3] = rnd.Next(6102) + 12112 - sum;
            foreach (double x in Integrity)
                levelFile.AddRange(BitConverter.GetBytes(x));
            levelFile.AddRange(GetByteArrayFromString(Title, 51));
            levelFile.AddRange(GetByteArrayFromString(LgrFile, 16));
            levelFile.AddRange(GetByteArrayFromString(GroundTextureName, 10));
            levelFile.AddRange(GetByteArrayFromString(SkyTextureName, 10));
            levelFile.AddRange(BitConverter.GetBytes(Polygons.Count + MagicDouble));
            foreach (Polygon x in Polygons)
            {
                if (x.IsCounterClockwise ^ IsSky(x))
                    x.ChangeOrientation();
                levelFile.AddRange(BitConverter.GetBytes(x.IsGrass));
                for (int i = 1; i <= 3; i++)
                    levelFile.Add(0);
                levelFile.AddRange(BitConverter.GetBytes(x.Vertices.Count));
                foreach (Vector z in x.Vertices)
                {
                    levelFile.AddRange(BitConverter.GetBytes(z.X));
                    levelFile.AddRange(BitConverter.GetBytes(z.Y));
                }
            }
            levelFile.AddRange(BitConverter.GetBytes(Objects.Count + MagicDouble));
            foreach (Object x in Objects)
            {
                levelFile.AddRange(BitConverter.GetBytes(x.Position.X));
                levelFile.AddRange(BitConverter.GetBytes(x.Position.Y));
                levelFile.AddRange(BitConverter.GetBytes((int) x.Type));
                levelFile.AddRange(BitConverter.GetBytes((int) x.AppleType));
                levelFile.AddRange(BitConverter.GetBytes(x.AnimationNumber - 1));
            }
            levelFile.AddRange(BitConverter.GetBytes(_textureData.Count + MagicDouble2));
            foreach (LevelFileTexture x in _textureData)
            {
                if (x.MaskName == null)
                {
                    levelFile.AddRange(GetByteArrayFromString(x.Name, 10));
                    for (int i = 1; i <= 20; i++)
                        levelFile.Add(0);
                }
                else
                {
                    for (int i = 1; i <= 10; i++)
                        levelFile.Add(0);
                    levelFile.AddRange(GetByteArrayFromString(x.Name, 10));
                    levelFile.AddRange(GetByteArrayFromString(x.MaskName, 10));
                }
                levelFile.AddRange(BitConverter.GetBytes(x.Position.X));
                levelFile.AddRange(BitConverter.GetBytes(x.Position.Y));
                levelFile.AddRange(BitConverter.GetBytes(x.Distance));
                levelFile.AddRange(BitConverter.GetBytes((int) x.Clipping));
            }
            levelFile.AddRange(BitConverter.GetBytes(EndOfDataMagicNumber));
            for (int i = 1; i <= 688; i++)
                levelFile.Add(0);
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
            foreach (Polygon x in Polygons)
            {
                XMin = Math.Min(XMin, x.XMin);
                XMax = Math.Max(XMax, x.XMax);
                YMax = Math.Max(YMax, x.YMax);
                YMin = Math.Min(YMin, x.YMin);
            }
            polygonXMin = XMin;
            polygonYMin = YMin;
            polygonXMax = XMax;
            polygonYMax = YMax;
            foreach (Object x in Objects)
            {
                XMin = Math.Min(XMin, x.Position.X);
                XMax = Math.Max(XMax, x.Position.X);
                YMax = Math.Max(YMax, x.Position.Y);
                YMin = Math.Min(YMin, x.Position.Y);
            }
            foreach (Picture x in Pictures)
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
        internal void UpdateImages(IEnumerable<ElmaRenderer.DrawableImage> lgrImages)
        {
            Pictures = new List<Picture>();
            AllPicturesFound = true;
            foreach (LevelFileTexture fileTexture in _textureData)
            {
                bool pictureFound = false;
                if (fileTexture.MaskName == null)
                {
                    foreach (ElmaRenderer.DrawableImage z in lgrImages)
                    {
                        if (z.Type == Lgr.ImageType.Picture && fileTexture.Name.Equals(z.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Pictures.Add(new Picture(z, fileTexture.Position, fileTexture.Distance, fileTexture.Clipping));
                            pictureFound = true;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (ElmaRenderer.DrawableImage texture in lgrImages)
                    {
                        if (texture.Type == Lgr.ImageType.Texture && fileTexture.Name.Equals(texture.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            pictureFound = true;
                            foreach (ElmaRenderer.DrawableImage mask in lgrImages)
                            {
                                if (mask.Type == Lgr.ImageType.Mask && mask.Name.Equals(fileTexture.MaskName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Pictures.Add(new Picture(fileTexture.Clipping, fileTexture.Distance, fileTexture.Position, texture, mask));
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
            int eax = 21;
            int ecx = 9783;
            const int ebp = 3389;
            for (int j = 0; j <= 687; j++)
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
            int count = length - str.Length;
            for (int i = 1; i <= count; i++)
                bytes.Add(0);
            return bytes.ToArray();
        }

        private static List<Top10Entry> ReadTop10Part(byte[] level, int startIndex)
        {
            var list = new List<Top10Entry>();
            for (int j = 0; j < level[startIndex]; j++)
                list.Add(new Top10Entry(Utils.ReadNullTerminatedString(level, startIndex + 44 + j * 15, 15),
                    Utils.ReadNullTerminatedString(level, startIndex + 194 + j * 15, 15),
                    BitConverter.ToInt32(level, startIndex + 4 + j * 4)));
            return list;
        }

        private void SaveImages()
        {
            _textureData = new List<LevelFileTexture>();
            foreach (Picture x in Pictures)
            {
                _textureData.Add(x.IsPicture
                    ? new LevelFileTexture(x.Name, null, x.Position, x.Distance, x.Clipping)
                    : new LevelFileTexture(x.TextureName, x.Name, x.Position, x.Distance, x.Clipping));
            }
        }

        internal enum AppleTypes
        {
            Normal = 0,
            GravityUp = 1,
            GravityDown = 2,
            GravityLeft = 3,
            GravityRight = 4
        }

        internal enum ClippingType
        {
            Unclipped = 0,
            Ground = 1,
            Sky = 2
        }

        [Serializable]
        private class LevelFileTexture
        {
            internal ClippingType Clipping;
            internal int Distance;
            internal string MaskName;
            internal string Name;
            internal Vector Position;

            internal LevelFileTexture(string name, string maskName, Vector position, int distance, ClippingType clipping)
            {
                Name = name;
                MaskName = maskName;
                Position = position;
                Distance = distance;
                Clipping = clipping;
            }
        }

        [Serializable]
        internal struct LevelTop10
        {
            internal List<Top10Entry> MultiPlayer;
            internal List<Top10Entry> SinglePlayer;

            internal bool IsEmpty
            {
                get
                {
                    bool singlePlayerEmpty = SinglePlayer == null || SinglePlayer.Count == 0;
                    bool multiPlayerEmpty = MultiPlayer == null || MultiPlayer.Count == 0;
                    return singlePlayerEmpty && multiPlayerEmpty;
                }
            }

            internal void Clear()
            {
                if (SinglePlayer != null)
                    SinglePlayer.Clear();
                if (MultiPlayer != null)
                    MultiPlayer.Clear();
            }

            internal double GetMultiPlayerAverage()
            {
                if (MultiPlayer == null)
                    return 0.0;
                double avg = MultiPlayer.Sum(x => x.Time / 100.0);
                return MultiPlayer.Count > 0 ? avg / MultiPlayer.Count : 0.0;
            }

            internal string GetMultiPlayerString(int index)
            {
                if (MultiPlayer == null || MultiPlayer.Count <= index)
                    return "None";
                string result = MultiPlayer[index].PlayerA + ", " + MultiPlayer[index].PlayerB;
                result = result.PadRight(21);
                result += (MultiPlayer[index].Time / 100.0).ToTimeString(2);
                return result;
            }

            internal double GetSinglePlayerAverage()
            {
                if (SinglePlayer == null)
                    return 0.0;
                double avg = SinglePlayer.Sum(x => x.Time / 100.0);
                return SinglePlayer.Count > 0 ? avg / SinglePlayer.Count : 0.0;
            }

            internal string GetSinglePlayerString(int index)
            {
                if (SinglePlayer == null || SinglePlayer.Count <= index)
                    return "None";
                string result = SinglePlayer[index].PlayerA.PadRight(12);
                result += (SinglePlayer[index].Time / 100.0).ToTimeString(2);
                return result;
            }
        }

        [Serializable]
        internal class Object
        {
            internal int AnimationNumber;
            internal AppleTypes AppleType;
            internal Vector Position;
            internal ObjectType Type;

            internal Object(Vector position, ObjectType type, AppleTypes appleType, int animNum = 1)
            {
                Position = position;
                Type = type;
                AppleType = appleType;
                AnimationNumber = Math.Min(Math.Max(animNum, 1), 9);
            }

            private Object(Object o)
            {
                AnimationNumber = o.AnimationNumber;
                AppleType = o.AppleType;
                Position = o.Position.Clone();
                Type = o.Type;
            }

            internal static Object ExitObject(Vector exitPosition)
            {
                return new Object(exitPosition, ObjectType.Flower, AppleTypes.Normal);
            }

            internal static Object StartObject(Vector startPosition)
            {
                return new Object(startPosition, ObjectType.Start, AppleTypes.Normal);
            }

            internal Object Clone()
            {
                return new Object(this);
            }
        }

        internal enum ObjectType
        {
            Flower = 1,
            Apple = 2,
            Killer = 3,
            Start = 4
        }

        internal class Picture
        {
            internal ClippingType Clipping;
            internal int Distance;
            internal double Height;
            internal int Id;
            internal string Name;
            internal Vector Position;
            internal double TextureHeight;
            internal int TextureId;
            internal string TextureName;
            internal double TextureWidth;
            internal double Width;

            internal Picture(ClippingType clipping, int distance, Vector position, ElmaRenderer.DrawableImage texture,
                ElmaRenderer.DrawableImage mask)
            {
                SetTexture(clipping, distance, position, texture, mask);
            }

            internal Picture(ElmaRenderer.DrawableImage pictureImage, Vector position, int distance,
                ClippingType clipping)
            {
                SetPicture(pictureImage, position, distance, clipping);
            }

            private Picture(Picture T)
            {
                Clipping = T.Clipping;
                Distance = T.Distance;
                Height = T.Height;
                Name = T.Name;
                Position = T.Position.Clone();
                Id = T.Id;
                Width = T.Width;
                TextureId = T.TextureId;
                TextureName = T.TextureName;
                TextureWidth = T.TextureWidth;
                TextureHeight = T.TextureHeight;
            }

            internal double AspectRatio
            {
                get { return TextureWidth / TextureHeight; }
            }

            internal bool IsPicture
            {
                get { return TextureName == null; }
            }

            internal Picture Clone()
            {
                return new Picture(this);
            }

            internal void SetPicture(ElmaRenderer.DrawableImage pictureImage, Vector position, int distance,
                ClippingType clipping)
            {
                Position = position;
                Id = pictureImage.TextureIdentifier;
                Width = pictureImage.Width;
                Height = pictureImage.Height;
                Clipping = clipping;
                Distance = distance;
                Name = pictureImage.Name;
                TextureId = 0;
                TextureName = null;
                TextureWidth = 0;
                TextureHeight = 0;
            }

            internal void SetTexture(ClippingType clipping, int distance, Vector position,
                ElmaRenderer.DrawableImage texture, ElmaRenderer.DrawableImage mask)
            {
                Clipping = clipping;
                Distance = distance;
                Height = mask.Height;
                Name = mask.Name;
                Position = position;
                Id = mask.TextureIdentifier;
                Width = mask.Width;
                TextureId = texture.TextureIdentifier;
                TextureName = texture.Name;
                TextureWidth = texture.Width;
                TextureHeight = texture.Height;
            }
        }

        [Serializable]
        internal struct Top10Entry
        {
            internal string PlayerA;
            internal string PlayerB;
            internal int Time;

            internal Top10Entry(string playerA, string playerB, int time)
            {
                PlayerA = playerA;
                PlayerB = playerB;
                Time = time;
            }
        }

        internal static Level FromDimensions(double width, double height)
        {
            return new Level(
                Polygon.Rectangle(new Vector(), width,
                    height),
                new Vector(width / 2,
                    height / 2),
                new Vector(width * 3 / 4,
                    height / 2));
        }
    }
}