using System;
using System.IO;

namespace Elmanager.IO;

internal record ElmaFile(string Path)
{
    public DateTime DateModified => FileInfo.LastWriteTime;
    public int Size => (int)FileInfo.Length;

    public double SizeInKb => Size / 1024.0;

    public string FileNameNoExt => System.IO.Path.GetFileNameWithoutExtension(FileName);

    public string FileName => FileInfo.Name;

    public FileInfo FileInfo => _fileInfo ??= new FileInfo(Path);
    private FileInfo? _fileInfo;
}