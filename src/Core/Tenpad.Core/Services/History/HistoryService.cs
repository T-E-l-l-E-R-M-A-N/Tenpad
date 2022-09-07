using System;
using System.Collections;
using System.Collections.Generic;

namespace Tenpad.Core.Services
{
    public sealed class HistoryService : IHistory
    {
        #region Private Fields

        private HistoryNode _main;

        #endregion

        #region Public Properties

        public bool CanGoBack => Current.Previous != null;
        public bool CanGoForward => Current.Next != null;
        public HistoryNode Current { get; private set; }

        #endregion

        #region Events

        public event EventHandler Navigated;

        #endregion

        #region Public Methods

        public void Init(string name, string fullName) => Current = _main = new HistoryNode(name, fullName);
        public void Update(DirectoryViewModel directoryViewModel)
        {
            var node = new HistoryNode(directoryViewModel.Name, directoryViewModel.FullName);

            Current.Next = node;
            node.Previous = Current;

            Current = node;

            RaiseNavigated();
        }

        public void GoBack()
        {
            var prev = Current.Previous;

            Current = prev!;

            RaiseNavigated();
        }

        public void GoForward()
        {
            var next = Current.Next;

            Current = next!;


            RaiseNavigated();
        }

        #endregion

        #region Private Methods

        private void RaiseNavigated() => Navigated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Enumerator

        public IEnumerator<HistoryNode> GetEnumerator()
        {
            yield return Current;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}