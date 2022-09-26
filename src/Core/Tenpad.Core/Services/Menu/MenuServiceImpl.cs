using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Tenpad.Core.Factory;
using Tenpad.Database;

namespace Tenpad.Core.Services
{
    internal sealed class MenuServiceImpl : BaseViewModel, IMenuService
    {
        #region Private Fields
        private readonly IMenuItemViewModelFactory _menuItemViewModelFactory;
        private readonly IFileSystemModelFactory _fileSystemModelFactory;
        private readonly ITabViewModelFactory _tabViewModelFactory;
        private readonly IPageViewModelFactory _pageViewModelFactory;

        private readonly List<string> _partitions = new List<string>();
        #endregion

        #region Public Propeerties
        public TenpadDbContext TenpadDbContext { get; }
        public MainViewModel MainViewModel { get; internal set; }
        public IList<IMenuItemViewModel> MainMenu { get; set; }
        public IList<IMenuItemViewModel> ContextMenu { get; set; }
        public ITabItemViewModel ActiveTabViewModel { get; set; }
        #endregion

        #region Constructor
        public MenuServiceImpl(
            TenpadDbContext tenpadDbContext,
            IMenuItemViewModelFactory menuItemViewModelFactory,
            IFileSystemModelFactory fileSystemModelFactory,
            ITabViewModelFactory tabViewModelFactory,
            IPageViewModelFactory pageViewModelFactory)
        {
            TenpadDbContext = tenpadDbContext;
            _menuItemViewModelFactory = menuItemViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tabViewModelFactory = tabViewModelFactory;
            _pageViewModelFactory = pageViewModelFactory;
        }
        #endregion

        #region Menu Commands
        public DelegateCommand CreateCommand => new DelegateCommand(OnCreate);
        public DelegateCommand ExitCommand => new DelegateCommand(OnExit);
        public DelegateCommand OpenCommand => new DelegateCommand(OnOpen, OnCanOpen);
        public DelegateCommand SaveCommand => new DelegateCommand(OnSave, OnCanSave);
        public DelegateCommand SaveAsCommand => new DelegateCommand(OnSaveAs, OnCanSave);
        #endregion

        #region Commands Methods
        private void OnCreate()
        {
            var p = _pageViewModelFactory.GetDocumentPageViewModel(MainViewModel, ActiveTabViewModel) as DocumentPageViewModel;
            p.CreateDocument();
            if ((ActiveTabViewModel as DefaultTabViewModel).PageViewModel is not DocumentPageViewModel)
            {
                (ActiveTabViewModel as DefaultTabViewModel).NavigateToPage(p);
            }
            else
            {
                var tab = _tabViewModelFactory.GetDefaultTabViewModel(MainViewModel, p);
                MainViewModel.TabItemCollection.Add(tab);
                MainViewModel.SelectedTabItemViewModel = ActiveTabViewModel = tab;
            }
        }

        private bool OnCanOpen() => ActiveTabViewModel is DefaultTabViewModel d && d.PageViewModel is not BrowsePageViewModel;
        private void OnOpen()
        {
            var p = _pageViewModelFactory.GetBrowsePageViewModel(MainViewModel, ActiveTabViewModel) as BrowsePageViewModel;

            p.LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo(p.HistoryService.Current.FullName)) as DirectoryViewModel);
            (ActiveTabViewModel as DefaultTabViewModel).NavigateToPage(p);
        }

        private bool OnCanSave() => (ActiveTabViewModel as DefaultTabViewModel).PageViewModel is DocumentPageViewModel;
        private void OnSave()
        {
            var docPage = (ActiveTabViewModel as DefaultTabViewModel).PageViewModel as DocumentPageViewModel;
            if(docPage.Header == "Blank Document")
            {

                OnSaveAs();
            }
            else
            {
                File.WriteAllText(docPage.ActiveDocument.FullName, docPage.DocumentContent);
            }
        }
        private void OnSaveAs()
        {
            var docPage = (ActiveTabViewModel as DefaultTabViewModel).PageViewModel as DocumentPageViewModel;
            var p = _pageViewModelFactory.GetBrowsePageViewModel(MainViewModel, ActiveTabViewModel) as BrowsePageViewModel;
            p.NewFileName = docPage.Header == "Blank Document" ? $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\": docPage.ActiveDocument.FullName;
            p.Saving = true;
            p.LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))) as DirectoryViewModel);
            (ActiveTabViewModel as DefaultTabViewModel).NavigateToPage(p);
        }

        private void OnExit()
        {
            if ((ActiveTabViewModel as DefaultTabViewModel).PageViewModel is DocumentPageViewModel d && d.DocumentContent != File.ReadAllText(d.ActiveDocument.FullName))
            {
                var dlg = MessageBox.Show("Do you want do save changes in file?", "Closing", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                switch (dlg)
                {
                    case MessageBoxResult.Yes:
                        foreach(var t in MainViewModel
                            .TabItemCollection
                            .Where(x => ((x as DefaultTabViewModel).PageViewModel as DocumentPageViewModel)
                            .DocumentContent 
                            != File.ReadAllText(((x as DefaultTabViewModel).PageViewModel as DocumentPageViewModel)
                            .ActiveDocument
                            .FullName)))
                        {
                            var docPage = (t as DefaultTabViewModel).PageViewModel as DocumentPageViewModel;
                            var p = _pageViewModelFactory.GetBrowsePageViewModel(MainViewModel, t) as BrowsePageViewModel;
                            p.NewFileName = docPage.Header == "Blank Document" ? $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\" : docPage.ActiveDocument.FullName;
                            p.Saving = true;
                            p.LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))) as DirectoryViewModel);
                            (t as DefaultTabViewModel).NavigateToPage(p);
                        }
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive).Close();
                        break;
                    case MessageBoxResult.No:
                        Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive).Close();
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else
            {
                Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive).Close();
            }
        }
        #endregion

        #region Public Methods
        public void Init(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
            MainViewModel.PropertyChanged += MainViewModel_PropertyChanged;

            PropertyChanged += OnPublicPropertyChanged;

            BuildMainMenu();
        }
        #endregion

        #region Public Events Handlers
        public void MenuItem_PropertyChanged(object s, PropertyChangedEventArgs e)
        {

        }
        #endregion

        #region Private Events Handlers
        private void OnPublicPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OpenCommand.RaiseCanExecuteChanged();
            SaveAsCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var t = MainViewModel.SelectedTabItemViewModel as DefaultTabViewModel;
            if(t != null)
            {
                (t.PageViewModel as PageViewModelBase).PropertyChanged += (s, e) =>
                {
                    OpenCommand.RaiseCanExecuteChanged();
                    SaveAsCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                };
            }
            ActiveTabViewModel = t;
            
        }

        #endregion

        #region Private Methods
        private void BuildMainMenu()
        {
            MainMenu = new ObservableCollection<IMenuItemViewModel>();

            MainMenu.Add(_menuItemViewModelFactory.GetMenuItemViewModel("File", new ObservableCollection<IMenuItemViewModel>
            {
                _menuItemViewModelFactory.GetMenuItemViewModel("Create", CreateCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Open...", OpenCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Save", SaveCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Save As...", SaveAsCommand),
                _menuItemViewModelFactory.GetMenuItemViewModel("Exit", ExitCommand),
            }));
        }
        #endregion
    }
}
