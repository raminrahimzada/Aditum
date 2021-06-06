using System.Linq;
using Aditum.Core;

namespace Aditum.Tests.Case2
{
    public class TestUserService2 : UserService<int, int,int, int, bool?>
    {
        public TestUserService2() : base(new PermissionSelectStrategy2(), new TestSerializer2())
        {
            ExceptionOccured += TestUserService_ExceptionOccured;
        }

        private static void TestUserService_ExceptionOccured(object sender, AditumException e)
        {
            throw e;
        }
    }

    public class PermissionSelectStrategy2 : IPermissionSelectStrategy<int, int, bool?>
    {
        public bool? Decide(bool? exclusivePermission, (int, int, bool?)[] groupPermissions)
        {
            //If a user has exclusive permission then use it
            if (exclusivePermission.HasValue) return exclusivePermission.Value;
            //else if any groups has granted then grant
            if (groupPermissions.Any(x => x.Item3 ?? false))
            {
                return true;
            }
            //no exclusive permission set and no group permission allows this operation
            return false;
        }

        public bool? Decide((int, int, bool?)[] groupPermissions)
        {
            //if any groups has granted then grant
            if (groupPermissions.Any(x => x.Item3 ?? false))
            {
                return true;
            }
            //no exclusive permission set and no group permission allows this operation
            return false;
        }
    }
}