using System;
using System.Collections.Generic;
using System.ComponentModel;
using Tenpad.Database;

namespace Tenpad.Core.Services
{
    public interface IMenuService
    {
        public MainViewModel MainViewModel { get; }
        public TenpadDbContext TenpadDbContext { get; }
        public IList<IMenuItemViewModel> MainMenu { get; set; }
        public IList<IMenuItemViewModel> ContextMenu { get; set; }
        public ITabItemViewModel ActiveTabViewModel { get; set; }
        void Init(MainViewModel mainViewModel);
        void MenuItem_PropertyChanged(object s, PropertyChangedEventArgs e);
    }
}
