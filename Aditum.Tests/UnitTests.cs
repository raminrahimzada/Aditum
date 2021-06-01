using Xunit;

namespace Aditum.Tests
{
    public class UnitTests
    {
        [Fact]
        public void Test_User_Add()
        {
            var service = new TestUserService();
            service.EnsureUserId(1);
            Assert.True(service.UserExists(1));
        }
        
        [Fact]
        public void Test_Group_Add()
        {
            var service = new TestUserService();
            service.EnsureGroupId(1);
            Assert.True(service.GroupExists(1));
        }

        [Fact]
        public void Test_Operation_Add()
        {
            var service = new TestUserService();
            service.EnsureOperationId(1);
            Assert.True(service.OperationExists(1));
        }

        [Fact]
        public void Test_Add_User_To_Group()
        {
            var service = new TestUserService();
            service.EnsureUserId(1);
            service.EnsureUserId(2);

            service.EnsureGroupId(1);
            service.EnsureGroupId(2);
            
            service.EnsureUserIsInGroup(1, 1);
            Assert.True(service.IsUserInGroup(1,1));
            Assert.False(service.IsUserInGroup(1,2));
            Assert.False(service.IsUserInGroup(2,1));
            Assert.False(service.IsUserInGroup(2,2));
        }

        [Fact]
        public void Test_Add_User_To_Group_And_Remove()
        {
            var service = new TestUserService();
            service.EnsureUserId(1);

            service.EnsureGroupId(1);

            service.EnsureUserIsInGroup(1, 1);
            Assert.True(service.IsUserInGroup(1, 1));

            service.EnsureUserIsNotInGroup(1, 1);
            Assert.False(service.IsUserInGroup(1, 1));

            service.EnsureUserIsInGroup(1, 1);
            Assert.True(service.IsUserInGroup(1, 1));
        }
    }
}
