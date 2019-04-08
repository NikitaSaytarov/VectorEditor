using System;
using System.Windows;
using System.Windows.Input;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Core.MVVM.Commands;

namespace VectorEditor.Services.CanvasOperation
{
    public sealed class CanvasOperationService : NotifyObject, ICanvasOperationService
    {
        public event EventHandler SizeChanging;

        private Size _canvasSize;
        public Size CanvasSize
        {
            get => _canvasSize;
            private set => SetProperty(ref _canvasSize, value);
        }

        public ICommand IncreaseCanvasSizeCommand { get; }
        public ICommand DecreaseCanvasSizeCommand { get; }

        public CanvasOperationService()
        {
            IncreaseCanvasSizeCommand = new RelayCommand(IncreaseCanvasSize);
            DecreaseCanvasSizeCommand = new RelayCommand(DecreaseCanvasSize);

            CanvasSize = new Size(1920, 1080);
        }

        private void DecreaseCanvasSize()
        {
            double width;
            double height;

            if (_canvasSize.Width - 100 < 1920)
            {
                width = 1920d;
            }
            else
            {
                width = _canvasSize.Width - 100;
            }

            if (_canvasSize.Height - 100 < 1080)
            {
                height = 1080d;
            }
            else
            {
                height = _canvasSize.Height - 100;
            }

            OnSizeChanging();
            CanvasSize = new Size(width, height);

        }

        public void ChangeSize(Size newSize)
        {
            if (newSize.Width < 1920 || newSize.Height < 1080)
                throw new InvalidOperationException();

            CanvasSize = newSize;
        }

        private void IncreaseCanvasSize()
        {
            OnSizeChanging();
            CanvasSize = new Size(_canvasSize.Width + 100, _canvasSize.Height + 100);
        }

        private void OnSizeChanging()
        {
            SizeChanging?.Invoke(this, EventArgs.Empty);
        }
    }
}