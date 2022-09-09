namespace Tenpad.Core.Factory
{
    public interface ITabViewModelFactory
    {
        ITabItemViewModel GetDefaultTabViewModel(MainViewModel mainViewModel, IPageViewModel pageViewModel);
    }
}
