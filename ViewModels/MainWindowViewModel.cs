using System;
using JetBrains.Annotations;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Domain.Data;
using VectorEditor.Services.ShapeProvider;
using VectorEditor.Services.VectorLoader;
using VectorEditor.Setup;
using VectorEditor.ViewModels.Actions;

namespace VectorEditor.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ICanvasHistoryManager _canvasHistoryManager;
        private readonly IVectorCanvasLoader _vectorCanvasLoader;

        private PropertiesPanelViewModel _propertiesPanelViewModel;
        public PropertiesPanelViewModel PropertiesPanelViewModel
        {
            get => _propertiesPanelViewModel;
            private set => SetProperty(ref _propertiesPanelViewModel, value);
        }

        private CanvasViewModel _canvasViewModel;
        public CanvasViewModel CanvasViewModel
        {
            get => _canvasViewModel;
            private set => SetProperty(ref _canvasViewModel, value);
        }

        private CommandPanelViewModel _commandPanelViewModel;
        public CommandPanelViewModel CommandPanelViewModel
        {
            get => _commandPanelViewModel;
            private set => SetProperty(ref _commandPanelViewModel, value);
        }

        public MainWindowViewModel([NotNull] IViewModelFactory viewModelFactory,
            [NotNull] ICanvasHistoryManager canvasHistoryManager,
            [NotNull] IVectorCanvasLoader vectorCanvasLoader)
        {
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
            _canvasHistoryManager = canvasHistoryManager ?? throw new ArgumentNullException(nameof(canvasHistoryManager));
            _vectorCanvasLoader = vectorCanvasLoader ?? throw new ArgumentNullException(nameof(vectorCanvasLoader));

            Initialize();
        }

        private void Initialize()
        {
            CommandPanelViewModel = _viewModelFactory.CreateViewModel<CommandPanelViewModel>();
            CommandPanelViewModel.ParentViewModel = this;
            CommandPanelViewModel.Subscribe(OnAction, this);

            CanvasViewModel = _viewModelFactory.CreateViewModel<CanvasViewModel>();
            CanvasViewModel.ParentViewModel = this;
            CanvasViewModel.Subscribe(OnAction, this);

            _canvasHistoryManager.TakeSnapshotChanged += OnTakeSnapshotChanged;
            _canvasHistoryManager.UndoChangesChanged += OnTakeSnapshotChanged;
        }

        private void OnTakeSnapshotChanged(object sender, int canvasHistorySize)
        {
            MessageInvoke(new CanvasHistoryChangedAction(canvasHistorySize));
        }

        private void OnAction(ActionBase action)
        {
            switch (action)
            {
                case AddPolylineAction addPolylineAction:
                    MessageInvoke(addPolylineAction);
                    break;
                case AddRectangleAction addRectangleAction:
                    MessageInvoke(addRectangleAction);
                    break;
                case EmptyCanvasAction emptyCanvasAction:
                    MessageInvoke(emptyCanvasAction);
                    break;
                case ExitAction _:
                    OnExit();
                    break;
                case CanvasScaleRangeChangedAction scaleRangeChangedAction:
                    MessageInvoke(scaleRangeChangedAction);
                    break;
                case LoadVectorAction _:
                    OnLoadVector();
                    break;
                case RemoveSelectedShapeAction removeSelectedShapeAction:
                    MessageInvoke(removeSelectedShapeAction);
                    break;
                case SaveRasterAction saveRasterAction:
                    MessageInvoke(saveRasterAction);
                    break;
                case SaveVectorAction _:
                    OnSaveVector();
                    break;
                case CancelDrawingShapeAction cancelDrawingShapeAction:
                    MessageInvoke(cancelDrawingShapeAction);
                    break;
                case UndoOperationAction _:
                    _canvasHistoryManager.UndoState();
                    break;
                case DragModeChanged dragModeChanged:
                    MessageInvoke(dragModeChanged);
                    break;
                case PrintCanvasAction printCanvasAction:
                    MessageInvoke(printCanvasAction);
                    break;
                case ShapeOnCanvasSelectedAction shapeOnCanvasSelectedAction:
                    if (shapeOnCanvasSelectedAction.SelectedShape != null)
                        OnShapeOnCanvasSelected(shapeOnCanvasSelectedAction.SelectedShape);
                    else
                    {
                        ReleasePropertiesPanelViewModel();
                        PropertiesPanelViewModel = _viewModelFactory.CreateViewModel<PropertiesPanelViewModel>(new EmptyShape());
                        PropertiesPanelViewModel.Subscribe(OnAction, this);
                    }

                    MessageInvoke(shapeOnCanvasSelectedAction);
                    break;
            }
        }

        private async void OnLoadVector()
        {
            var canvasState = await _vectorCanvasLoader.LoadVectorAsync(QueryQts.Token);

            if (canvasState != null)
            {
                _canvasHistoryManager.LoadCanvasState(canvasState);
                MessageInvoke(new FillCanvasAction());
            }
        }

        private async void OnSaveVector()
        {
            await _vectorCanvasLoader.SaveVectorAsync(QueryQts.Token);
        }

        private void OnShapeOnCanvasSelected([NotNull] Shape selectedShape)
        {
            if (selectedShape == null)
                throw new ArgumentNullException(nameof(selectedShape));

            ReleasePropertiesPanelViewModel();
            PropertiesPanelViewModel = _viewModelFactory.CreateViewModel<PropertiesPanelViewModel>(selectedShape);
            PropertiesPanelViewModel.Subscribe(OnAction, this);
        }

        private void OnExit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public override void Dispose()
        {
            base.Dispose();

            _canvasHistoryManager.TakeSnapshotChanged += OnTakeSnapshotChanged;

            Unsubscribe(CommandPanelViewModel);
            CommandPanelViewModel.Dispose();

            Unsubscribe(CanvasViewModel);
            CanvasViewModel.Dispose();

            ReleasePropertiesPanelViewModel();
        }

        private void ReleasePropertiesPanelViewModel()
        {
            if (PropertiesPanelViewModel != null)
            {
                Unsubscribe(PropertiesPanelViewModel);
                PropertiesPanelViewModel.Dispose();
            }
        }
    }
}