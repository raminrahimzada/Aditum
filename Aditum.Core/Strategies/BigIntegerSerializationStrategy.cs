using System.IO;

namespace Aditum.Core
{
    public abstract class BigIntegerSerializationStrategy<TPermission> :
        ISerializeStrategy<long, long, long, TPermission>
    {
        public void Serialize(BinaryWriter writer, long i)
        {
            writer.Write(i);
        }
        public void Deserialize(BinaryReader reader, out long i)
        {
            i = reader.ReadInt64();
        }

        public abstract void Deserialize(BinaryReader reader, out TPermission permission);
        public abstract void Serialize(BinaryWriter writer, TPermission permission);
    }
}