using System;
using System.IO;

namespace Aditum.Core
{
    public static partial class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, int? i)
        {
            writer.Write(i.HasValue);
            if (i.HasValue)
            {
                writer.Write(i.Value);
            }
        }
        public static void Write(this BinaryWriter writer, long? i)
        {
            writer.Write(i.HasValue);
            if (i.HasValue)
            {
                writer.Write(i.Value);
            }
        }
        public static void Write(this BinaryWriter writer, byte? i)
        {
            writer.Write(i.HasValue);
            if (i.HasValue)
            {
                writer.Write(i.Value);
            }
        }
        public static void Write(this BinaryWriter writer, short? i)
        {
            writer.Write(i.HasValue);
            if (i.HasValue)
            {
                writer.Write(i.Value);
            }
        }
        public static void Write(this BinaryWriter writer, bool? i)
        {
            //this way we can  save bool? with 1 byte instead of 2 separate byte 
            //0->null
            //1->true
            //2->false
            byte b;
            if (!i.HasValue)
            {
                b = 0;
            }
            else
            {
                b = i.Value ? (byte)1 : (byte)2;
            }

            writer.Write(b);
        }
        public static void Write(this BinaryWriter writer, Guid? i)
        {
            writer.Write(i.HasValue);
            if (i.HasValue)
            {
                writer.Write(i.Value);
            }
        }
        public static void Write(this BinaryWriter writer, Guid i)
        {
            writer.Write(i.ToByteArray());
        }
    }
}