using System.Collections.Generic;

namespace Tenpad.Core.Factory
{
    public interface IPageViewModelFactory
    {
        IHomePageViewModel GetHomePageViewModel(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);
        IDocumentPageViewModel GetDocumentPageViewModel(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);
        IBrowsePageViewModel GetBrowsePageViewModel(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);
    }
}