using Tenpad.Core.Services;

namespace Tenpad.Core
{
    public class TabItemViewModelBase : BaseViewModel, ITabItemViewModel, ISelectable
    {
        #region PRivate Fields
        private IMenuService _menuService;
        #endregion
        public string? Header { get; set; }
        public TabType Type { get; set; }

        public bool IsSelected { get; set; }
        public TabItemViewModelBase(TabType type)
        {
            Type = type;
            _menuService = IoC.Resolve<IMenuService>();

            PropertyChanged += TabVmBased_PropertyChanged;
        }

        #region Private Events Handlers

        private void TabVmBased_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (_menuService as MenuServiceImpl).OpenCommand?.RaiseCanExecuteChanged();
            (_menuService as MenuServiceImpl).SaveAsCommand?.RaiseCanExecuteChanged();
            (_menuService as MenuServiceImpl).SaveCommand?.RaiseCanExecuteChanged();
        }
        #endregion
    }
}