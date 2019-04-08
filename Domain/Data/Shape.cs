using System;
using System.Windows.Media;
using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.Domain.Data
{
    public abstract class Shape : ModelBase, IEquatable<Shape>
    {
        public event EventHandler StateChanged;
        public event EventHandler StateChanging;
        public event EventHandler Selected;

        private double _borderThickness;
        public double BorderThickness
        {
            get => _borderThickness;
            set
            {
                OnStateChanging();
                SetProperty(ref _borderThickness, value);
                OnStateChanged();
            }
        }

        private Brush _borderBrush;
        public Brush BorderBrush
        {
            get => _borderBrush;
            set
            {
                OnStateChanging();
                SetProperty(ref _borderBrush, value);
                OnStateChanged();
            }
        }

        private Brush _backgroundBrush;
        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                OnStateChanging();
                SetProperty(ref _backgroundBrush, value);
                OnStateChanged();
            }
        }

        private string _guid;
        public string Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }

        protected Shape()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public void OperationStart()
        {
            OnStateChanging();
            OnSelected();
        }

        public void OperationCompleted()
        {
            OnStateChanged();
        }

        protected void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStateChanging()
        {
            StateChanging?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSelected()
        {
            Selected?.Invoke(this, EventArgs.Empty);
        }

        public bool Equals(Shape other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.Guid != Guid)
                return false;
            return _borderThickness.Equals(other._borderThickness)
                   && Equals(_borderBrush, other._borderBrush)
                   && Equals(_backgroundBrush, other._backgroundBrush);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            if (((Shape) obj).Guid != Guid)
                return false;
            return Equals((Shape)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _borderThickness.GetHashCode();
                hashCode = (hashCode * 397) ^ (_borderBrush != null ? _borderBrush.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_backgroundBrush != null ? _backgroundBrush.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}