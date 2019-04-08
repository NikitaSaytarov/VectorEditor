using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using VectorEditor.Domain.Data;

namespace DiagramDesigner
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += ResizeThumb_DragDelta;
            DragStarted += OnDragStarted;
            DragCompleted += OnDragCompleted;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var rectangle = (RectangleShape)DataContext;
            rectangle.OperationCompleted();
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var rectangle = (RectangleShape)DataContext;
            rectangle.OperationStart();
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var rectangle = (RectangleShape)DataContext;
            if (rectangle != null)
            {
                double deltaVertical, deltaHorizontal;
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, rectangle.Height);
                        rectangle.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, rectangle.Height);
                        rectangle.Height -= deltaVertical;
                        rectangle.Y += deltaVertical;
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, rectangle.Width);
                        rectangle.Width -= deltaHorizontal;
                        rectangle.X += deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, rectangle.Width);
                        rectangle.Width -= deltaHorizontal;
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
