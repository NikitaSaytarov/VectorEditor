using System.IO;

namespace VectorEditor.Services.SerializeService.Binary
{
    public sealed class ProtobufSerializer : IBinarySerializer
    {
        public byte[] Serialize<T>(T data)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        public T Deserialize<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }
    }
}