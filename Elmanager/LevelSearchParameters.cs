using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Elmanager
{
    internal class LevelSearchParameters : SearchParameters
    {
        private const int MaxCount = 10000;
        internal Range<int> GroundPolygons = new Range<int>(0, MaxCount);
        internal Range<int> GrassPolygons = new Range<int>(0, MaxCount);
        internal Range<int> GroundVertices = new Range<int>(0, MaxCount);
        internal Range<int> GrassVertices = new Range<int>(0, MaxCount);
        internal Range<int> SingleTop10Times = new Range<int>(0, MaxCount);
        internal Range<int> MultiTop10Times = new Range<int>(0, MaxCount);
        internal Range<int> Killers = new Range<int>(0, MaxCount);
        internal Range<int> Flowers = new Range<int>(0, MaxCount);
        internal Range<int> Pictures = new Range<int>(0, MaxCount);
        internal Range<int> Textures = new Range<int>(0, MaxCount);
        internal Range<int> Apples = new Range<int>(0, MaxCount);

        internal Dictionary<AppleType, Range<int>> GravApples = new Dictionary<AppleType, Range<int>>
        {
            {AppleType.Normal, new Range<int>(0, MaxCount)},
            {AppleType.GravityUp, new Range<int>(0, MaxCount)},
            {AppleType.GravityDown, new Range<int>(0, MaxCount)},
            {AppleType.GravityLeft, new Range<int>(0, MaxCount)},
            {AppleType.GravityRight, new Range<int>(0, MaxCount)},
        };

        internal Regex Title = new Regex("");
        internal Regex Lgr = new Regex("");
        internal Regex GroundTexture = new Regex("");
        internal Regex SkyTexture = new Regex("");
        internal Regex SinglePlayerNick = new Regex("");
        internal Regex MultiPlayerNick = new Regex("");

        internal Range<double> SinglePlayerBestTime = new Range<double>(0, double.MaxValue);
        internal Range<double> MultiPlayerBestTime = new Range<double>(0, double.MaxValue);

        internal Range<int> Replays = new Range<int>(0, MaxCount);

        public bool Matches(Level lev)
        {
            return Title.IsMatch(lev.Title) && Lgr.IsMatch(lev.LgrFile) &&
                   GroundTexture.IsMatch(lev.GroundTextureName) && SkyTexture.IsMatch(lev.SkyTextureName) &&
                   (lev.Top10.SinglePlayer.Count == 0 ||
                    lev.Top10.SinglePlayer.Any(e => SinglePlayerNick.IsMatch(e.PlayerA))) &&
                   (lev.Top10.MultiPlayer.Count == 0 || lev.Top10.MultiPlayer.Any(e =>
                        MultiPlayerNick.IsMatch(e.PlayerA) || MultiPlayerNick.IsMatch(e.PlayerB))) &&
                   SingleTop10Times.Accepts(lev.Top10.SinglePlayer.Count) &&
                   MultiTop10Times.Accepts(lev.Top10.MultiPlayer.Count) &&
                   (lev.Top10.SinglePlayer.Count == 0 ||
                    SinglePlayerBestTime.Accepts(lev.Top10.SinglePlayer[0].TimeInSecs)) &&
                   (lev.Top10.MultiPlayer.Count == 0 ||
                    MultiPlayerBestTime.Accepts(lev.Top10.MultiPlayer[0].TimeInSecs)) &&
                   GrassPolygons.Accepts(lev.GrassPolygonCount) &&
                   GroundPolygons.Accepts(lev.GroundPolygonCount) &&
                   GroundVertices.Accepts(lev.GroundVertexCount) &&
                   GrassVertices.Accepts(lev.GrassVertexCount) &&
                   Killers.Accepts(lev.KillerObjectCount) &&
                   Flowers.Accepts(lev.ExitObjectCount) &&
                   Pictures.Accepts(lev.PictureCount) &&
                   Textures.Accepts(lev.TextureCount) &&
                   Apples.Accepts(lev.AppleObjectCount) &&
                   GravApples.All(kv => kv.Value.Accepts(lev.GetGravityAppleCount(kv.Key))) &&
                   Date.Accepts(lev.DateModified) &&
                   (AcrossLev == BoolOption.Dontcare || !(AcrossLev == BoolOption.True ^ lev.IsAcrossLevel)) &&
                   Size.Accepts(lev.Size);
        }
    }
}