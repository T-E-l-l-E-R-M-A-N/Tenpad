using System.ComponentModel;
using System.IO;

namespace Tenpad.Core.Factory
{
    internal sealed class FileSystemModelFactory : IFileSystemModelFactory
    {
        public IFileSystemModel GetNewFileSystemModelItem(FileSystemModelType type, FileSystemInfo info,
            PropertyChangedEventHandler? onPropertyChanged = null)
        {
            FileSystemModelBase obj;
            switch (type)
            {
                case FileSystemModelType.Directory:
                    obj = new DirectoryViewModel();
                    obj.Init(info as DirectoryInfo);
                    break;
                case FileSystemModelType.File or _:
                    obj = new FileViewModel(info as FileInfo);
                    obj.Init(info as FileInfo);
                    break;
            }

            obj.PropertyChanged += onPropertyChanged;

            return obj;
        }
    }
}