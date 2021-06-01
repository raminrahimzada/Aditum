using Aditum.Core;

namespace Aditum.Tests.Case1
{
    public class TestUserService1 : UserService<int, int, int, bool>
    {
        public TestUserService1():base(new TestSerializer1())
        {
            ExceptionOccured += TestUserService_ExceptionOccured;
        }

        private static void TestUserService_ExceptionOccured(object sender, AditumException e)
        {
            throw e;
        }        
    }
}