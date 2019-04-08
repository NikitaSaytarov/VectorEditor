using System.Threading;
using Serilog;

namespace VectorEditor.Core.MVVM.Base
{
    public abstract class ModelBase : NotifyObject, IModel
    {
        protected readonly ILogger Logger = Log.Logger;
        protected bool IsDisposed { get; private set; }

        protected readonly CancellationTokenSource QueryToken = new CancellationTokenSource();

        public virtual void Dispose()
        {
            IsDisposed = true;

            try
            {
                QueryToken.Cancel();
                QueryToken.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}