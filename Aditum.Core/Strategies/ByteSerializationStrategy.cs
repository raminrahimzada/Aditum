using System.IO;

namespace Aditum.Core
{
    public abstract class ByteSerializationStrategy<TPermission> :
        ISerializeStrategy<byte, byte,byte, byte, TPermission>
    {
        public void Serialize(BinaryWriter writer, byte i)
        {
            writer.Write(i);
        }
        public void Deserialize(BinaryReader reader, out byte i)
        {
            i = reader.ReadByte();
        }

        public abstract void Deserialize(BinaryReader reader, out TPermission permission);
        public abstract void Serialize(BinaryWriter writer, TPermission permission);
    }
}