using CatJson;
using UnityEngine.Scripting;

namespace JamForge.Serialization
{
    [Preserve]
    public class CatJsonSerializer : IJsonSerializer
    {
        public string To<T>(T obj, bool isFormat = false)
        {
            JsonParser.Default.IsFormat = isFormat;

            return JsonParser.Default.ToJson(obj);
        }

        public T From<T>(string json)
        {
            return JsonParser.Default.ParseJson<T>(json);
        }
    }
}
