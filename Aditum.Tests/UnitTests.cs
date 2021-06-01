using System;
using System.IO;
using System.Runtime.InteropServices;
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

        [Fact]
        public void Test_Group_Permission_Test_1()
        {
            var service = new TestUserService();
            service.EnsureGroupId(1);
            service.EnsureOperationId(1);
            service.EnsureOperationId(2);

            //group 1 has permission on operation 1 but not on operation 2
            service.SetGroupPermission(1, 1, true);
            
            Assert.True(service.GetGroupPermission(1,1));
            Assert.False(service.GetGroupPermission(1,2));

            service.UnSetGroupPermission(1, 1);
            service.SetGroupPermission(1, 2, true);

            Assert.False(service.GetGroupPermission(1, 1));
            Assert.True(service.GetGroupPermission(1, 2));
        }

        [Fact]
        public void Test_User_Extra_Permission_1()
        {
            var service = new TestUserService();
            service.EnsureGroupId(1);
            service.EnsureOperationId(1);
            service.EnsureUserId(1);

            //group 1 has permission on operation 1 
            service.SetGroupPermission(1, 1, true);
            
            service.EnsureUserIsInGroup(1, 1);


            //user is in group 1 so have access to operation 1
            //but we deny her operation 1 exclusively
            service.SetUserExclusivePermission(1, 1, false);
            //so
            Assert.True(service.GetGroupPermission(1, 1));
            Assert.False(service.GetUserPermission(1, 1));
        }

        [Fact]
        public void Test_User_Extra_Permission_2()
        {
            var service = new TestUserService();
            service.EnsureGroupId(1);
            service.EnsureOperationId(1);
            service.EnsureUserId(1);

            //group 1 has no permission on operation 1 
            service.SetGroupPermission(1, 1, false);
            
            service.EnsureUserIsInGroup(1, 1);
            
            
            //user is in group 1 so have not access to operation 1
            //but we grant her operation 1 exclusively
            service.SetUserExclusivePermission(1, 1, true);
            //so
            Assert.False(service.GetGroupPermission(1, 1));
            Assert.True(service.GetUserPermission(1, 1));
        }

        [Fact]
        public void Test_Service_Dump_And_Load()
        {
            var rand = new Random();
            var serviceOld = new TestUserService();
            for (int i = 0; i < 10000; i++)
            {
                var userId = rand.Next();
                var groupId = rand.Next();
                var operationId = rand.Next();
                
                serviceOld.EnsureGroupId(groupId);
                serviceOld.EnsureOperationId(operationId);
                serviceOld.EnsureUserId(userId);

                serviceOld.SetGroupPermission(groupId, operationId, rand.Next() % 2 == 0);
                serviceOld.SetUserExclusivePermission(userId, operationId, rand.Next() % 2 == 0);
            }
            byte[] buffer;
            using (var ms = new MemoryStream())
            {
                serviceOld.DumpTo(ms);
                buffer = ms.ToArray();
            }
            Assert.True(buffer.Length > 0);
            var serviceNew = new TestUserService();
            using (var ms=new MemoryStream())
            {
                ms.Write(buffer);
                ms.Position = 0;
                serviceNew.LoadFrom(ms);
                //check if the same users
                foreach (var userId in serviceNew.EnumerateUserIds())
                {
                    Assert.True(serviceOld.UserExists(userId));
                }
                foreach (var userId in serviceOld.EnumerateUserIds())
                {
                    Assert.True(serviceNew.UserExists(userId));
                }
                //check if the same groups
                foreach (var groupId in serviceNew.EnumerateGroupIds())
                {
                    Assert.True(serviceOld.GroupExists(groupId));
                }
                foreach (var groupId in serviceOld.EnumerateGroupIds())
                {
                    Assert.True(serviceNew.GroupExists(groupId));
                }
                //check if the same operations
                foreach (var operationId in serviceNew.EnumerateOperationIds())
                {
                    Assert.True(serviceOld.OperationExists(operationId));
                }
                foreach (var operationId in serviceOld.EnumerateOperationIds())
                {
                    Assert.True(serviceNew.OperationExists(operationId));
                }
                //todo check for group permissions
                //todo check for exclusive permissions
            }
        }
    }
}
