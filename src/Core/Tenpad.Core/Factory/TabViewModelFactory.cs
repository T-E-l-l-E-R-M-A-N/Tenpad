using System.Collections.Generic;
using System.Linq;

namespace Tenpad.Core.Factory
{
    internal sealed class TabViewModelFactory : ITabViewModelFactory
    {

        public ITabItemViewModel GetDefaultTabViewModel(MainViewModel mainViewModel, IPageViewModel pageViewModel)
        {
            var t = IoC.Resolve<IEnumerable<ITabItemViewModel>>().FirstOrDefault(x => x.Type == TabType.Default) as DefaultTabViewModel;
            t.Init(mainViewModel, pageViewModel);
            return t;
        }

    }
}