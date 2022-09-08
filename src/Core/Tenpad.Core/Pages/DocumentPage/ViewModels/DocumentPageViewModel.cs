using System;
using System.IO;
using System.Linq;
using System.Windows;
using Prism.Commands;
using Tenpad.Core.Factory;
using Tenpad.Database;

namespace Tenpad.Core
{
    public sealed class DocumentPageViewModel : PageViewModelBase, IDocumentPageViewModel
    {

        #region Private Fields

        private MainViewModel? _mainViewModel;
        private DefaultTabViewModel? _parentTabItemViewModel;
        private readonly IPageViewModelFactory _pageViewModelFactory;
        private readonly IFileSystemModelFactory _fileSystemModelFactory;
        private readonly TenpadDbContext _tenpadDbContext;
        private readonly Random rand = new Random();

        private string LocalTenpadUserDataDirectoryPath;

        #endregion

        #region Public Properties

        public FileViewModel ActiveDocument { get; set; }
        public string DocumentContent { get; set; }

        #endregion

        #region Constructor

        public DocumentPageViewModel(IPageViewModelFactory pageViewModelFactory, IFileSystemModelFactory fileSystemModelFactory, TenpadDbContext tenpadDbContext) : base(PageType.EditDoc)
        {
            _pageViewModelFactory = pageViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tenpadDbContext = tenpadDbContext;

            LocalTenpadUserDataDirectoryPath = _tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_directory").Content;
        }

        #endregion

        #region Commands

        public DelegateCommand CreateDocumentCommand { get; set; }
        public DelegateCommand OpenDocumentCommand { get; set; }
        public DelegateCommand SaveDocumentCommand { get; set; }
        public DelegateCommand SaveAsDocumentCommand { get; set; }
        public DelegateCommand ImportDocumentCommand { get; set; }

        #endregion

        #region Public Methods
        public override void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel)
        {
            _mainViewModel = mainViewModel;
            _parentTabItemViewModel = tabItemViewModel as DefaultTabViewModel;
        }
        public void LoadDocument(FileViewModel fileViewModel)
        {
            var d = new DocumentViewModel(fileViewModel.Info as FileInfo);
            d.Init(fileViewModel.Info as FileInfo);
            
            ActiveDocument = d;
            try
            {
                using (var stream = File.OpenRead(d.FullName))
                {
                    var reader = new StreamReader(stream);
                    DocumentContent = reader.ReadToEnd();
                }
                
                _parentTabItemViewModel.Header = Header = $"{ActiveDocument.Name} | Editor";
            }
            catch (Exception e)
            {
                if (MessageBox.Show(e.Message, "Error read document",
                    MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                {
                    //_parentTabItemViewModel.NavigateToPage();
                }
                _parentTabItemViewModel.Header = Header = "Empty | Editor";
            }
            
        }
        public void CreateDocument()
        {
            var d = rand.Next(5000);
            File.CreateText($"{LocalTenpadUserDataDirectoryPath}\\{d}_blank_.txt").Close();
            DocumentContent = new StreamReader(File.OpenRead($"{LocalTenpadUserDataDirectoryPath}\\{d}_blank_.txt")).ReadToEnd();
            ActiveDocument = new DocumentViewModel(new FileInfo($"{LocalTenpadUserDataDirectoryPath}\\{d}_blank_.txt"));
            _parentTabItemViewModel.Header = Header = $"Blank Document | Editor";
        }

        #endregion

        
    }
}