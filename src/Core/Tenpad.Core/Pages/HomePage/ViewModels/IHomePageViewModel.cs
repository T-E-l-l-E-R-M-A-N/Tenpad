using System.Collections.ObjectModel;

namespace Tenpad.Core
{
    public interface IHomePageViewModel
    {
        string WelcomeText { get; set; }
        ObservableCollection<DocumentViewModel> RecentDocumentItems { get; set; }

    }
}
