namespace VectorEditor.Services.SerializeService.Binary
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T data);
        T Deserialize<T>(byte[] bytes);
    }
}