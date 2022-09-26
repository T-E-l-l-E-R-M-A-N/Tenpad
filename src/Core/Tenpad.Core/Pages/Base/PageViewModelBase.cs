using Tenpad.Core.Services;

namespace Tenpad.Core
{
    public abstract class PageViewModelBase : BaseViewModel, IPageViewModel
    {
        #region PRivate Fields
        private IMenuService _menuService;
        #endregion

        #region Public Properties

        public string? Header { get; set; }
        public PageType Type { get; set; }

        #endregion

        #region Constructor
        protected PageViewModelBase(PageType type)
        {
            Type = type;
            _menuService = IoC.Resolve<IMenuService>();

            PropertyChanged += PageViewModelBased_PropertyChanged;
        }

        #endregion

        #region Public Methods

        public abstract void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);

        #endregion

        #region Private Events Handlers

        private void PageViewModelBased_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            (_menuService as MenuServiceImpl).OpenCommand?.RaiseCanExecuteChanged();
            (_menuService as MenuServiceImpl).SaveAsCommand?.RaiseCanExecuteChanged();
            (_menuService as MenuServiceImpl).SaveCommand?.RaiseCanExecuteChanged();
        }
        #endregion
    }
}