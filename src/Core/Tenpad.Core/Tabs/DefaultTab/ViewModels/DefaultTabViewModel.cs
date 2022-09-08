using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Tenpad.Core.Factory;

namespace Tenpad.Core
{
    public sealed class DefaultTabViewModel : TabItemViewModelBase, IDefaultTabViewModel
    {

        #region Private Fields

        private MainViewModel? _mainViewModel;
        private readonly ITabViewModelFactory _tabViewModelFactory;
        private readonly IPageViewModelFactory _pageViewModelFactory;
        private IPageViewModel? _pageViewModel;
        private IPageViewModel? _previousPageViewModel;

        #endregion

        #region Public Properties
        public bool IsSelected { get; set; }

        public IPageViewModel? PreviousPageViewModel
        {
            get => _previousPageViewModel;
            set
            {
                _previousPageViewModel = value;
                OnPropertyChanged();
            }
        }

        public IPageViewModel? PageViewModel
        {
            get => _pageViewModel;
            set
            {
                _pageViewModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsPinned { get; set; }

        #endregion

        #region Constructor

        public DefaultTabViewModel(ITabViewModelFactory tabViewModelFactory, IPageViewModelFactory pageViewModelFactory) : base(TabType.Default)
        {
            _tabViewModelFactory = tabViewModelFactory;
            _pageViewModelFactory = pageViewModelFactory;
            Header = "Blank Tab";

            ReturnCommand = new DelegateCommand(OnReturn, OnCanReturn);
            ReturnHomeCommand = new DelegateCommand(OnReturnHome, OnCanReturnHome);
            CloseTabCommand = new DelegateCommand(OnCloseTab);
        }
        #endregion

        #region Commands

        public DelegateCommand CloseTabCommand { get; }
        public DelegateCommand ReturnCommand { get; set; }
        public DelegateCommand ReturnHomeCommand { get; set; }

        #endregion

        #region Commands Methods

        private void OnCloseTab()
        {
            if (PageViewModel is DocumentPageViewModel documentPageViewModel 
                && documentPageViewModel.ActiveDocument.FullName != null 
                && documentPageViewModel.DocumentContent != File.ReadAllText(documentPageViewModel.ActiveDocument.FullName))
            {
                var dlg = MessageBox.Show("Do you want do save this document", "Saving Changes",
                    MessageBoxButton.YesNoCancel);
                switch (dlg)
                {
                    case MessageBoxResult.Yes:
                    {
                        File.WriteAllText(documentPageViewModel.DocumentContent, documentPageViewModel.ActiveDocument.FullName);
                        if (IsSelected)
                        {
                            var list = _mainViewModel.TabItemCollection;
                            list.Remove(this);
                            if (list.Count == 0)
                            {
                                _mainViewModel.CreateTabCommand.Execute();
                                _mainViewModel.TabItemCollection.Remove(this);

                            }
                            else
                            {
                                _mainViewModel.SelectedTabItemViewModel = list.Last();
                                _mainViewModel.TabItemCollection.Remove(this);
                            }

                        }
                        else
                        {
                            _mainViewModel.TabItemCollection.Remove(this);
                        }

                        break;
                    }
                    case MessageBoxResult.No when IsSelected:
                    {
                        var list = _mainViewModel.TabItemCollection;
                        list.Remove(this);
                        if (list.Count == 0)
                        {
                            _mainViewModel.CreateTabCommand.Execute();
                            _mainViewModel.TabItemCollection.Remove(this);

                        }
                        else
                        {
                            _mainViewModel.SelectedTabItemViewModel = list.Last();
                            _mainViewModel.TabItemCollection.Remove(this);
                        }

                        break;
                    }
                    case MessageBoxResult.No:
                        _mainViewModel.TabItemCollection.Remove(this);
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else
            {
                if (IsSelected)
                {
                    var list = _mainViewModel.TabItemCollection;
                    list.Remove(this);
                    if (list.Count == 0)
                    {
                        _mainViewModel.CreateTabCommand.Execute();
                        _mainViewModel.TabItemCollection.Remove(this);

                    }
                    else
                    {
                        _mainViewModel.SelectedTabItemViewModel = list.Last();
                        _mainViewModel.TabItemCollection.Remove(this);
                    }

                }
                else
                {
                    _mainViewModel.TabItemCollection.Remove(this);
                }
            }
        }

        private bool OnCanReturn() => PreviousPageViewModel != null;

        private void OnReturn()
        {
            PageViewModel = PreviousPageViewModel;
            PreviousPageViewModel = null;
            ReturnCommand.RaiseCanExecuteChanged();
            ReturnHomeCommand.RaiseCanExecuteChanged();
        }

        private bool OnCanReturnHome() => PageViewModel is not IHomePageViewModel;

        private void OnReturnHome()
        {
            PreviousPageViewModel = null;
            PageViewModel = _pageViewModelFactory.GetHomePageViewModel(_mainViewModel, this) as IPageViewModel;
            Header = "Tanpad | Home";
            ReturnHomeCommand.RaiseCanExecuteChanged();
            ReturnCommand.RaiseCanExecuteChanged();
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
            PreviousPageViewModel = PageViewModel;
            PageViewModel = pageViewModel;
            ReturnCommand.RaiseCanExecuteChanged();
            ReturnHomeCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region Private Events Handlers
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CloseTabCommand?.RaiseCanExecuteChanged();
        }


        #endregion
    }
}