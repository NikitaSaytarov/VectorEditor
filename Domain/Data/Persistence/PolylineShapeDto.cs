using ProtoBuf;

namespace VectorEditor.Domain.Data.Persistence
{
    [ProtoContract]
    public class PolylineShapeDto : ShapeDto
    {
        [ProtoMember(1)]
        public VertexInfo[] Vertices { get; set; }

        public PolylineShapeDto()
        {
            
        }
    }
}