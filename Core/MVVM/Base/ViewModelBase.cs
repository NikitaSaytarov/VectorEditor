using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Serilog;

namespace VectorEditor.Core.MVVM.Base
{
    public abstract class ViewModelBase : NotifyObject, IDisposable
    {
        private readonly List<(Action<ActionBase> action, ViewModelBase vm)> _messages = new List<(Action<ActionBase> action, ViewModelBase vm)>();
        private volatile bool _isDisposed;
        private readonly object _locker = new object();

        protected readonly CancellationTokenSource QueryQts = new CancellationTokenSource();

        protected readonly ILogger Logger = Log.Logger;

        private ViewModelBase _parentViewModel;
        public ViewModelBase ParentViewModel
        {
            get => _parentViewModel;
            set
            {
                _parentViewModel = value;
                OnParentViewModelDefined();
            }
        }

        protected void Unsubscribe(ViewModelBase viewModelBase)
        {
            if (viewModelBase == null)
                return;

            lock (_locker)
            {
                if (_messages.Any(m => m.vm == viewModelBase))
                {
                    var message = _messages.First(m => m.vm == viewModelBase);
                    _messages.Remove(message);
                }
            }
        }

        protected void MessageInvoke(ActionBase action)
        {
            if (_isDisposed)
                return;

            (Action<ActionBase> action, ViewModelBase vm)[] copy;
            lock (_locker)
            {
                copy = _messages.ToArray();
            }

            foreach (var message in copy)
            {
                if (_isDisposed)
                    return;

                message.action?.Invoke(action);
            }
        }

        private void ClearMessages()
        {
            lock (_locker)
            {
                _messages.Clear();
            }
        }

        public virtual Task ViewLoadedAsync()
        {
            return Task.FromResult(0); //for Net 4.6: Task.CompletedTask;
        }

        public void Subscribe([NotNull] Action<ActionBase> onAction, ViewModelBase sourceViewModel)
        {
            if (_isDisposed)
                return;

            lock (_locker)
            {
                if (_messages.Any(m => ReferenceEquals(m.vm, sourceViewModel)))
                {
                    Debugger.Break();
                    return;
                }

                _messages.Add((onAction, sourceViewModel));
            }
        }

        protected virtual void OnParentViewModelDefined()
        {

        }

        public virtual Task InitializeAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.FromResult(0); //for Net 4.6: Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _isDisposed = true;

            ClearMessages();

            try
            {
                QueryQts.Cancel();
                QueryQts.Dispose();
            }
            catch
            {
                // ignored
            }
            ParentViewModel = null;
        }
    }
}