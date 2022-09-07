using System;
using System.ComponentModel;
using Prism.Commands;
using Tenpad.Core.Factory;

namespace Tenpad.Core
{
    public sealed class DefaultTabViewModel : TabItemViewModelBase, IDefaultTabViewModel
    {

        #region Private Fields

        private MainViewModel? _mainViewModel;
        private readonly ITabViewModelFactory _tabViewModelFactory;

        #endregion

        #region Public Properties

        public IPageViewModel? PreviousPageViewModel { get; set; }
        public IPageViewModel? PageViewModel { get; set; }
        public bool IsPinned { get; set; }

        #endregion

        #region Constructor

        public DefaultTabViewModel(ITabViewModelFactory tabViewModelFactory) : base(TabType.Default)
        {
            _tabViewModelFactory = tabViewModelFactory;
            Header = "Blank Tab";
            CloseTabCommand = new DelegateCommand(OnCloseTab, OnCanCloseTab);

            
        }
        #endregion

        #region Commands

        public DelegateCommand CloseTabCommand { get; }

        #endregion

        #region Commands Methods
        private bool OnCanCloseTab() => IsPinned == false;

        private void OnCloseTab()
        {
            _mainViewModel.TabItemCollection.Remove(this);
        }


        #endregion

        #region Public Methods

        public void Init(MainViewModel mainViewModel, IPageViewModel pageViewModel)
        {
            _mainViewModel = mainViewModel;
            PageViewModel = pageViewModel;

            this.PropertyChanged += OnPropertyChanged;
        }

        public void NavigateToPage(IPageViewModel pageViewModel)
        {
            var t = _tabViewModelFactory.GetDefaultTabViewModel(_mainViewModel, pageViewModel);
            _mainViewModel.TabItemCollection.Add(t);
            _mainViewModel.SelectedTabItemViewModel = t;
        }

        #endregion

        #region Private Events Handlers
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CloseTabCommand.RaiseCanExecuteChanged();
        }


        #endregion
    }
}