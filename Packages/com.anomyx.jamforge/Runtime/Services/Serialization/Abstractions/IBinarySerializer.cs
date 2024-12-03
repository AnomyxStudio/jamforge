namespace JamForge.Serialization
{
    public interface IBinarySerializer
    {
        byte[] To<T>(T obj, bool isCompressed = false);
        
        T From<T>(byte[] bytes, bool isCompressed = false);
    }
}
