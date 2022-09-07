namespace Tenpad.Core
{
    public class TabItemViewModelBase : BaseViewModel, ITabItemViewModel
    {
        public string? Header { get; set; }
        public TabType Type { get; set; }

        public TabItemViewModelBase(TabType type)
        {
            Type = type;
        }
    }
}