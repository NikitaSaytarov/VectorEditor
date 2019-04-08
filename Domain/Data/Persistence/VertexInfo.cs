using ProtoBuf;

namespace VectorEditor.Domain.Data.Persistence
{
    [ProtoContract]
    public class VertexInfo
    {
        [ProtoMember(1)]
        public double X { get; set; }
        [ProtoMember(2)]
        public double Y { get; set; }
    }
}