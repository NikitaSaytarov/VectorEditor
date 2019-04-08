using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using LightInject;
using Mapster;
using VectorEditor.Domain.Data;
using VectorEditor.Domain.Data.Persistence;

namespace VectorEditor.Setup.Mapping
{
    public class ShapesMappingProfile
    {
        public ShapesMappingProfile(IServiceFactory serviceFactory)
        {
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            TypeAdapterConfig<RectangleShape, RectangleShapeDto>
                .NewConfig()
                .Map(dest => dest.BackgroundBrushColor, src => src.BackgroundBrush.ToString())
                .Map(dest => dest.BorderBrushColor, src => src.BorderBrush.ToString())
                .Map(dest => dest.BorderThickness, src => src.BorderThickness)
                .Map(dest => dest.Width, src => src.Width)
                .Map(dest => dest.Height, src => src.Height)
                .Map(dest => dest.X, src => src.X)
                .Map(dest => dest.Y, src => src.Y)
                .Map(dest => dest.Guid, src => src.Guid);

            TypeAdapterConfig<Vertex, VertexInfo>
                .NewConfig()
                .Map(dest => dest.X, src => src.X)
                .Map(dest => dest.Y, src => src.Y);

            TypeAdapterConfig<Size ,SizeInfo>
                .NewConfig()
                .ConstructUsing(size => new SizeInfo()
                {
                    Width = size.Width,
                    Height = size.Height
                });

            TypeAdapterConfig<VertexInfo, Vertex>
                .NewConfig()
                .Map(dest => dest.X, src => src.X)
                .Map(dest => dest.Y, src => src.Y);

            TypeAdapterConfig<PolylineShapeDto, PolylineShape>
                .NewConfig()
                .Map(dest => dest.BackgroundBrush, src => new BrushConverter().ConvertFromString(src.BackgroundBrushColor))
                .Map(dest => dest.BorderBrush, src => new BrushConverter().ConvertFromString(src.BorderBrushColor))
                .Map(dest => dest.BorderThickness, src => src.BorderThickness)
                .Map(dest => dest.Vertices, src => new ObservableCollection<Vertex>(src.Vertices.Adapt<Vertex[]>()))
                .Map(dest => dest.Guid, src => src.Guid);

            TypeAdapterConfig<PolylineShape, PolylineShapeDto>
                .NewConfig()
                .Map(dest => dest.BackgroundBrushColor, src => src.BackgroundBrush.ToString())
                .Map(dest => dest.BorderBrushColor, src => src.BorderBrush.ToString())
                .Map(dest => dest.BorderThickness, src => src.BorderThickness)
                .Map(dest => dest.Vertices, src => src.Vertices.Adapt<VertexInfo[]>())
                .Map(dest => dest.Guid, src => src.Guid);

            TypeAdapterConfig<RectangleShapeDto, RectangleShape>
                .NewConfig()
                .Map(dest => dest.BackgroundBrush, src => new BrushConverter().ConvertFromString(src.BackgroundBrushColor))
                .Map(dest => dest.BorderBrush, src => new BrushConverter().ConvertFromString(src.BorderBrushColor))
                .Map(dest => dest.BorderThickness, src => src.BorderThickness)
                .Map(dest => dest.Width, src => src.Width)
                .Map(dest => dest.Height, src => src.Height)
                .Map(dest => dest.X, src => src.X)
                .Map(dest => dest.Y, src => src.Y)
                .ConstructUsing(shape => new RectangleShape
                {
                    Guid = shape.Guid
                });

            TypeAdapterConfig<ObservableCollection<Shape>, CanvasState>
                .NewConfig()
                .ConstructUsing(shapes => CanvasStateConstruct(shapes));

            TypeAdapterConfig<CanvasState, ObservableCollection<Shape>>
                .NewConfig()
                .ConstructUsing(canvasState => ShapesConstruct(canvasState));
        }

        private ObservableCollection<Shape> ShapesConstruct(CanvasState canvasState)
        {
            if (canvasState.ShapesDto == null)
                return new ObservableCollection<Shape>();

            var rectangleShapeDtos = canvasState.ShapesDto.OfType<RectangleShapeDto>();
            var canvasStateShapes = rectangleShapeDtos.Adapt<RectangleShape[]>();

            var polylineShapeDtos = canvasState.ShapesDto.OfType<PolylineShapeDto>();
            var polylineShapes = polylineShapeDtos.Adapt<PolylineShape[]>();

            var shapesList = new List<Shape>();
            shapesList.AddRange(canvasStateShapes);
            shapesList.AddRange(polylineShapes);

            return new ObservableCollection<Shape>(shapesList.ToArray());

        }

        private CanvasState CanvasStateConstruct(ObservableCollection<Shape> shapes)
        {
            var canvasState = new CanvasState();
            var canvasStateShapesDto = shapes.OfType<RectangleShape>().Adapt<RectangleShapeDto[]>();
            var polylineShapeDto = shapes.OfType<PolylineShape>().Adapt<PolylineShapeDto[]>();

            var shapesList = new List<ShapeDto>();
            shapesList.AddRange(canvasStateShapesDto);
            shapesList.AddRange(polylineShapeDto);

            canvasState.ShapesDto = shapesList.ToArray();
            return canvasState;
        }
    }
}