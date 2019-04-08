using System;
using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.Setup
{
    public interface IViewModelFactory
    {
        ViewModelBase CreateViewModel(Type viewModelType);
        T CreateViewModel<T>(ModelBase model = null)
            where T : ViewModelBase;
    }
}