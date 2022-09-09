using Prism.Commands;
using System.Collections.Generic;
using System.ComponentModel;

namespace Tenpad.Core.Factory
{
    internal sealed class MenuItemViewModelFactory : IMenuItemViewModelFactory
    {
        public IMenuItemViewModel GetMenuItemViewModel(string header, DelegateCommandBase command)
        {
            var menuItem = IoC.Resolve<IMenuItemViewModel>();
            menuItem.Init(header, command);
            return menuItem;
        }

        public IMenuItemViewModel GetMenuItemViewModel(string header, IList<IMenuItemViewModel> children)
        {
            var menuItem = IoC.Resolve<IMenuItemViewModel>();
            menuItem.Init(header, children);
            return menuItem;
        }

        public IMenuItemViewModel GetMenuItemViewModel(string header, bool isCheckable, PropertyChangedEventHandler onChecked)
        {
            var menuItem = IoC.Resolve<IMenuItemViewModel>();
            menuItem.Init(header, isCheckable, onChecked);
            return menuItem;
        }

        public IMenuItemViewModel GetMenuItemViewModel(string header, bool isCheckable, bool isRadioButtonMode, string group, PropertyChangedEventHandler onChecked)
        {
            var menuItem = IoC.Resolve<IMenuItemViewModel>();
            menuItem.Init(header, isCheckable, isRadioButtonMode, group, onChecked);
            return menuItem;
        }
    }
}
