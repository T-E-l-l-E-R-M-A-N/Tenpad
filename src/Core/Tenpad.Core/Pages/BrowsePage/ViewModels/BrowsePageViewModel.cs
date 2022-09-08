﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private readonly ITabViewModelFactory _tabViewModelFactory;
        private readonly TenpadDbContext _tenpadDbContext;

        #endregion

        #region Public Properties

        public string Path { get; set; }

        public string SaveFileDocumentContent { get; set; }

        //public DirectoryInfo CurrentDirectory { get; set; }
        public IHistory HistoryService { get; set; }

        public ObservableCollection<IFileSystemModel> DirectoriesAndFiles { get; set; } =
            new();

        public ObservableCollection<ISelectable> SelectedCollection { get; set; } =
            new();
        public string NewFileName { get; set; }

        #endregion

        #region Constructor

        public BrowsePageViewModel(IPageViewModelFactory pageViewModelFactory,
            IFileSystemModelFactory fileSystemModelFactory,
            ITabViewModelFactory tabViewModelFactory,
            TenpadDbContext tenpadDbContext,
            IHistory historyService) : base(PageType.OpenDoc)
        {
            _pageViewModelFactory = pageViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tabViewModelFactory = tabViewModelFactory;
            _tenpadDbContext = tenpadDbContext;
            HistoryService = historyService;

            OpenCommand = new DelegateCommand(OnOpen);
            SaveDocumentCommand = new DelegateCommand(OnSaveDocument);

            GoBackCommand = new DelegateCommand(OnGoBack, OnCanGoBack);
            GoForwardCommand = new DelegateCommand(OnGoForward, OnCanGoForward);

            HistoryService.Navigated += HistoryServiceOnNavigated;
        }
        #endregion

        #region Commands

        public DelegateCommand OpenCommand { get; set; }
        public DelegateCommand SaveDocumentCommand { get; set; }
        public DelegateCommand ReturnCommand => _parentTabItemViewModel.ReturnCommand;
        public DelegateCommand GoBackCommand { get; set; }
        public DelegateCommand GoForwardCommand { get; set; }

        #endregion

        #region Commands Methods
        private void OnOpen()
        {
            if (SelectedCollection.Count == 1 && SelectedCollection[0] is IFileSystemModel obj)
            {
                if (obj is DirectoryViewModel directory)
                {
                    HistoryService.Update(directory);
                    LoadDirectory(directory);
                }
                else if (obj is FileViewModel file)
                {
                    var d =
                        _pageViewModelFactory.GetDocumentPageViewModel(_mainViewModel, _parentTabItemViewModel) as
                            DocumentPageViewModel;
                    _parentTabItemViewModel.NavigateToPage(d);
                    d.LoadDocument(file);

                    var dOM = new Tenpad
                        .Database
                        .DataObjectModel(
                            $"system_applocal_tenpad_userdata_data_recent({file.Name})",
                            file.FullName);

                    if (_tenpadDbContext.Data.FirstOrDefault(x => x.Id == $"system_applocal_tenpad_userdata_data_recent({file.Name})") == null)
                    {
                        _tenpadDbContext.Data.Add(dOM);
                    }

                    _tenpadDbContext.SaveChanges();
                }
            }
            else if (SelectedCollection.Count >= 2)
            {
                foreach (var item in DirectoriesAndFiles.Where(x => (x as ISelectable).IsSelected && x is FileViewModel))
                {
                    var tab = _tabViewModelFactory.GetDefaultTabViewModel(_mainViewModel,
                        null) as DefaultTabViewModel;
                    tab.PageViewModel = _pageViewModelFactory.GetDocumentPageViewModel(_mainViewModel, tab) as IPageViewModel;
                    (tab.PageViewModel as DocumentPageViewModel).LoadDocument(item as FileViewModel);
                    _mainViewModel.TabItemCollection.Add(tab);

                    var dOM = new Tenpad
                        .Database
                        .DataObjectModel(
                            $"system_applocal_tenpad_userdata_data_directory_recent({item.Name})",
                            $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Tenpad\\Data\\");

                    if (_tenpadDbContext.Data.FirstOrDefault(x => x.Id == $"system_applocal_tenpad_userdata_data_recent({item.Name})") == null)
                    {
                        _tenpadDbContext.Data.Add(dOM);
                    }

                    _tenpadDbContext.SaveChanges();

                }
                _mainViewModel.TabItemCollection.Remove(_parentTabItemViewModel);
                _mainViewModel.SelectedTabItemViewModel = _mainViewModel.TabItemCollection.LastOrDefault();

                
            }
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
            SelectedCollection.Clear();

            var dir = directoryViewModel.Info as DirectoryInfo;

            Header = dir.Name;
            Path = dir.FullName;
            _parentTabItemViewModel.Header = Header;

            foreach (var enumerateDirectory in dir.EnumerateDirectories())
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    DirectoriesAndFiles.Add(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, enumerateDirectory, ISelectableModel_PropertyChanged));
                }, DispatcherPriority.Background);
            }

            foreach (var enumerateFile in dir.EnumerateFiles())
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (enumerateFile.Extension.ToLower() is ".txt" or ".rtf" or ".cs" or ".xml" or ".html")
                    {
                        DirectoriesAndFiles.Add(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.File, enumerateFile, ISelectableModel_PropertyChanged));
                    }
                }, DispatcherPriority.Background);
            }
        }

        private void ISelectableModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ISelectable sel and IFileSystemModel && e.PropertyName == nameof(ISelectable.IsSelected))
            {
                if (sel.IsSelected)
                {
                    SelectedCollection.Add(sel);
                }
                else
                {
                    SelectedCollection.Remove(sel);
                }
            }
        }

        #endregion

        #region Private Events Handlers
        private void HistoryServiceOnNavigated(object? sender, EventArgs e)
        {
            GoBackCommand.RaiseCanExecuteChanged();
            GoForwardCommand.RaiseCanExecuteChanged();
        }


        #endregion

    }
}