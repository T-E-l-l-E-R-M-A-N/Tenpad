using System.Collections.ObjectModel;
using Prism.Commands;
using Tenpad.Core.Services;

namespace Tenpad.Core
{
    public interface IBrowsePageViewModel
    {
        IHistory HistoryService { get; set; }
        ObservableCollection<IFileSystemModel> DirectoriesAndFiles { get; set; }
        string NewFileName { get; set; }
        DelegateCommand OpenCommand { get; set; }
        DelegateCommand SaveDocumentCommand { get; set; }

        void LoadDirectory(DirectoryViewModel directoryViewModel);
    }
}
