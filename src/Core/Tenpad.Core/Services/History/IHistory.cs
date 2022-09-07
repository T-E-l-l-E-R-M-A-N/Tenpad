using System;
using System.Collections.Generic;

namespace Tenpad.Core.Services
{
    public interface IHistory : IEnumerable<HistoryNode>
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }

        HistoryNode Current { get; }

        event EventHandler Navigated;
        public void Init(string name, string fullName);
        void Update(DirectoryViewModel directoryViewModel);
        void GoBack();
        void GoForward();
    }
}