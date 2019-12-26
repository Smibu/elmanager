namespace Elmanager
{
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

        internal double AspectRatio => TextureWidth / TextureHeight;

        internal bool IsPicture => TextureName == null;

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
}