using System;
using System.IO;

namespace Aditum.Core
{
    public abstract class GuidSerializationStrategy<TPermission> :
        ISerializeStrategy<Guid, Guid, Guid, TPermission>
    {
        public void Serialize(BinaryWriter writer, Guid i)
        {
            writer.Write(i);
        }
        public void Deserialize(BinaryReader reader, out Guid i)
        {
            i = reader.ReadGuid();
        }

        public abstract void Deserialize(BinaryReader reader, out TPermission permission);
        public abstract void Serialize(BinaryWriter writer, TPermission permission);
    }
}