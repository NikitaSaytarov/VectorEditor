using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.ViewModels.Actions
{
    public class CanvasScaleRangeChangedAction : ActionBase
    {
        public double CanvasScale { get; }

        public CanvasScaleRangeChangedAction(double canvasScaleRange)
        {
            CanvasScale = canvasScaleRange;
        }
    }
}