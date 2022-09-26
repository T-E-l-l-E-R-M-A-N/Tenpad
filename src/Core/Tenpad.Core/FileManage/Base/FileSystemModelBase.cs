using System.IO;
using Prism.Mvvm;
using Tenpad.Core.Services;

namespace Tenpad.Core
{
    public abstract class FileSystemModelBase : BaseViewModel, IFileSystemModel, ISelectable
    {
        private bool _isSelected;
        public FileSystemInfo Info { get; private set; } = null!;
        public string Name { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public FileSystemModelType Type { get; }
        public bool IsSelected 
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

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