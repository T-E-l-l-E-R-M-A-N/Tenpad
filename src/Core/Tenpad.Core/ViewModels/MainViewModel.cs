using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
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
        private readonly TenpadDbContext _tenpadDbContext;
        private bool _isLeftTabStripEnabled;

        #endregion

        #region Public Properties

        public ObservableCollection<ITabItemViewModel> TabItemCollection { get; set; } =
            new();

        public ITabItemViewModel SelectedTabItemViewModel { get; set; }

        public bool IsLeftTabStripEnabled
        {
            get => _tenpadDbContext != null 
                   && _tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled") != null
                ? bool.Parse(_tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled").Content) 
                : _isLeftTabStripEnabled;
            set
            {
                _isLeftTabStripEnabled = value;
                var dOM = new Tenpad
                    .Database
                    .DataObjectModel(
                        "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled",
                        $"{value}");

                if (_tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled") == null)
                {
                    _tenpadDbContext.Data.Add(dOM);
                }
                else
                {
                    _tenpadDbContext.Data.Update(dOM);
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
            TenpadDbContext tenpadDbContext)
        {
            _tabViewModelFactory = tabViewModelFactory;
            _pageViewModelFactory = pageViewModelFactory;
            _tenpadDbContext = tenpadDbContext;

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

            CreateTabCommand.Execute();
        }

        #endregion

        #region Commands

        public DelegateCommand CreateTabCommand { get; }

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

        #endregion
    }
}
