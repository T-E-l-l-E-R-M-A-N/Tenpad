using Prism.Commands;

namespace Tenpad.Core
{
    public interface IDefaultTabViewModel
    {
        IPageViewModel PreviousPageViewModel { get; set; }
        IPageViewModel PageViewModel { get; set; }
        bool IsPinned { get; set; }

        void Init(MainViewModel mainViewModel, IPageViewModel pageViewModel);

        void NavigateToPage(IPageViewModel pageViewModel);
    }
}