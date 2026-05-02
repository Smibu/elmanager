namespace Elmanager.Rendering;

public interface IGraphicsContext
{
    void MakeCurrent();
    void SwapBuffers();
}
