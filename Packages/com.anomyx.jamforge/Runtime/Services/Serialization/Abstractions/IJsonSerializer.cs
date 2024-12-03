namespace JamForge.Serialization
{
    public interface IJsonSerializer
    {
        string To<T>(T obj, bool isFormat = false);

        T From<T>(string json);
    }
}
