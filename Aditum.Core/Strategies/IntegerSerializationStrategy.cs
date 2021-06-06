using System.IO;

namespace Aditum.Core
{
    public abstract class IntegerSerializationStrategy<TPermission> :
        ISerializeStrategy<int, int, int,int, TPermission>
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
}