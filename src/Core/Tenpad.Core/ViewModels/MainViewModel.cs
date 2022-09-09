using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using HandyControl.Collections;
using Prism.Commands;
using Prism.Mvvm;
using Tenpad.Core.Factory;
using Tenpad.Database;

namespace Tenpad.Core
{
    public class MainViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly ITabViewModelFactory _tabViewModelFactory;
        private readonly IPageViewModelFactory _pageViewModelFactory;
        private readonly IMenuItemViewModelFactory _menuItemViewModelFactory;
        private readonly IFileSystemModelFactory _fileSystemModelFactory;
        private readonly TenpadDbContext _tenpadDbContext;
        private bool _isLeftTabStripEnabled;

        #endregion

        #region Public Properties
        public ObservableCollection<IMenuItemViewModel> MenuCollection { get; set; } =
            new();
        public ObservableCollection<ITabItemViewModel> TabItemCollection { get; set; } =
            new();

        public ITabItemViewModel SelectedTabItemViewModel { get; set; }

        public bool IsLeftTabStripEnabled
        {
            get => _tenpadDbContext != null
                   && _tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled") != null
                ? bool.Parse(_tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled").Content.ToString())
                : _isLeftTabStripEnabled;
            set
            {
                _isLeftTabStripEnabled = value;
                MenuCollection[2].Children[0].IsChecked = value;
                var dOM = new Tenpad
                    .Database
                    .DataObjectModel(
                        "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled",
                        $"{value}");

                if (_tenpadDbContext.Data.FirstOrDefault(x =>
                    x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled") != null)
                {
                    _tenpadDbContext.Remove(_tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled"));
                    _tenpadDbContext.Add(dOM);
                }

                else
                {
                    _tenpadDbContext.Add(dOM);
                }

                _tenpadDbContext.SaveChanges();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor

        public MainViewModel(
            ITabViewModelFactory tabViewModelFactory,
            IPageViewModelFactory pageViewModelFactory,
            IMenuItemViewModelFactory menuItemViewModelFactory,
            IFileSystemModelFactory fileSystemModelFactory,
            TenpadDbContext tenpadDbContext)
        {
            _tabViewModelFactory = tabViewModelFactory;
            _pageViewModelFactory = pageViewModelFactory;
            _menuItemViewModelFactory = menuItemViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tenpadDbContext = tenpadDbContext;

            
        }

        #endregion

        #region Commands

        public DelegateCommand CreateTabCommand { get; set; }

        #region Menu Items Commands
        public DelegateCommand MenuItemCreateCommand { get; set; }
        public DelegateCommand MenuItemOpenCommand { get; set; }
        public DelegateCommand MenuItemSaveCommand { get; set; }
        public DelegateCommand MenuItemSaveAsCommand { get; set; }
        public DelegateCommand MenuItemExitCommand { get; set; }
        #endregion

        #endregion

        #region Commands Methods

        private void OnCreateTab()
        {
            var tab = _tabViewModelFactory.GetDefaultTabViewModel(this,
                null) as DefaultTabViewModel;
            tab.PageViewModel = _pageViewModelFactory.GetHomePageViewModel(this, tab) as IPageViewModel;
            TabItemCollection.Add(tab as ITabItemViewModel);

            SelectedTabItemViewModel = TabItemCollection.LastOrDefault();
        }

        #region Menu Items Commands Methods
        private bool OnCanOpen() => (SelectedTabItemViewModel as DefaultTabViewModel).PageViewModel is not IBrowsePageViewModel;

        private void OnOpen()
        {
            if ((SelectedTabItemViewModel as DefaultTabViewModel).PageViewModel is IDocumentPageViewModel doc && doc.ActiveDocument != null)
            {
                var tab = _tabViewModelFactory.GetDefaultTabViewModel(this,
                null) as DefaultTabViewModel;
                var p = _pageViewModelFactory.GetBrowsePageViewModel(this, tab) as IPageViewModel;
                (p as BrowsePageViewModel).LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo((p as BrowsePageViewModel).HistoryService.Current.FullName)) as DirectoryViewModel);
                tab.PageViewModel = p;
                TabItemCollection.Add(tab);
                SelectedTabItemViewModel = TabItemCollection.LastOrDefault();
            }
            else
            {
                var p = _pageViewModelFactory.GetBrowsePageViewModel(this, SelectedTabItemViewModel) as IPageViewModel;
                (p as BrowsePageViewModel).LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo((p as BrowsePageViewModel).HistoryService.Current.FullName)) as DirectoryViewModel);
                (SelectedTabItemViewModel as DefaultTabViewModel).NavigateToPage(p);
            }
        }

        private void OnCreate()
        {
            if ((SelectedTabItemViewModel as DefaultTabViewModel).PageViewModel is IDocumentPageViewModel doc && doc.ActiveDocument != null)
            {
                var tab = _tabViewModelFactory.GetDefaultTabViewModel(this,
                null) as DefaultTabViewModel;
                var p = _pageViewModelFactory.GetDocumentPageViewModel(this, tab) as IPageViewModel;
                (p as IDocumentPageViewModel).CreateDocument();
                tab.PageViewModel = p;
                TabItemCollection.Add(tab);
                SelectedTabItemViewModel = TabItemCollection.LastOrDefault();
            }
            else
            {
                var p = _pageViewModelFactory.GetDocumentPageViewModel(this, SelectedTabItemViewModel) as IPageViewModel;
                (p as IDocumentPageViewModel).CreateDocument();
                (SelectedTabItemViewModel as DefaultTabViewModel).NavigateToPage(p);
            }
        }
        private bool OnCanSave() => 
            (SelectedTabItemViewModel as DefaultTabViewModel).PageViewModel is IDocumentPageViewModel doc 
            && doc.DocumentContent != File.ReadAllText(doc.ActiveDocument.FullName);
        private void OnSave()
        {
            var c = (SelectedTabItemViewModel as IDocumentPageViewModel).DocumentContent;
            (SelectedTabItemViewModel as IDocumentPageViewModel).DocumentContent = null;
            File.WriteAllText((SelectedTabItemViewModel as IDocumentPageViewModel).ActiveDocument.FullName, c);
        }
        private void OnSaveAs()
        {
            var p = _pageViewModelFactory.GetBrowsePageViewModel(this, SelectedTabItemViewModel) as IPageViewModel;
            (p as BrowsePageViewModel).Saving = true;
            (p as BrowsePageViewModel).NewFileName = (SelectedTabItemViewModel as IDocumentPageViewModel).ActiveDocument.FullName;
            (SelectedTabItemViewModel as DefaultTabViewModel).NavigateToPage(p);
        }

        private void OnCloseWindow()
        {
            Application.Current.Windows.OfType<Window>().First(w => w.IsActive).Close();
        }
        #endregion


        #endregion

        #region Public Methods
        public void Init()
        {
            var dOM = new Tenpad
                .Database
                .DataObjectModel(
                    "system_applocal_tenpad_userdata_data_directory",
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Tenpad\\Data\\");

            if (_tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_directory") == null)
            {
                _tenpadDbContext.Data.Add(dOM);
            }

            _tenpadDbContext.SaveChanges();

            CreateTabCommand = new DelegateCommand(OnCreateTab);

            {
                MenuItemOpenCommand = new DelegateCommand(OnOpen, OnCanOpen);
                MenuItemCreateCommand = new DelegateCommand(OnCreate);
                MenuItemSaveCommand = new DelegateCommand(OnSave, OnCanSave);
                MenuItemSaveAsCommand = new DelegateCommand(OnSaveAs, OnCanSave);
                MenuItemExitCommand = new DelegateCommand(OnCloseWindow);
            }

            CreateTabCommand.Execute();

            InitMainMenu();

            ((SelectedTabItemViewModel as DefaultTabViewModel).PageViewModel as PageViewModelBase).PropertyChanged += MainViewModel_PropertyChanged;

        }

        private void InitMainMenu()
        {
            MenuCollection.Add(_menuItemViewModelFactory.GetMenuItemViewModel("File", new List<IMenuItemViewModel> 
            {
                _menuItemViewModelFactory.GetMenuItemViewModel("Create", MenuItemCreateCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Open...", MenuItemOpenCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Save", MenuItemSaveCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Save As", MenuItemSaveAsCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Exit", MenuItemExitCommand),
            }));
            MenuCollection.Add(_menuItemViewModelFactory.GetMenuItemViewModel("Edit", new List<IMenuItemViewModel>
            {
                _menuItemViewModelFactory.GetMenuItemViewModel("Copy", MenuItemOpenCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Cut", MenuItemOpenCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Paste", MenuItemSaveCommand)
            }));
            MenuCollection.Add(_menuItemViewModelFactory.GetMenuItemViewModel("View", new List<IMenuItemViewModel>
            {
                _menuItemViewModelFactory.GetMenuItemViewModel("Vertical Tabs", true, MenuItem_PropertyChangedEventHandler)
            }));
            MenuCollection.Add(_menuItemViewModelFactory.GetMenuItemViewModel("Tools", new List<IMenuItemViewModel>
            {
                _menuItemViewModelFactory.GetMenuItemViewModel("Word Wrap", true, MenuItem_PropertyChangedEventHandler)
            }));
            MenuCollection.Add(_menuItemViewModelFactory.GetMenuItemViewModel("Help", new List<IMenuItemViewModel>
            {
                _menuItemViewModelFactory.GetMenuItemViewModel("About Tenpad", MenuItemExitCommand)
            }));
        }
        #endregion

        #region Private Events Handlers
        private void MainViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MenuItemSaveCommand.RaiseCanExecuteChanged();
            MenuItemSaveAsCommand.RaiseCanExecuteChanged();
        }

        private void MenuItem_PropertyChangedEventHandler(object s, PropertyChangedEventArgs e)
        {
            if(s is MenuItemViewModel m)
            {
                if(m.Header == "Vertical Tabs")
                {
                    IsLeftTabStripEnabled = m.IsChecked;
                }
            }
        }
        #endregion
    }
}
