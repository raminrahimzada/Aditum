using System;
using System.IO;
using Aditum.Tests.Case1;
using Xunit;

namespace Aditum.Tests.Case2
{
    public class UnitTests2
    {
        private static readonly Random rand = new Random();
        static int Id()
        {
            return rand.Next();
        }

        private readonly TestUserService2 _service;
        public UnitTests2()
        {
            _service=new TestUserService2();
        }
    }
}
