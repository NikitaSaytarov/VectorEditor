using System;
using JetBrains.Annotations;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Domain.Data.Persistence;

namespace VectorEditor.ViewModels.Actions
{
    public class CanvasVectorLoadedAction : ActionBase
    {
        public CanvasState CanvasState { get; }

        public CanvasVectorLoadedAction([NotNull] CanvasState canvasState)
        {
            CanvasState = canvasState ?? throw new ArgumentNullException(nameof(canvasState));
        }
    }
}