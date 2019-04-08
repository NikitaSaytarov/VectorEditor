using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using JetBrains.Annotations;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Core.MVVM.Commands;
using VectorEditor.Domain.Data;
using VectorEditor.Services.CanvasOperation;
using VectorEditor.ViewModels.Actions;

// ReSharper disable IdentifierTypo

namespace VectorEditor.ViewModels
{
    public sealed class CommandPanelViewModel : ViewModelBase
    {
        public ICanvasOperationService CanvasOperationService { get; }
        public ObservableCollection<double> CanvasScaleRange { get; }

        private bool _isRemoveSelectedShapeVisible;
        public bool IsRemoveSelectedShapeVisible
        {
            get => _isRemoveSelectedShapeVisible;
            set => SetProperty(ref _isRemoveSelectedShapeVisible, value);
        }

        private bool _isUndoOperationVisible;
        public bool IsUndoOperationVisible
        {
            get => _isUndoOperationVisible;
            set => SetProperty(ref _isUndoOperationVisible, value);
        }

        private bool _isSaveVectorVisible;
        public bool IsSaveVectorVisible
        {
            get => _isSaveVectorVisible;
            set => SetProperty(ref _isSaveVectorVisible, value);
        }

        private bool _isSaveRusterVisible;
        public bool IsSaveRusterVisible
        {
            get => _isSaveRusterVisible;
            set => SetProperty(ref _isSaveRusterVisible, value);
        }

        private bool _isEmptyCanvas;
        public bool IsEmptyCanvas
        {
            get => _isEmptyCanvas;
            set => SetProperty(ref _isEmptyCanvas, value);
        }

        private bool _isDragMode;
        public bool IsDragMode
        {
            get => _isDragMode;
            set => SetProperty(ref _isDragMode, value);
        }


        private double _selectedCanvasScale;
        public double SelectedCanvasScale
        {
            get => _selectedCanvasScale;
            set => SetProperty(ref _selectedCanvasScale, value);
        }

        public ICommand AddRectangleCommand { get; }
        public ICommand AddPolylineShapeCommand { get; }
        public ICommand RemoveSelectedShapeCommand { get; }
        public ICommand SaveVectorCommand { get; }
        public ICommand LoadVectorCommand { get; }
        public ICommand SaveRastCommand { get; }
        public ICommand UndoOperationCommand { get; }
        public ICommand ExitCommandCommand { get; }
        public ICommand IsDragModeCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand ClickCommand { get; }

        public CommandPanelViewModel([NotNull] ICanvasOperationService canvasOperationService)
        {
            CanvasOperationService = canvasOperationService ?? throw new ArgumentNullException(nameof(canvasOperationService));

            AddRectangleCommand = new RelayCommand(AddRectangle);
            AddPolylineShapeCommand = new RelayCommand(AddPolylineShape);
            RemoveSelectedShapeCommand = new RelayCommand(RemoveSelectedShape);
            SaveVectorCommand = new RelayCommand(SaveVector);
            LoadVectorCommand = new RelayCommand(LoadVector);
            SaveRastCommand = new RelayCommand(SaveRast);
            UndoOperationCommand = new RelayCommand(UndoOperation);
            ExitCommandCommand = new RelayCommand(ExitCommand);
            IsDragModeCommand = new RelayCommand(IsDragModeAction);
            PrintCommand = new RelayCommand(Print);
            ClickCommand = new RelayCommand(Click);

            IsEmptyCanvas = true;

            CanvasScaleRange = new ObservableCollection<double>();

            SelectedCanvasScale = 1d;
            for (double i = 0.1d; i <= 3.1d; i += 0.1d)
            {
                CanvasScaleRange.Add(Math.Round(i, 2));
            }
        }

        private void Click()
        {
            MessageInvoke(new CancelDrawingShapeAction());
        }

        private void Print()
        {
            MessageInvoke(new PrintCanvasAction());
        }

        private void IsDragModeAction()
        {
            IsDragMode = !IsDragMode;
            MessageInvoke(new DragModeChanged(IsDragMode));
        }

        protected override void OnParentViewModelDefined()
        {
            base.OnParentViewModelDefined();
            ParentViewModel?.Subscribe(OnAction, this);
        }

        private void OnAction(ActionBase action)
        {
            switch (action)
            {
                case EmptyCanvasAction _:
                    IsEmptyCanvas = true;
                    break;
                case FillCanvasAction _:
                    IsEmptyCanvas = false;
                    break;
                case ShapeOnCanvasSelectedAction shapeOnCanvasSelectedAction:
                    OnShapeOnCanvasSelected(shapeOnCanvasSelectedAction.SelectedShape);
                    break;
                case CanvasHistoryChangedAction canvasHistoryChangedAction:
                    OnCanvasHistoryChanged(canvasHistoryChangedAction.CanvasHistorySize);
                    break;
            }
        }

        private void OnCanvasHistoryChanged(int canvasHistorySize)
        {
            IsUndoOperationVisible = canvasHistorySize != 0;

            if (canvasHistorySize == 0)
                IsEmptyCanvas = true;
        }

        private void OnShapeOnCanvasSelected(Shape selectedShape)
        {
            IsRemoveSelectedShapeVisible = selectedShape != null;
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsEmptyCanvas))
            {
                IsRemoveSelectedShapeVisible = !IsEmptyCanvas;
                IsSaveRusterVisible = !IsEmptyCanvas;
                IsSaveVectorVisible = !IsEmptyCanvas;
            }
            if (propertyName == nameof(SelectedCanvasScale))
            {
                MessageInvoke(new CanvasScaleRangeChangedAction(SelectedCanvasScale));
            }
        }

        private void UndoOperation()
        {
            MessageInvoke(new UndoOperationAction());
        }

        private void ExitCommand()
        {
            MessageInvoke(new ExitAction());
        }

        private void SaveRast()
        {
            MessageInvoke(new SaveRasterAction());
        }

        private void LoadVector()
        {
            MessageInvoke(new LoadVectorAction());
        }

        private void SaveVector()
        {
            MessageInvoke(new SaveVectorAction());
        }

        private void RemoveSelectedShape()
        {
            MessageInvoke(new RemoveSelectedShapeAction());
        }

        private void AddPolylineShape()
        {
            IsEmptyCanvas = false;
            MessageInvoke(new AddPolylineAction());
        }

        private void AddRectangle()
        {
            IsEmptyCanvas = false;
            MessageInvoke(new AddRectangleAction());
        }
    }
}