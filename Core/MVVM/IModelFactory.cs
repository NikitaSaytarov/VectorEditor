using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.Core.MVVM
{
    public interface IModelFactory
    {
        T CreateModel<T>()
            where T : ModelBase;
        T CreateModel<TParam, T>(TParam parameter)
            where T : ModelBase;
    }
}