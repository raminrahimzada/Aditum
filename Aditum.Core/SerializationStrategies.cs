using System;
using System.IO;

namespace Aditum
{
    public abstract class ByteSerializationStrategy<TPermission> :
        ISerializeStrategy<byte, byte, byte, TPermission>
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
    public abstract class IntegerSerializationStrategy<TPermission> :
        ISerializeStrategy<int, int, int, TPermission>
    {
        public void Serialize(BinaryWriter writer, int i)
        {
            writer.Write(i);
        }
        public void Deserialize(BinaryReader reader, out int i)
        {
            i = reader.ReadInt32();
        }

        public abstract void Deserialize(BinaryReader reader, out TPermission permission);
        public abstract void Serialize(BinaryWriter writer, TPermission permission);
    }
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
    public abstract class GuidSerializationStrategy<TPermission> :
        ISerializeStrategy<Guid, Guid, Guid, TPermission>
    {
        public void Serialize(BinaryWriter writer, Guid i)
        {
            writer.Write(i.ToByteArray());
        }
        public void Deserialize(BinaryReader reader, out Guid i)
        {
            var buffer = reader.ReadBytes(16);
            i = new Guid(buffer);
        }

        public abstract void Deserialize(BinaryReader reader, out TPermission permission);
        public abstract void Serialize(BinaryWriter writer, TPermission permission);
    }
}