using Aditum.Core;

namespace Aditum.Tests.Case2
{
    public class TestUserService2 : UserService<int, int, int, bool?>
    {
        public TestUserService2() : base(new TestSerializer2())
        {
            ExceptionOccured += TestUserService_ExceptionOccured;
        }

        private static void TestUserService_ExceptionOccured(object sender, AditumException e)
        {
            throw e;
        }
    }
}