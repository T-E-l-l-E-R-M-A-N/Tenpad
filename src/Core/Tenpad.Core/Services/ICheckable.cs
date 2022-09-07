namespace Tenpad.Core.Services
{
    public interface ICheckable
    {
        bool IsChecked { get; set; }
        bool IsCheckable { get; set; }
    }
}