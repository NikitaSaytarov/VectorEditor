using ProtoBuf;

namespace VectorEditor.Domain.Data.Persistence
{
    [ProtoContract]
    public class CanvasState
    {
        [ProtoMember(1)]
        public ShapeDto[] ShapesDto { get; set; }

        [ProtoMember(2)]
        public SizeInfo CanvasSize { get; set; }
        
        public CanvasState()
        {

        }
    }
}