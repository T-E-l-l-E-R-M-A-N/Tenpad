using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Prism.Commands;
using Tenpad.Core.Factory;
using Tenpad.Database;

namespace Tenpad.Core
{
    public sealed class HomePageViewModel : PageViewModelBase, IHomePageViewModel
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

        public string WelcomeText { get; set; }
        public ObservableCollection<DocumentViewModel> RecentDocumentItems { get; set; } =
            new();

        #endregion

        #region Constructor

        public HomePageViewModel(IPageViewModelFactory pageViewModelFactory, 
            IFileSystemModelFactory fileSystemModelFactory, 
            ITabViewModelFactory tabViewModelFactory,
            TenpadDbContext tenpadDbContext) : base(PageType.Home)
        {
            _pageViewModelFactory = pageViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tabViewModelFactory = tabViewModelFactory;
            _tenpadDbContext = tenpadDbContext;
            WelcomeText = "Tenpad | Welcome";

            CreateDocCommand = new DelegateCommand(OnOpenCreateDocPage);
            OpenBrowseCommand = new DelegateCommand(OnOpenBrowsePage);
        }

        #endregion

        #region Commands

        public DelegateCommand CreateDocCommand { get; }
        public DelegateCommand OpenBrowseCommand { get; }

        #endregion

        #region Commands Methods

        private void OnOpenCreateDocPage()
        {
            var p =
                _pageViewModelFactory.GetDocumentPageViewModel(_mainViewModel, _parentTabItemViewModel) as
                    DocumentPageViewModel;
            p.CreateDocument();
            _parentTabItemViewModel.NavigateToPage(p);
        }
        private void OnOpenBrowsePage()
        {
            var p = _pageViewModelFactory.GetBrowsePageViewModel(_mainViewModel, _parentTabItemViewModel) as BrowsePageViewModel;

            p.LoadDirectory(_fileSystemModelFactory.GetNewFileSystemModelItem(FileSystemModelType.Directory, new DirectoryInfo(p.HistoryService.Current.FullName)) as DirectoryViewModel);
            _parentTabItemViewModel.NavigateToPage(p);
        }

        #endregion

        #region Public Methods

        public override void Init(MainViewModel mainViewModel, ITabItemViewModel tabItemViewModel)
        {
            _mainViewModel = mainViewModel;
            _parentTabItemViewModel = tabItemViewModel as DefaultTabViewModel;

            _parentTabItemViewModel.Header = "Tanpad | Home";

            foreach (DataObjectModel dataObjectModel in _tenpadDbContext.Data.Where(x => x.Id.StartsWith("system_applocal_tenpad_userdata_data_recent")))
            {
                RecentDocumentItems.Add(new DocumentViewModel(new FileInfo(dataObjectModel.Content)));
            }
        }

        #endregion
    }
}