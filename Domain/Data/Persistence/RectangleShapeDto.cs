using ProtoBuf;

namespace VectorEditor.Domain.Data.Persistence
{
    [ProtoContract]
    public class RectangleShapeDto : ShapeDto
    {
        [ProtoMember(1)]
        public double X { get; set; }
        [ProtoMember(2)]
        public double Y { get; set; }
        [ProtoMember(3)]
        public double Width { get; set; }
        [ProtoMember(4)]
        public double Height { get; set; }

        public RectangleShapeDto()
        {

        }
    }
}