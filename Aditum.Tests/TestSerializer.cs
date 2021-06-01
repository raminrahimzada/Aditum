using System.IO;
using Aditum.Core;

namespace Aditum.Tests
{
    public class TestSerializer : ISerializeStrategy<int, int, int, bool>
    {
        public void Serialize(BinaryWriter writer, int i)
        {
            writer.Write(i);
        }

        public void Serialize(BinaryWriter writer, bool b)
        {
            writer.Write(b);
        }

        public void Deserialize(BinaryReader reader, out int i)
        {
            i = reader.ReadInt32();
        }

        public void Deserialize(BinaryReader reader, out bool b)
        {
            b= reader.ReadBoolean();
        }
    }
}