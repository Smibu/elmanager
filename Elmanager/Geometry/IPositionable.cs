namespace Elmanager.Geometry;

internal interface IPositionable
{
    public double X { get; }
    public double Y { get; }
    public VectorMark Mark { get; set; }
    public Vector Position { get; }
}