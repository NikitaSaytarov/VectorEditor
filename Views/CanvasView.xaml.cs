using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VectorEditor.Domain.Data;
using VectorEditor.ViewModels;

namespace VectorEditor.Views
{
    public partial class CanvasView
    {
        private Canvas _canvas;
        private Canvas _drawPolylineCanvas;
        private Canvas _drawRectangleCanvas;

        public CanvasView()
        {
            InitializeComponent();
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var shape = ((FrameworkElement)sender).DataContext;
            ShapesListView.SelectedItem = shape;
        }

        private void MainCanvas_OnLoaded(object sender, RoutedEventArgs e)
        {
            _canvas = (Canvas) sender;

            var viewModel = (CanvasViewModel) DataContext;
            viewModel.InitializeCanvas(_canvas);
        }

        private void CanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var panel = (Panel)sender;
            var position = e.GetPosition(panel);

            var viewModel = (CanvasViewModel)DataContext;
            viewModel.DrawingPolyline.Vertices.Add(new Vertex()
            {
                X = position.X,
                Y = position.Y
            });
        }

        private void MouseRightButtonDown2(object sender, MouseButtonEventArgs e)
        {
            var viewModel = (CanvasViewModel)DataContext;
            viewModel.CompleteDrawingPolyline();
        }


        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var vertex = (Vertex)((Thumb)sender).DataContext;

            vertex.X = vertex.X + e.HorizontalChange;
            vertex.Y = vertex.Y + e.VerticalChange;
        }

        private void ThumbDragDeltaForExist(object sender, DragDeltaEventArgs e)
        {
            var vertex = (Vertex)((Thumb)sender).DataContext;

            vertex.X = vertex.X + e.HorizontalChange;
            vertex.Y = vertex.Y + e.VerticalChange;
        }

        private void DragStartedForExist(object sender, DragStartedEventArgs e)
        {
            var vertex = (Vertex)((Thumb)sender).DataContext;
            vertex.OperationStarted();
        }

        private void DragCompletedForExist(object sender, DragCompletedEventArgs e)
        {
            var vertex = (Vertex)((Thumb)sender).DataContext;
            vertex.OperationCompleted();
        }

        Point scrollMousePoint = new Point();
        double hOff = 1;
        double vOff = 1;

        private Cursor _prevCursor;

        private void scrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var uiElement = Mouse.DirectlyOver as UIElement;
            var canvasViewModel = ((CanvasViewModel)DataContext);

            if (!canvasViewModel.IsDragMode)
                return;

            if (uiElement is Rectangle || canvasViewModel.DrawingPolyline != null || !(uiElement is Canvas))
                return;

            scrollMousePoint = e.GetPosition(ScrollViewer);
            hOff = ScrollViewer.HorizontalOffset;
            vOff = ScrollViewer.VerticalOffset;
            ScrollViewer.CaptureMouse();

            _prevCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void scrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollViewer.IsMouseCaptured)
            {
                ScrollViewer.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(ScrollViewer).X));
                ScrollViewer.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(ScrollViewer).Y));
            }
        }

        private void scrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvasViewModel = ((CanvasViewModel)DataContext);
            if (!canvasViewModel.IsDragMode)
                return;

            Mouse.OverrideCursor = _prevCursor;
            ScrollViewer.ReleaseMouseCapture();
        }

        private void scrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + e.Delta);
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + e.Delta);
        }

        protected override void OnLoaded(object sender, RoutedEventArgs e)
        {
            base.OnLoaded(sender, e);
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CanvasViewModel.CanvasScaleRate))
            {
                var canvasViewModel = (CanvasViewModel)ViewModel;
                ScaleCanvas(canvasViewModel.CanvasScaleRate);
            }
        }

        private void ScaleCanvas(CanvasScaleRate canvasScaleRate)
        {
            var st = (ScaleTransform)_canvas.RenderTransform;
            var newSt = new ScaleTransform(st.ScaleX, st.ScaleY)
            {
                ScaleX = 1 * canvasScaleRate.Scale, ScaleY = 1 * canvasScaleRate.Scale
            };
            _canvas.LayoutTransform = newSt;
            _canvas.UpdateLayout();

            _drawPolylineCanvas.LayoutTransform = newSt;
            _drawPolylineCanvas.UpdateLayout();

            _drawRectangleCanvas.LayoutTransform = newSt;
            _drawRectangleCanvas.UpdateLayout();

            ScrollViewer.ScrollToVerticalOffset(0);
            ScrollViewer.ScrollToVerticalOffset(CanvasContainer.ActualHeight / 2);

            ScrollViewer.ScrollToHorizontalOffset(0);
            ScrollViewer.ScrollToHorizontalOffset(CanvasContainer.ActualWidth/ 2);
        }


        private bool _mouseDown;
        private Point _mouseDownPos;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
            _mouseDownPos = e.GetPosition(_drawRectangleCanvas);
            theGrid.CaptureMouse();
        
            Canvas.SetLeft(selectionBox, _mouseDownPos.X);
            Canvas.SetTop(selectionBox, _mouseDownPos.Y);
            selectionBox.Width = 0;
            selectionBox.Height = 0;
        
            selectionBox.Visibility = Visibility.Visible;
        }
        
        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
            theGrid.ReleaseMouseCapture();
        
            selectionBox.Visibility = Visibility.Collapsed;
        
            var mouseUpPos = e.GetPosition(_drawRectangleCanvas);
            var canvasViewModel = (CanvasViewModel)ViewModel;
            canvasViewModel.CompleteDrawingRectangle(_mouseDownPos, mouseUpPos);
        }
        
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                Point mousePos = e.GetPosition(_drawRectangleCanvas);
        
                if (_mouseDownPos.X < mousePos.X)
                {
                    Canvas.SetLeft(selectionBox, _mouseDownPos.X);
                    selectionBox.Width = mousePos.X - _mouseDownPos.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mousePos.X);
                    selectionBox.Width = _mouseDownPos.X - mousePos.X;
                }
        
                if (_mouseDownPos.Y < mousePos.Y)
                {
                    Canvas.SetTop(selectionBox, _mouseDownPos.Y);
                    selectionBox.Height = mousePos.Y - _mouseDownPos.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mousePos.Y);
                    selectionBox.Height = _mouseDownPos.Y - mousePos.Y;
                }
            }
        }

        private void DrawingPolyline_OnLoaded(object sender, RoutedEventArgs e)
        {
            _drawPolylineCanvas = (Canvas) sender;
        }

        private void DrawingRectangle_OnLoaded(object sender, RoutedEventArgs e)
        {
            _drawRectangleCanvas = (Canvas)sender;
        }
    }
}
