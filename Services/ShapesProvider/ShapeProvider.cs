using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using JetBrains.Annotations;
using Mapster;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Domain.Data;
using VectorEditor.Domain.Data.Persistence;
using VectorEditor.Services.CanvasOperation;
using VectorEditor.Services.SerializeService.Binary;

namespace VectorEditor.Services.ShapeProvider
{
    public sealed class ShapeProvider : NotifyObject, ICanvasHistoryManager
    {
        private readonly IBinarySerializer _binarySerializer;
        private readonly ByteCompressor.ByteCompressor _byteCompressor;
        private readonly ICanvasOperationService _canvasOperationService;
        private readonly ObservableCollection<Shape> _shapes;
        public ReadOnlyObservableCollection<Shape> Shapes { get; }

        public Stack<byte[]> CanvasHistory { get; }

        public event EventHandler<int> TakeSnapshotChanged;
        public event EventHandler<int> UndoChangesChanged;
        public event EventHandler<Shape> ShapeSelected;

        public ShapeProvider([NotNull] IBinarySerializer binarySerializer,
            [NotNull] ByteCompressor.ByteCompressor byteCompressor,
            [NotNull] ICanvasOperationService canvasOperationService)
        {
            _binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
            _byteCompressor = byteCompressor ?? throw new ArgumentNullException(nameof(byteCompressor));
            _canvasOperationService = canvasOperationService ?? throw new ArgumentNullException(nameof(canvasOperationService));

            CanvasHistory = new Stack<byte[]>();

            _shapes = new ObservableCollection<Shape>();
            Shapes = new ReadOnlyObservableCollection<Shape>(_shapes);

            _canvasOperationService.SizeChanging += CanvasOperationServiceOnSizeChanging;
        }

        private void CanvasOperationServiceOnSizeChanging(object sender, EventArgs e)
        {
            MakeSnapshot();
        }

        public void RemoveShape(Shape shape)
        {
            MakeSnapshot();

            shape.StateChanging -= ShapeOnStateChanging;
            shape.Selected -= ShapeOnSelected;
            _shapes.Remove(shape);
        }

        public void AddShape(Shape shape)
        {
            MakeSnapshot();

            shape.StateChanging += ShapeOnStateChanging;
            shape.Selected += ShapeOnSelected;
            _shapes.Add(shape);
        }

        public void LoadCanvasState(CanvasState canvasState)
        {
            MakeSnapshot();
            ClearFromShapes();
            LoadShapes(canvasState);
        }

        public void MakeSnapshot()
        {
            var snapshot = _shapes.Adapt<CanvasState>();
            snapshot.CanvasSize = _canvasOperationService.CanvasSize.Adapt<SizeInfo>();

            var snapshotBytes = _binarySerializer.Serialize(snapshot);
            var snapshotBytesCompressed = _byteCompressor.Compress(snapshotBytes);

            CanvasHistory.Push(snapshotBytesCompressed);

            OnCanvasHistoryChanged(CanvasHistory.Count);
        }

        public CanvasState GetSnapshot()
        {
            return _shapes.Adapt<CanvasState>();
        }

        private void ShapeOnStateChanging(object sender, EventArgs e)
        {
            MakeSnapshot();
        }

        public void UndoState()
        {
            var canvasStateBytes = CanvasHistory.Pop();
            var canvasStateBytesDecompressed = _byteCompressor.Decompress(canvasStateBytes);
            var canvasState = _binarySerializer.Deserialize<CanvasState>(canvasStateBytesDecompressed);
            _canvasOperationService.ChangeSize(new Size(canvasState.CanvasSize.Width, canvasState.CanvasSize.Height));

            ClearFromShapes();
            LoadShapes(canvasState);

            OnUndoChangesChanged(CanvasHistory.Count);
        }

        private void LoadShapes(CanvasState canvasState)
        {
            var shapesCollection = canvasState.Adapt<ObservableCollection<Shape>>();

            foreach (var shape in shapesCollection)
            {
                shape.StateChanging += ShapeOnStateChanging;
                shape.Selected += ShapeOnSelected;
                _shapes.Add(shape);
            }
        }

        private void ClearFromShapes()
        {
            foreach (var shape in _shapes)
            {
                shape.StateChanging -= ShapeOnStateChanging;
                shape.Selected -= ShapeOnSelected;
            }
            _shapes.Clear();
        }

        private void ShapeOnSelected(object sender, EventArgs e)
        {
            OnShapeSelected((Shape)sender);
        }

        private void OnCanvasHistoryChanged(int stackSize)
        {
            TakeSnapshotChanged?.Invoke(this, stackSize);
        }

        private void OnUndoChangesChanged(int e)
        {
            UndoChangesChanged?.Invoke(this, e);
        }

        private void OnShapeSelected(Shape e)
        {
            ShapeSelected?.Invoke(this, e);
        }
    }
}