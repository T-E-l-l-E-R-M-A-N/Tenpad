using System.ComponentModel;
using System.IO;

namespace Tenpad.Core.Factory
{
    public interface IFileSystemModelFactory
    {
        IFileSystemModel GetNewFileSystemModelItem(FileSystemModelType type, FileSystemInfo info,
            PropertyChangedEventHandler onPropertyChanged = null);
    }
}