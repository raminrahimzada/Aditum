using System;
using System.IO;
using Aditum.Core;
using Xunit;

namespace Aditum.Tests.Case1
{
    public class UnitTests1
    {
        private static readonly Random Rand = new Random();
        static int Id()
        {
            return Rand.Next();
        } 

        [Fact]
        public void Test_User_Add_1()
        {
            var service = new TestUserService1();
            var userId = Id();
            service.EnsureUser(userId);
            Assert.True(service.UserExists(userId));
        }

        [Fact]
        public void Test_Add_Group_Type_1()
        {
            var service = new TestUserService1();
            var groupTypeId = Id();
            service.EnsureGroupType(groupTypeId);
            Assert.True(service.GroupTypeExists(groupTypeId));
        }

        [Fact]
        public void Test_Add_Group_Type_2()
        {
            var service = new TestUserService1();
            var idArr = new[] { Id(), Id(), Id(), Id() };
            service.EnsureGroupType(idArr);
            foreach (var groupTypeId in idArr)
            {
                Assert.True(service.GroupTypeExists(groupTypeId));
            }
        }

        [Fact]
        public void Test_User_Add_2()
        {
            var service = new TestUserService1();
            var userIdArray = new[] { Id(), Id(), Id(), Id() };
            service.EnsureUser(userIdArray);
            foreach (var userId in userIdArray)
            {
                Assert.True(service.UserExists(userId));
            }
        }

        [Fact]
        public void Test_User_And_Group_Add()
        {
            var userId = Id();
            var groupId = Id();
            var service = new TestUserService1();
            service.EnsureUser(userId,groupId);
            Assert.True(service.UserExists(userId));
            Assert.True(service.GroupExists(groupId));
            Assert.True(service.IsUserInGroup(userId, groupId));

            Assert.False(service.IsUserInGroup(userId-1, groupId-1));
            Assert.False(service.IsUserInGroup(userId-1, groupId-1));
        }

        [Fact]
        public void Test_User_And_Group_And_GroupType_Add()
        {
            var userId = Id();
            var groupId = Id();
            var groupTypeId = Id();
            var service = new TestUserService1();
            service.EnsureUser(userId, groupId, groupTypeId);
            Assert.True(service.UserExists(userId));
            Assert.True(service.GroupExists(groupId));
            Assert.True(service.GroupTypeExists(groupTypeId));
            Assert.True(service.IsUserInGroup(userId, groupId));
            Assert.True(service.IsGroupOfType(groupId, groupTypeId));
        }


        [Fact]
        public void Test_Group_Add_1()
        {
            var groupId = Id();
            
            var service = new TestUserService1();
            service.EnsureGroup(groupId);
            Assert.True(service.GroupExists(groupId));
        }
        [Fact]
        public void Test_Group_Add_2()
        {
            var service = new TestUserService1();
            var groupIdArray = new int[] { Id(), Id(), Id(), Id() };
            service.EnsureGroup(groupIdArray);
            foreach (var groupId in groupIdArray)
            {
                Assert.True(service.GroupExists(groupId));
            }
        }

        [Fact]
        public void Test_Group_And_Group_Type_Add()
        {
            var groupId = Id();
            var groupTypeId = Id();
            var service = new TestUserService1();
            service.EnsureGroup(groupId, groupTypeId);
            Assert.True(service.GroupExists(groupId));
            Assert.True(service.GroupTypeExists(groupTypeId));
        }


        [Fact]
        public void Test_Operation_Add()
        {
            var operationId = Id();
            var service = new TestUserService1();

            service.EnsureOperation(operationId);
            Assert.True(service.OperationExists(operationId));
        }


        [Fact]
        public void Test_Add_User_To_Group()
        {
            var service = new TestUserService1();
            var userId1 = Id();
            var userId2 = Id();
            service.EnsureUser(userId1);
            service.EnsureUser(userId2);

            var groupId1 = Id();
            var groupId2 = Id();
            service.EnsureGroup(groupId1);
            service.EnsureGroup(groupId2);

            service.EnsureUser(userId1, groupId1);

            //
            Assert.True(service.IsUserInGroup(userId1, groupId1));
            Assert.False(service.IsUserInGroup(userId1, groupId2));
            Assert.False(service.IsUserInGroup(userId2, groupId1));
            Assert.False(service.IsUserInGroup(userId2, groupId2));
        }


