using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using VectorEditor.Core.MVVM;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Core.MVVM.Commands;
using VectorEditor.Domain.Data;
using VectorEditor.Services.AppColorsService;
using VectorEditor.Services.CanvasOperation;
using VectorEditor.Services.RasterLoader;
using VectorEditor.Services.ShapeProvider;
using VectorEditor.ViewModels.Actions;

namespace VectorEditor.ViewModels
{
    public sealed class CanvasScaleRate : NotifyObject
    {
        private double _scale;
        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }
    }

    public sealed class CanvasViewModel : ViewModelBase
    {
        public ICanvasOperationService CanvasOperationService { get; }
        private Canvas _canvasView;

        private readonly IModelFactory _modelFactory;
        private readonly IShapeProvider _shapeProvider;
        private readonly AppColorService _appColorService;
        private readonly IRasterLoader _rasterLoader;

        public ReadOnlyObservableCollection<Shape> Shapes => _shapeProvider.Shapes;

        private Shape _selectedShape;
        public Shape SelectedShape
        {
            get => _selectedShape;
            private  set => SetProperty(ref _selectedShape, value);
        }

        private bool _isDragMode;
        public bool IsDragMode
        {
            get => _isDragMode;
            set => SetProperty(ref _isDragMode, value);
        }

        private bool _isDrawingShape;
        public bool IsDrawingShape
        {
            get => _isDrawingShape;
            private set => SetProperty(ref _isDrawingShape, value);
        }

        private PolylineShape _drawingPolyline;
        public PolylineShape DrawingPolyline
        {
            get => _drawingPolyline;
            private set => SetProperty(ref _drawingPolyline, value);
        }

        private RectangleShape _drawingRectangle;
        public RectangleShape DrawingRectangle
        {
            get => _drawingRectangle;
            private set => SetProperty(ref _drawingRectangle, value);
        }

        private CanvasScaleRate _canvasScaleRate;
        public CanvasScaleRate CanvasScaleRate
        {
            get => _canvasScaleRate;
            set => SetProperty(ref _canvasScaleRate, value);
        }

        public ICommand SelectedShapeCommand { get; }
        public ICommand UnselectShapeCommand { get; }

        public CanvasViewModel([NotNull] IModelFactory modelFactory,
            [NotNull] IShapeProvider shapeProvider,
            [NotNull] AppColorService appColorService,
            [NotNull] IRasterLoader rasterLoader,
            [NotNull] ICanvasOperationService canvasOperationService)
        {
            CanvasOperationService = canvasOperationService ?? throw new ArgumentNullException(nameof(canvasOperationService));
            _modelFactory = modelFactory ?? throw new ArgumentNullException(nameof(modelFactory));
            _shapeProvider = shapeProvider ?? throw new ArgumentNullException(nameof(shapeProvider));
            _appColorService = appColorService ?? throw new ArgumentNullException(nameof(appColorService));
            _rasterLoader = rasterLoader ?? throw new ArgumentNullException(nameof(rasterLoader));

            ((ICanvasHistoryManager)_shapeProvider).UndoChangesChanged += OnUndoChangesChanged;
            _shapeProvider.ShapeSelected += ShapeProviderOnShapeSelected;

            UnselectShapeCommand = new RelayCommand<MouseButtonEventArgs>(UnselectShape);
            SelectedShapeCommand = new RelayCommand<MouseButtonEventArgs>(SelectedShapeAction);

            CanvasScaleRate = new CanvasScaleRate()
            {
                Scale = 1d
            };

            SelectedShape = new EmptyShape();
        }

        private void ShapeProviderOnShapeSelected(object sender, Shape shape)
        {
            SelectedShape = shape;
        }

        private void OnUndoChangesChanged(object sender, int e)
        {
            if (SelectedShape != null)
                SelectedShape = _shapeProvider.Shapes.FirstOrDefault(s => string.Equals(s.Guid, SelectedShape.Guid, StringComparison.InvariantCultureIgnoreCase));
        }

        private void UnselectShape(MouseButtonEventArgs args)
        {
            if(!(args.OriginalSource is Canvas))
                return;

            SelectedShape = null;
        }

        private void SelectedShapeAction(MouseButtonEventArgs args)
        {
            var dataContext = ((FrameworkElement)args.OriginalSource).DataContext;
            if (dataContext is Shape shape)
            {
                SelectedShape = shape;
            }
            else if (dataContext is Vertex)
            {
                var vertex = (Vertex)dataContext;
                var selectedShape = _shapeProvider.Shapes.OfType<PolylineShape>().First(l => l.Vertices.Contains(vertex));
                SelectedShape = selectedShape;
            }
        }

        protected override void OnParentViewModelDefined()
        {
            base.OnParentViewModelDefined();
            ParentViewModel?.Subscribe(OnAction, this);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(SelectedShape))
            {
                MessageInvoke(new ShapeOnCanvasSelectedAction(SelectedShape));
            }

            if (propertyName == nameof(DrawingPolyline) || propertyName == nameof(DrawingRectangle))
            {
                if (DrawingPolyline == null && DrawingRectangle == null)
                {
                    IsDrawingShape = false;
                    return;
                }

                IsDrawingShape = true;
            }
        }

        private void OnAction(ActionBase action)
        {
            switch (action)
            {
                case CanvasScaleRangeChangedAction scaleRangeChangedAction:
                    OnCanvasScaleRangeChanged(scaleRangeChangedAction.CanvasScale);
                    break;
                case AddPolylineAction addPolylineAction:
                    OnAddPolyline();
                    break;
                case AddRectangleAction _:
                    OnAddRectangle();
                    break;
                case EmptyCanvasAction emptyCanvasAction:
                    break;
                case SaveRasterAction saveRasterAction:
                    OnSaveRaster();
                    break;
                case CancelDrawingShapeAction cancelDrawingShapeAction:
                    OnCancelDrawingShape();
                    break;
                case ExitAction exitAction:
                    break;
                case PrintCanvasAction printCanvasAction:
                    OnPrintCanvas();
                    break;
                case LoadVectorAction loadVectorAction:
                    break;
                case RemoveSelectedShapeAction _:
                    OnRemoveSelectedShape();
                    break;
                case SaveVectorAction saveVectorAction:
                    break;
                case UndoOperationAction _:
                    break;
                case DragModeChanged dragModeChanged:
                    OnDragModeChanged(dragModeChanged.IsDragMode);
                    break;
            }
        }

        private void OnCancelDrawingShape()
        {
            if (DrawingPolyline != null)
            {
                if (DrawingPolyline.Vertices.Any())
                {
                    CompleteDrawingPolyline();
                }
                else
                {
                    DrawingPolyline = null;
                }
            }

            if (DrawingRectangle != null)
            {
                DrawingRectangle = null;
            }
        }

        private void OnCanvasScaleRangeChanged(double canvasScale)
        {
            CanvasScaleRate = new CanvasScaleRate()
            {
                Scale = canvasScale
            };
        }

        private void OnAddPolyline()
        {
            DrawingPolyline = new PolylineShape();
        }

        private async void OnPrintCanvas()
        {
            await _rasterLoader.PrintCanvasAsync(_canvasView, QueryQts.Token);
        }

        private async void OnSaveRaster()
        {
            await _rasterLoader.SaveRasterCanvasAsync(_canvasView, QueryQts.Token);
        }

        private void OnDragModeChanged(bool isDragMode)
        {
            IsDragMode = isDragMode;
        }

        private void OnRemoveSelectedShape()
        {
            if (SelectedShape == null || SelectedShape is EmptyShape)
                throw new InvalidOperationException("SelectedShape == null");

            _shapeProvider.RemoveShape(SelectedShape);

            if (_shapeProvider.Shapes.Count == 0)
                MessageInvoke(new EmptyCanvasAction());
        }

        private void OnAddRectangle()
        {
            DrawingRectangle = new RectangleShape();
        }

        public override void Dispose()
        {
            base.Dispose();
            ((ICanvasHistoryManager)_shapeProvider).UndoChangesChanged -= OnUndoChangesChanged;
            _shapeProvider.ShapeSelected -= ShapeProviderOnShapeSelected;
        }

        public void InitializeCanvas([NotNull] Canvas canvas)
        {
            if (_canvasView != null)
                throw new InvalidOperationException();

            _canvasView = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        public void CompleteDrawingPolyline()
        {
            try
            {
                if (!DrawingPolyline.Vertices.Any())
                    return;

                DrawingPolyline.BorderThickness = 2;
                DrawingPolyline.BorderBrush = _appColorService.Colors.First();
                DrawingPolyline.BackgroundBrush = _appColorService.Colors[1];

                _shapeProvider.AddShape(DrawingPolyline);
                SelectedShape = DrawingPolyline;
            }
            finally
            {
                DrawingPolyline = null;
            }
        }

        public void CompleteDrawingRectangle(Point mouseDownPos, Point mouseUpPos)
        {
            try
            {
                var rectangle = _modelFactory.CreateModel<RectangleShape>();

                rectangle.X = Math.Min(mouseDownPos.X, mouseUpPos.X);
                rectangle.Y = Math.Min(mouseDownPos.Y, mouseUpPos.Y);
                rectangle.Width = Math.Abs(mouseDownPos.X - mouseUpPos.X);
                rectangle.Height = Math.Abs(mouseDownPos.Y - mouseUpPos.Y);
                rectangle.BorderThickness = 2;
                rectangle.BorderBrush = _appColorService.Colors.First();
                rectangle.BackgroundBrush = _appColorService.Colors[5];

                _shapeProvider.AddShape(rectangle);
                SelectedShape = rectangle;
            }
            finally
            {
                DrawingRectangle = null;
            }
        }
    }
}