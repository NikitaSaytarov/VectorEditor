using System;
using System.Linq;
using System.Windows.Controls.Primitives;
using VectorEditor.Domain.Data;

namespace DiagramDesigner
{
    public class PolylineMoveThumb : Thumb
    {
        private Vertex[] _startVertex;
        private bool _isSignal;

        public PolylineMoveThumb()
        {
            DragDelta += MoveThumb_DragDelta;
            DragStarted += OnDragStarted;
            DragCompleted += OnDragCompleted;
        }


        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var polylineShape = (PolylineShape)DataContext;
            polylineShape.OperationCompleted();
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var dataContext = (PolylineShape)((Thumb)sender).DataContext;
            _startVertex = dataContext.Vertices.Select(v => (Vertex)v.Clone()).ToArray();
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var dataContext = (PolylineShape)((Thumb)sender).DataContext;

            if (!_isSignal)
            {
                dataContext.OperationStart();
                _isSignal = true;
            }
            
            for (var index = 0; index < dataContext.Vertices.Count; index++)
            {
                var vertex = dataContext.Vertices[index];
                vertex.X = _startVertex[index].X + e.HorizontalChange;
                vertex.Y = _startVertex[index].Y + e.VerticalChange;

                if(index == 0)
                    Console.WriteLine(vertex.X);
            }
        }
    }
}