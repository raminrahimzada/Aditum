using System;
using System.IO;
using Aditum.Core;
using Xunit;

namespace Aditum.Tests
{
    public class ExtensionsTests
    {
        private static readonly Random rand = new Random();

        private int Int => rand.Next();
        private bool Bool => rand.Next() % 2 == 0;
        private long Long => rand.Next();
        private short Short => (short) (rand.Next() % short.MaxValue);

        private int? IntNullable => Bool ? null as int? : Int;
        private bool? BoolNullable => Bool ? null as bool? : Bool;
        private long? LongNullable => Bool ? null as long? : Long;
        private short? ShortNullable => Bool ? null as short? : Short;
        private Guid? GuidNullable => Bool ? null as Guid? : Guid.NewGuid();

        void Test(Action<BinaryWriter> write, Action<BinaryReader> read)
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);
            write(writer);
            ms.Position = 0;
            var reader = new BinaryReader(ms);
            read(reader);
            reader.Dispose();
            writer.Dispose();
            ms.Dispose();
        }

        [Fact]
        public void Test_Int_Nullable()
        {
            var x = IntNullable;
            Test(writer =>
            {
                writer.Write(x);
            }, reader =>
            {
                var y = reader.ReadIntNullable();
                Assert.StrictEqual(x, y);
            });
        }
        
        [Fact]
        public void Test_Long_Nullable()
        {
            var x = LongNullable;
            Test(writer =>
            {
                writer.Write(x);
            }, reader =>
            {
                var y = reader.ReadLongNullable();
                Assert.StrictEqual(x, y);
            });
        }
        
        [Fact]
        public void Test_Bool_Nullable()
        {
            var x = BoolNullable;
            Test(writer =>
            {
                writer.Write(x);
            }, reader =>
            {
                var y = reader.ReadBooleanNullable();
                Assert.StrictEqual(x, y);
            });
        } 
        
        [Fact]
        public void Test_Short_Nullable()
        {
            var x = ShortNullable;
            Test(writer =>
            {
                writer.Write(x);
            }, reader =>
            {
                var y = reader.ReadShortNullable();
                Assert.StrictEqual(x, y);
            });
        }
        
        [Fact]
        public void Test_Guid_Nullable()
        {
            var x = GuidNullable;
            Test(writer =>
            {
                writer.Write(x);
            }, reader =>
            {
                var y = reader.ReadGuidNullable();
                Assert.StrictEqual(x, y);
            });
        }
        
        [Fact]
        public void Test_Guid()
        {
            var x = Guid.NewGuid();
            Test(writer =>
            {
                writer.Write(x);
            }, reader =>
            {
                var y = reader.ReadGuid();
                Assert.StrictEqual(x, y);
            });
        }

        [Fact]
        public void Test_Not_Contains()
        {
            var arr = new int[] {1, 2, 3, 4, 5};
            foreach (var x in arr)
            {
                Assert.False(arr.NotContains(x));
            }
            Assert.True(arr.NotContains(11));
            Assert.True(arr.NotContains(111));
            Assert.True(arr.NotContains(1111));
            Assert.True(arr.NotContains(11111));
        }
    }
}
