namespace Tenpad.Core
{
    public interface IPageViewModel
    {
        string Header { get; set; }
        PageType Type { get; set; }

        void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);
    }
}