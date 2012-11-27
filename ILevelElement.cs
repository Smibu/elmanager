namespace Elmanager
{
    /// <summary>
    /// Not used yet, but meant to be a common interface for pictures and level objects.
    /// </summary>
    internal interface ILevelElement
    {
        Vector GetPosition();
        void MarkAs(Geometry.VectorMark mark);
        void Move(double dx, double dy);
        void SetPosition(Vector position);
        string ToString();
        void Transform(Matrix m);
    }
}