using System.IO;

namespace Tenpad.Core
{
    public class FileViewModel : FileSystemModelBase
    {
        public string Extension { get; }
        public FileViewModel(FileInfo file) : base(FileSystemModelType.File)
        {
            Extension = file.Extension.ToUpper();
        }
    }
}