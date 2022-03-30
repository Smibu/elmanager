namespace Elmanager.Lgr;

internal record ImageMeta(string Name, ImageType Type, ClippingType ClippingType, int Distance) : IImageMeta;