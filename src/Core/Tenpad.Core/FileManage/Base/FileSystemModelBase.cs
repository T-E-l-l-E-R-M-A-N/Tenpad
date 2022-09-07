using System.IO;
using Prism.Mvvm;
using Tenpad.Core.Services;

namespace Tenpad.Core
{
    public abstract class FileSystemModelBase : BaseViewModel, IFileSystemModel, ISelectable
    {
        public FileSystemInfo Info { get; private set; } = null!;
        public string Name { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public FileSystemModelType Type { get; }
        public bool IsSelected { get; set; }

        protected FileSystemModelBase(FileSystemModelType type)
        {
            Type = type;
        }
        public void Init(FileSystemInfo info)
        {
            Info = info;
            Name = info.Name;
            FullName = info.FullName;
        }

    }
}