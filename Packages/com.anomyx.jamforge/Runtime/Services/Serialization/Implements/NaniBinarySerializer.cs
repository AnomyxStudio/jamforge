using Nino.Serialization;
using UnityEngine.Scripting;

namespace JamForge.Serialization
{
    [Preserve]
    public class JamSerializeAttribute : NinoSerializeAttribute { }

    [Preserve]
    public class JamMemberAttribute : NinoMemberAttribute
    {
        public JamMemberAttribute(ushort index) : base(index) { }
    }

    [Preserve]
    public class NaniBinarySerializer : IBinarySerializer
    {
        public byte[] To<T>(T obj, bool isCompressed = false)
        {
            return Serializer.Serialize(obj, isCompressed ? CompressOption.Zlib : CompressOption.NoCompression);
        }

        public T From<T>(byte[] bytes, bool isCompressed = false)
        {
            return Deserializer.Deserialize<T>(bytes, isCompressed ? CompressOption.Zlib : CompressOption.NoCompression);
        }
    }
}
