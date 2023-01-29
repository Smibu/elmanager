namespace Elmanager.IO;

internal record ElmaFileObject<T>(ElmaFile File, T Obj)
{
    public ElmaFileObject<T> WithPath(string path) => this with { File = new ElmaFile(path) };
    public static ElmaFileObject<T> FromPath(string path, T obj) => new(new ElmaFile(path), obj);
}