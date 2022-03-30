namespace Elmanager.Lgr;

internal interface IImageMeta
{
    string Name { get; }
    ImageType Type { get; }
    ClippingType ClippingType { get; }
    int Distance { get; }
}