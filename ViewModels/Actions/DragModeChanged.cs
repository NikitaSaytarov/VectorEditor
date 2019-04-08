using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.ViewModels.Actions
{
    public class DragModeChanged : ActionBase
    {
        public bool IsDragMode { get; }

        public DragModeChanged(bool isDragMode)
        {
            IsDragMode = isDragMode;
        }
    }
}