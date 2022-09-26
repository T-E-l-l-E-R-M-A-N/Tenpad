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
using Tenpad.Core.Services;
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
        private ITabItemViewModel _tabItemVm;

        #endregion

        #region Public Properties
        public string StatusText { get; set; }
        public IMenuService MenuService { get; set; }
        public ObservableCollection<ITabItemViewModel> TabItemCollection { get; set; } =
            new();
            
        public ITabItemViewModel SelectedTabItemViewModel 
        {
            get => _tabItemVm; 
            set
            {
                
                _tabItemVm = value;
                OnPropertyChanged();
            }
        }

        public bool IsLeftTabStripEnabled
        {
            get => _tenpadDbContext != null
                   && _tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled") != null
                ? bool.Parse(_tenpadDbContext.Data.FirstOrDefault(x => x.Id == "system_applocal_tenpad_userdata_data_settings_isLeftTabStripIsEnabled").Content.ToString())
                : _isLeftTabStripEnabled;
            set
            {
                _isLeftTabStripEnabled = value;
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
            TenpadDbContext tenpadDbContext,
            IMenuService menuService)
        {
            _tabViewModelFactory = tabViewModelFactory;
            _pageViewModelFactory = pageViewModelFactory;
            _menuItemViewModelFactory = menuItemViewModelFactory;
            _fileSystemModelFactory = fileSystemModelFactory;
            _tenpadDbContext = tenpadDbContext;

            MenuService = menuService;
            
        }

        #endregion

        #region Commands

        public DelegateCommand CreateTabCommand { get; set; }

        #endregion

        #region Commands Methods

        private void OnCreateTab()
        {
            var tab = _tabViewModelFactory.GetDefaultTabViewModel(this,
                null) as DefaultTabViewModel;
           
            tab.PageViewModel = _pageViewModelFactory.GetHomePageViewModel(this, tab) as IPageViewModel;
            TabItemCollection.Add(tab);

            SelectedTabItemViewModel = tab;
        }


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
            CreateTabCommand.Execute();

            MenuService.Init(this);
            MenuService.ActiveTabViewModel = SelectedTabItemViewModel;
        }

        #endregion

        #region Private Events Handlers
        #endregion
    }
}
