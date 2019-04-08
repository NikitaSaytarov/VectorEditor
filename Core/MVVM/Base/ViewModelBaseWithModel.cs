namespace VectorEditor.Core.MVVM.Base
{
    public abstract class ViewModelBase<T> : ViewModelBase
        where T : IModel
    {
        public T Model { get; private set; }

        protected ViewModelBase(T model)
        {
            Model = model;
        }
    }

}