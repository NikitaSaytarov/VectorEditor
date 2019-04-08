using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace VectorEditor.Services.CanvasOperation
{
    public interface ICanvasOperationService : INotifyPropertyChanged
    {
        event EventHandler SizeChanging;
        Size CanvasSize { get; }
        ICommand IncreaseCanvasSizeCommand { get; }
        ICommand DecreaseCanvasSizeCommand { get; }
        void ChangeSize(Size newSize);
    }
}