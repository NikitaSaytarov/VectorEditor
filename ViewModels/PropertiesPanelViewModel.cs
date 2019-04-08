using System;
using System.Windows.Input;
using JetBrains.Annotations;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Core.MVVM.Commands;
using VectorEditor.Domain.Data;
using VectorEditor.Services.AppColorsService;
using VectorEditor.ViewModels.Actions;

namespace VectorEditor.ViewModels
{
    public sealed class PropertiesPanelViewModel : ViewModelBase<Shape>
    {
        public AppColorService AppColorService { get; }

        public ICommand ClickCommand { get; }
        public ICommand ShapePropertyChangedCommand { get; }

        public PropertiesPanelViewModel(Shape model, [NotNull] AppColorService appColorService)
            : base(model)
        {
            AppColorService = appColorService ?? throw new ArgumentNullException(nameof(appColorService));
            ClickCommand = new RelayCommand(Click);
            ShapePropertyChangedCommand = new RelayCommand(ShapePropertyChanged);
        }

        private void ShapePropertyChanged()
        {
            Model.OperationStart();
            Model.OperationCompleted();
        }

        private void Click()
        {
            MessageInvoke(new CancelDrawingShapeAction());
        }
    }
}