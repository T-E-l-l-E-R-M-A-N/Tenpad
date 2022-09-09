using Prism.Commands;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tenpad.Core.Factory
{
    public interface IMenuItemViewModelFactory
    {
        IMenuItemViewModel GetMenuItemViewModel(string header, DelegateCommandBase command);
        IMenuItemViewModel GetMenuItemViewModel(string header, IList<IMenuItemViewModel> children);
        IMenuItemViewModel GetMenuItemViewModel(string header, bool isCheckable, PropertyChangedEventHandler onChecked);
        IMenuItemViewModel GetMenuItemViewModel(string header, bool isCheckable, bool isRadioButtonMode, string group, PropertyChangedEventHandler onChecked);
    }
}
