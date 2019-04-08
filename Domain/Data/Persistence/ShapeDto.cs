using System;
using ProtoBuf;

namespace VectorEditor.Domain.Data.Persistence
{
    [ProtoContract]
    [ProtoInclude(100,typeof(RectangleShapeDto))]
    [ProtoInclude(200,typeof(PolylineShapeDto))]
    public abstract class ShapeDto
    {
        [ProtoMember(1)]
        public double BorderThickness { get; set; }
        [ProtoMember(2)]
        public string BorderBrushColor { get; set; }
        [ProtoMember(3)]
        public string BackgroundBrushColor { get; set; }
        [ProtoMember(4)]
        public string Guid { get; set; }

        public ShapeDto()
        {
            
        }
    }
}