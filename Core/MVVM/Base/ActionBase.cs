namespace VectorEditor.Core.MVVM.Base
{
    public abstract class ActionBase
    {
        public ViewModelBase Destination { get; set; }
        public bool Handled { get; set; }
    }
}