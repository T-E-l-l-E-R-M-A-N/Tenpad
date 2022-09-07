using System.Collections.Generic;
using System.Linq;

namespace Tenpad.Core.Factory
{
    internal sealed class PageViewModelFactory : IPageViewModelFactory
    {
        public IHomePageViewModel GetHomePageViewModel(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel)
        {
            var page = IoC.Resolve<IEnumerable<IPageViewModel>>().FirstOrDefault(x => x.Type == PageType.Home) as HomePageViewModel;
            page.Init(mainViewModel, tabItemViewModel);
            return page;
        }

        public IDocumentPageViewModel GetDocumentPageViewModel(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel)
        {
            var page = IoC.Resolve<IEnumerable<IPageViewModel>>().FirstOrDefault(x => x.Type == PageType.EditDoc) as DocumentPageViewModel;
            page.Init(mainViewModel, tabItemViewModel);
            return page;
        }

        public IBrowsePageViewModel GetBrowsePageViewModel(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel)
        {
            var page = IoC.Resolve<IEnumerable<IPageViewModel>>().FirstOrDefault(x => x.Type == PageType.OpenDoc) as BrowsePageViewModel;
            page.Init(mainViewModel, tabItemViewModel);
            return page;
        }
    }
}