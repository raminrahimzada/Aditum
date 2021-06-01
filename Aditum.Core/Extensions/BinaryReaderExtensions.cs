using System;
using System.IO;

namespace Aditum.Core
{
    public static partial class BinaryWriterExtensions
    {
        public static int? ReadIntNullable(this BinaryReader reader)
        {
            var b = reader.ReadBoolean();
            if (!b) return null;
            return reader.ReadInt32();
        }
        public static bool? ReadBooleanNullable(this BinaryReader reader)
        {
            var b = reader.ReadByte();
            if (b == 0) return null;
            if (b == 1) return true;
            if (b == 2) return false;
            throw AditumException.InvalidState(new {b});
        }
        public static long? ReadLongNullable(this BinaryReader reader)
        {
            var b = reader.ReadBoolean();
            if (!b) return null;
            return reader.ReadInt64();
        }
        public static short? ReadShortNullable(this BinaryReader reader)
        {
            var b = reader.ReadBoolean();
            if (!b) return null;
            return reader.ReadInt16();
        }
        public static Guid? ReadGuidNullable(this BinaryReader reader)
        {
            var b = reader.ReadBoolean();
            if (!b) return null;
            return reader.ReadGuid();
        }
        public static Guid ReadGuid(this BinaryReader reader)
        {
            var buffer = reader.ReadBytes(16);
            return new Guid(buffer);
        }
    }
}