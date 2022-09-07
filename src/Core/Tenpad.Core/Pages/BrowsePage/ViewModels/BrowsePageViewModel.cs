using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Prism.Commands;
using Tenpad.Core.Factory;
using Tenpad.Core.Services;
using Tenpad.Database;

namespace Tenpad.Core
{
    public sealed class BrowsePageViewModel : PageViewModelBase, IBrowsePageViewModel
    {
        #region Private Fields

        private MainViewModel? _mainViewModel;
        private DefaultTabViewModel? _parentTabItemViewModel;

        private readonly IPageViewModelFactory _pageViewModelFactory;
        private readonly IFileSystemModelFactory _fileSystemModelFactory;
        private readonly TenpadDbContext _tenpadDbContext;

        #endregion

        #region Public Properties

        public string Path { get; set; }

        public string SaveFileDocumentContent { get; set; }

        //public DirectoryInfo CurrentDirectory { get; set; }
        public IHistory HistoryService { get; set; }

        public ObservableCollection<IFileSystemModel> DirectoriesAndFiles { get; set; } =
            new();
        public string NewFileName { get; set; }

        #endregion

        #region Constructor

        public BrowsePageViewModel(IPageViewModelFactory pageViewModelFactory,
            IFileSystemModelFactory fileSystemModelFactory,
            TenpadDbContext tenpadDbContext,
            IHistory historyService) : base(PageType.OpenDoc)
        {
            _pageViewModelFactory = pageViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tenpadDbContext = tenpadDbContext;
            HistoryService = historyService;

            OpenDirectoryCommand = new DelegateCommand<DirectoryViewModel>(OnOpenDirectory);
            OpenDocumentCommand = new DelegateCommand<IFileSystemModel>(OnOpenDocument);
            SaveDocumentCommand = new DelegateCommand(OnSaveDocument);

            GoBackCommand = new DelegateCommand(OnGoBack, OnCanGoBack);
            GoForwardCommand = new DelegateCommand(OnGoForward, OnCanGoForward);
        }

        #endregion

        #region Commands

        public DelegateCommand<DirectoryViewModel> OpenDirectoryCommand { get; set; }
        public DelegateCommand<IFileSystemModel> OpenDocumentCommand { get; set; }
        public DelegateCommand SaveDocumentCommand { get; set; }

        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }

        #endregion

        #region Commands Methods
        private void OnOpenDirectory(DirectoryViewModel obj)
        {
            HistoryService.Update(obj);
            LoadDirectory(obj);
        }
        private void OnOpenDocument(IFileSystemModel obj)
        {
            var d =
                _pageViewModelFactory.GetDocumentPageViewModel(_mainViewModel, _parentTabItemViewModel) as
                    IPageViewModel;
            (d as DocumentPageViewModel).LoadDocument(obj as FileViewModel);
            _parentTabItemViewModel.NavigateToPage(d);
        }
        private void OnSaveDocument()
        {
            File.WriteAllText(NewFileName, SaveFileDocumentContent);
        }

        private bool OnCanGoBack() => HistoryService.CanGoBack;
        private void OnGoBack()
        {
            HistoryService.GoBack();
            LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo(HistoryService.Current.FullName)) as DirectoryViewModel);
        }
        private bool OnCanGoForward() => HistoryService.CanGoForward;
        private void OnGoForward()
        {
            HistoryService.GoForward();
            LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo(HistoryService.Current.FullName)) as DirectoryViewModel);
        }

        #endregion

        #region Public Methods

        public override void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel)
        {
            _mainViewModel = mainViewModel;
            _parentTabItemViewModel = tabItemViewModel as DefaultTabViewModel;
            HistoryService.Init(Environment.UserName, Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        public void LoadDirectory(DirectoryViewModel directoryViewModel)
        {
            DirectoriesAndFiles.Clear();

            var dir = directoryViewModel.Info as DirectoryInfo;

            Header = dir.Name;
            Path = dir.FullName;
            _parentTabItemViewModel.Header = Header;

            foreach (var enumerateDirectory in dir.EnumerateDirectories())
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DirectoriesAndFiles.Add(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, enumerateDirectory));
                }, DispatcherPriority.Background);
            }

            foreach (var enumerateFile in dir.EnumerateFiles(".txt"))
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DirectoriesAndFiles.Add(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.File, enumerateFile));
                }, DispatcherPriority.Background);
            }
        }

        #endregion

    }
}