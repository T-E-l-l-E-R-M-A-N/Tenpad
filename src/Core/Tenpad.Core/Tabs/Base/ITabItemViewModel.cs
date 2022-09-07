namespace Tenpad.Core
{
    public interface ITabItemViewModel
    {
        string Header { get; set; }
        TabType Type { get; set; }
    }
}