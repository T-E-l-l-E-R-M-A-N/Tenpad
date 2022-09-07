using System.IO;

namespace Tenpad.Core
{
    public interface IFileSystemModel
    {
        FileSystemInfo Info { get; }
        string Name { get; set; }
        string FullName { get; set; }
        FileSystemModelType Type { get; }

        void Init(FileSystemInfo info);
    }
}
