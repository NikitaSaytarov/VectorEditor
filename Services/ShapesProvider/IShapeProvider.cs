using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VectorEditor.Domain.Data;

namespace VectorEditor.Services.ShapeProvider
{
    public interface IShapeProvider
    {
        ReadOnlyObservableCollection<Shape> Shapes { get; }
        event EventHandler<Shape> ShapeSelected;

        void RemoveShape(Shape shape);
        void AddShape(Shape shape);

        Stack<byte[]> CanvasHistory { get; }
    }
}