using ProtoBuf;

namespace VectorEditor.Domain.Data.Persistence
{
    [ProtoContract]
    public sealed class SizeInfo
    {
        [ProtoMember(1)]
        public double Width { get; set; }

        [ProtoMember(2)]
        public double Height { get; set; }

        public SizeInfo()
        {
            
        }
    }
}