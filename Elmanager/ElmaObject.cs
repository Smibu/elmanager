using System;
using System.ComponentModel;

namespace Elmanager
{
    public class ElmaObject
    {
        [Description("Date modified")] public DateTime DateModified;
        public int Size;
        internal string Path;

        [Description("Size (kB)")] public double SizeInKb => Size / 1024.0;

        [Description("File name")] public string FileNameNoExt => System.IO.Path.GetFileNameWithoutExtension(FileName);

        public string FileName
        {
            get => System.IO.Path.GetFileName(Path);
            set
            {
                var fileName = value + Constants.RecExtension;
                Path = System.IO.Path.GetDirectoryName(Path) + "\\" + fileName;
            }
        }
    }
}