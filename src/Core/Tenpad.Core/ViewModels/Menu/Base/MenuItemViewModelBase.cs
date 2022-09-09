using System.Collections.Generic;
using System.ComponentModel;
using Prism.Commands;

namespace Tenpad.Core
{
    public abstract class MenuItemViewModelBase : BaseViewModel, IMenuItemViewModel
    {
        #region Public Properties
        public string Header { get; set; }
        public bool IsChecked { get; set; }
        public bool IsRadioButtonMode { get; set; }
        public string Group { get; set; }
        public DelegateCommandBase Command { get; set; }
        public IList<IMenuItemViewModel> Children { get; set; }
        public bool IsCheckable { get; set; }
        #endregion

        #region Public Methods
        public void Init(string header, DelegateCommandBase command)
        {
            Header = header;
            Command = command;
        }

        public void Init(string header, bool isCheckable, PropertyChangedEventHandler onChecked)
        {
            Header = header;
            IsCheckable = isCheckable;
            PropertyChanged += onChecked;
        }

        public void Init(string header, bool isCheckable, bool isRadioButtonMode, string group, PropertyChangedEventHandler onChecked)
        {
            Header = header;
            IsCheckable = isCheckable;
            IsRadioButtonMode = isRadioButtonMode;
            Group = group;
            PropertyChanged += onChecked;
        }

        public void Init(string header, IList<IMenuItemViewModel> children)
        {
            Header = header;
            Children = children;
        }
        #endregion
    }
}