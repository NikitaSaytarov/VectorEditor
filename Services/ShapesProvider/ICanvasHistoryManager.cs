using System;
using VectorEditor.Domain.Data;
using VectorEditor.Domain.Data.Persistence;

namespace VectorEditor.Services.ShapeProvider
{
    public interface ICanvasHistoryManager : IShapeProvider
    {
        void MakeSnapshot();
        CanvasState GetSnapshot();
        void UndoState();

        event EventHandler<int> TakeSnapshotChanged;
        event EventHandler<int> UndoChangesChanged;

        void LoadCanvasState(CanvasState canvasState);
    }
}