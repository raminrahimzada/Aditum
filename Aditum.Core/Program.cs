using System;

namespace Aditum.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var service = new UserService<int, int, int, bool>();
            service.ExceptionOccured += Service_ExceptionOccured;
            service.Changed += Service_Changed;
            service.EnsureUserId(1);
            service.EnsureUserId(2);
            service.EnsureGroupId(1);
            service.EnsureGroupId(2);
            service.EnsureOperationId(1);
            service.EnsureUserIsInGroup(1, 1);
            service.EnsureUserIsInGroup(1, 2);
            service.EnsureUserIsInGroup(2, 2);
            service.SetGroupPermission(1, 1, true);
            service.SetGroupPermission(1, 2, false);
            service.SetGroupPermission(2, 1, true);
            service.SetGroupPermission(2, 2, true);
            var ok = service.GetUserPermission(1, 1);
        }

        private static void Service_Changed(object sender,EventArgs e)
        {
            Console.WriteLine("Settings Changed");
        }

        private static void Service_ExceptionOccured(object sender, Exception e)
        {
            throw e;
        }
    }
}
