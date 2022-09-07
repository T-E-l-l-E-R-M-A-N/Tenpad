namespace Tenpad.Core
{
    public interface IPageViewModel
    {
        string Header { get; set; }
        PageType Type { get; set; }

        void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);
    }

    public abstract class PageViewModelBase : BaseViewModel, IPageViewModel
    {
        #region Public Properties

        public string? Header { get; set; }
        public PageType Type { get; set; }

        #endregion

        #region Constructor
        protected PageViewModelBase(PageType type)
        {
            Type = type;

        }
        #endregion

        #region Public Methods

        public abstract void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel);

        #endregion
    }
}