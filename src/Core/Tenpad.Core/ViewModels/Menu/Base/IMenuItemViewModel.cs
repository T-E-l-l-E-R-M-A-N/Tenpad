using System.Collections.Generic;
using System.ComponentModel;
using Prism.Commands;

namespace Tenpad.Core
{
    public interface IMenuItemViewModel
    {
        string Header { get; set; }

        bool IsCheckable { get; set; }

        bool IsChecked { get; set; }

        bool IsRadioButtonMode { get; set; }
        string Group { get; set; }

        DelegateCommandBase Command { get; set; }
        IList<IMenuItemViewModel> Children { get; set; }

        void Init(string header, DelegateCommandBase command);
        void Init(string header, IList<IMenuItemViewModel> children);
        void Init(string header, bool isCheckable, PropertyChangedEventHandler onChecked);
        void Init(string header, bool isCheckable, bool isRadioButtonMode, string Group, PropertyChangedEventHandler onChecked);
    }
}