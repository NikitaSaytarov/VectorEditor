using System.Windows.Controls.Primitives;
using VectorEditor.Domain.Data;

namespace DiagramDesigner
{
    public class MoveThumb : Thumb
    {
        private bool _isSignal;

        public MoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
            DragStarted += OnDragStarted;
            DragCompleted += OnDragCompleted;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var rectangle = (RectangleShape)DataContext;
            rectangle.OperationCompleted();

            _isSignal = false;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            //ignored
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!_isSignal)
            {
                var rectangle = (RectangleShape)DataContext;
                rectangle.OperationStart();

                _isSignal = true;
            }

            var dataContext = (RectangleShape)DataContext;
            double left = dataContext.X;
            double top = dataContext.Y;

            dataContext.X = left + e.HorizontalChange;
            dataContext.Y = top + e.VerticalChange;
        }
    }
}
