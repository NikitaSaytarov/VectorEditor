using System;
using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.Domain.Data
{
    public class Vertex : NotifyObject, ICloneable
    {
        public event EventHandler StateChanged;
        public event EventHandler StateChanging;

        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private double _y;
        public double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        public void OperationStarted()
        {
            OnStateChanging();
        }

        public void OperationCompleted()
        {
            OnStateChanged();
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStateChanging()
        {
            StateChanging?.Invoke(this, EventArgs.Empty);
        }

        public object Clone()
        {
            return new Vertex()
            {
                X = X,
                Y = Y
            };
        }
    }
}