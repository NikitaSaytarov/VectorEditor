using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.ViewModels.Actions
{
    public class CanvasHistoryChangedAction : ActionBase
    {
        public int CanvasHistorySize { get; }

        public CanvasHistoryChangedAction(int canvasHistorySize)
        {
            CanvasHistorySize = canvasHistorySize;
        }
    }
}