        [Fact]
        public void Test_Add_User_To_Group_And_Remove()
        {
            var userId = Id();
            var operationId = Id();
            var groupId = Id();
            var groupTypeId = Id();

            var service = new TestUserService1();

            service.EnsureUser(userId);

            service.EnsureGroup(groupId);

            service.EnsureUser(userId,groupId);
            Assert.True(service.IsUserInGroup(userId, groupId));

            service.RemoveUserFromGroup(userId,groupId);
            Assert.False(service.IsUserInGroup(userId,groupId));

            service.EnsureUser(userId, groupId);
            Assert.True(service.IsUserInGroup(userId, groupId));
        }


        [Fact]
        public void Test_Group_Permission_Test_1()
        {
            var userId = Id();
            var operationId1 = Id();
            var operationId2 = Id();
            var groupId = Id();
            var groupTypeId = Id();

            var service = new TestUserService1();

            service.EnsureGroup(groupId);
            service.EnsureOperation(operationId1);
            service.EnsureOperation(operationId2);

            //group 1 has permission on operation 1 but not on operation 2
            service.SetGroupPermission(groupId, operationId1, true);

            Assert.True(service.GetGroupPermission(groupId, operationId1));
            Assert.False(service.GetGroupPermission(groupId, operationId2));

            service.UnSetGroupPermission(groupId, operationId1);
            service.SetGroupPermission(groupId, operationId2, true);

            Assert.False(service.GetGroupPermission(groupId, operationId1));
            Assert.True(service.GetGroupPermission(groupId,operationId2));
        }

        
        [Fact]
        public void Test_User_Extra_Permission_1()
        {
            var userId = Id();
            var operationId1 = Id();
            var operationId2 = Id();
            var groupId = Id();
            var groupTypeId = Id();

            var service = new TestUserService1();

            service.EnsureGroup(groupId);
            service.EnsureOperation(operationId1);
            service.EnsureUser(userId);

            //group 1 has permission on operation 1 
            service.SetGroupPermission(groupId, operationId1, true);

            service.EnsureUser(userId, groupId);


            //user is in group 1 so have access to operation 1
            //but we deny her operation 1 exclusively
            service.SetUserExclusivePermission(userId, operationId1, false);
            //so
            Assert.True(service.GetGroupPermission(groupId, operationId1));
            Assert.False(service.GetUserPermission(userId, operationId2));
        }


        [Fact]
        public void Test_User_Extra_Permission_2()
        {
            var userId = Id();
            var operationId1 = Id();
            var operationId2 = Id();
            var groupId = Id();
            var groupTypeId = Id();

            var service = new TestUserService1();

            service.EnsureGroup(groupId);
            service.EnsureOperation(operationId1);
            service.EnsureUser(userId);

            //group 1 has no permission on operation 1 
            service.SetGroupPermission(groupId, operationId1, false);
            
            //user is from group1
            service.EnsureUser(userId, groupId);


            //user is in group 1 so have not access to operation 1
            //but we grant her operation 1 exclusively
            service.SetUserExclusivePermission(userId, operationId1, true);
            //so
            Assert.False(service.GetGroupPermission(groupId, operationId1));
            Assert.True(service.GetUserPermission(userId, operationId1));
        }

        [Fact]
        public void Test_UnKnown_User_And_Group()
        {
            var service = new TestUserService1();
            for (int i = 0; i < 100; i++)
            {
                Assert.False(service.UserExists(Id()));
                Assert.False(service.OperationExists(Id()));
                Assert.False(service.GroupExists(Id()));
                Assert.False(service.GroupTypeExists(Id()));
            }

            try
            {
                service.GetUserPermission(Id(), Id());
                //this should not be hit
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is AditumException);
            }
            try
            {
                service.UnSetUserExtraPermission(Id(), Id());
                //this should not be hit
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is AditumException);
            }
            try
            {
                service.UnSetGroupPermission(Id(), Id());
                //this should not be hit
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is AditumException);
            }
            try
            {
                service.SetUserExclusivePermission(Id(), Id(), true);
                //this should not be hit
                Assert.True(false);
            }
            catch (Exception e)
            {
                Assert.True(e is AditumException);
            }
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

                serviceOld.EnsureGroup(groupId);
                serviceOld.EnsureOperation(operationId);
                serviceOld.EnsureUser(userId);

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
            using (var ms = new MemoryStream())
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
