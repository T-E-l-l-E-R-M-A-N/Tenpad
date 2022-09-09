using System.Collections.ObjectModel;

namespace Tenpad.Core
{
    public interface IHomePageViewModel
    {
        string WelcomeText { get; set; }
        ObservableCollection<FileViewModel> RecentDocumentItems { get; set; }

    }
}
