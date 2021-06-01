using System;
using System.IO;
using Aditum.Tests.Case1;
using Xunit;

namespace Aditum.Tests.Case2
{
    public class UnitTests2
    {
        private readonly TestUserService2 _service;
        public UnitTests2()
        {
            _service=new TestUserService2();
        }
        
        [Fact]
        public void Test_User_Add()
        {
            
            _service.EnsureUserId(1);
            Assert.True(_service.UserExists(1));
        }
        
        
        [Fact]
        public void Test_Group_Add()
        {
            
            _service.EnsureGroupId(1);
            Assert.True(_service.GroupExists(1));
        }
        
        
        [Fact]
        public void Test_Operation_Add()
        {
            
            _service.EnsureOperationId(1);
            Assert.True(_service.OperationExists(1));
        }
        
        
        [Fact]
        public void Test_Add_User_To_Group()
        {
            
            _service.EnsureUserId(1);
            _service.EnsureUserId(2);

            _service.EnsureGroupId(1);
            _service.EnsureGroupId(2);
            
            _service.EnsureUserIsInGroup(1, 1);
            Assert.True(_service.IsUserInGroup(1,1));
            Assert.False(_service.IsUserInGroup(1,2));
            Assert.False(_service.IsUserInGroup(2,1));
            Assert.False(_service.IsUserInGroup(2,2));
        }
        
        
        [Fact]
        public void Test_Add_User_To_Group_And_Remove()
        {
            
            _service.EnsureUserId(1);

            _service.EnsureGroupId(1);

            _service.EnsureUserIsInGroup(1, 1);
            Assert.True(_service.IsUserInGroup(1, 1));

            _service.EnsureUserIsNotInGroup(1, 1);
            Assert.False(_service.IsUserInGroup(1, 1));

            _service.EnsureUserIsInGroup(1, 1);
            Assert.True(_service.IsUserInGroup(1, 1));
        }
        
        
        [Fact]
        public void Test_Group_Permission_Test_1()
        {
            
            _service.EnsureGroupId(1);
            _service.EnsureOperationId(1);
            _service.EnsureOperationId(2);

            //group 1 has permission on operation 1 but not on operation 2
            _service.SetGroupPermission(1, 1, true);
            
            Assert.True(_service.GetGroupPermission(1,1));
            Assert.False(_service.GetGroupPermission(1, 2) ?? false);

            _service.UnSetGroupPermission(1, 1);
            _service.SetGroupPermission(1, 2, true);

            Assert.False(_service.GetGroupPermission(1, 1) ?? false);
            Assert.True(_service.GetGroupPermission(1, 2));
        }
        
        
        [Fact]
        public void Test_User_Extra_Permission_1()
        {
            _service.EnsureGroupId(1);
            _service.EnsureOperationId(1);
            _service.EnsureUserId(1);

            //group 1 has permission on operation 1 
            _service.SetGroupPermission(1, 1, true);
            
            _service.EnsureUserIsInGroup(1, 1);


            //user is in group 1 so have access to operation 1
            //but we deny her operation 1 exclusively
            _service.SetUserExclusivePermission(1, 1, null);
            //so
            Assert.True(_service.GetGroupPermission(1, 1));
            Assert.Null(_service.GetUserPermission(1, 1));
        }
        
        
        [Fact]
        public void Test_User_Extra_Permission_2()
        {
            
            _service.EnsureGroupId(1);
            _service.EnsureOperationId(1);
            _service.EnsureUserId(1);

            //group 1 has no permission on operation 1 
            _service.SetGroupPermission(1, 1, null);
            
            _service.EnsureUserIsInGroup(1, 1);
            
            
            //user is in group 1 so have not access to operation 1
            //but we grant her operation 1 exclusively
            _service.SetUserExclusivePermission(1, 1, true);
            //so
            Assert.Null(_service.GetGroupPermission(1, 1));
            Assert.True(_service.GetUserPermission(1, 1));
        }
        
        
        [Fact]
        public void Test_Service_Dump_And_Load()
        {
            var rand = new Random();
            var serviceOld = new TestUserService1();
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
            var serviceNew = new TestUserService1();
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
