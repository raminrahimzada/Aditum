using System;
using System.Collections.Generic;
using Aditum.Core;

namespace Aditum.Tests
{
    public class TestUserService : UserService<int, int, int, bool>
    {
        public TestUserService():base(new TestSerializer())
        {
            this.ExceptionOccured += TestUserService_ExceptionOccured;
        }

        private void TestUserService_ExceptionOccured(object sender, System.Exception e)
        {
            throw e;
        }        
    }
}