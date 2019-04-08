using VectorEditor.Core.MVVM.Base;
using VectorEditor.Domain.Data;

namespace VectorEditor.ViewModels.Actions
{
    public class ShapeOnCanvasSelectedAction : ActionBase
    {
        public Shape SelectedShape { get; }

        public ShapeOnCanvasSelectedAction(Shape selectedShape)
        {
            SelectedShape = selectedShape;
        }
    }
}