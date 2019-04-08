using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace VectorEditor.Domain.Data
{
    public sealed class PolylineShape : Shape
    {
        private ObservableCollection<Vertex> _vertices;
        public ObservableCollection<Vertex> Vertices
        {
            get => _vertices;
            set
            {
                if (_vertices != null)
                    _vertices.CollectionChanged -= VerticesCollectionChanged;
                _vertices = value;

                foreach (var vertexItem in _vertices)
                {
                    vertexItem.PropertyChanged += VertexPropertyChanged;
                    vertexItem.StateChanging += ItemOnStateChanging;
                    vertexItem.StateChanged += ItemOnStateChanged;
                }
                _vertices.CollectionChanged += VerticesCollectionChanged;
            }
        }

        public PolylineShape()
        {
            Vertices = new ObservableCollection<Vertex>();
        }

        private void VerticesCollectionChanged(
            object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems.OfType<Vertex>())
                {
                    item.PropertyChanged += VertexPropertyChanged;
                    item.StateChanging+=ItemOnStateChanging;
                    item.StateChanged+=ItemOnStateChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems.OfType<Vertex>())
                {
                    item.PropertyChanged -= VertexPropertyChanged;
                    item.StateChanging -= ItemOnStateChanging;
                    item.StateChanged -= ItemOnStateChanged;
                }
            }

            OnPropertyChanged(nameof(Vertices));
        }

        private void ItemOnStateChanged(object sender, EventArgs e)
        {
            OnStateChanged();
        }

        private void ItemOnStateChanging(object sender, EventArgs e)
        {
            OnStateChanging();
        }

        private void VertexPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Vertices));
        }
    }
}