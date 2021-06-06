using System.IO;
using System.Linq;
using Aditum.Core;

namespace DemoAspNetCoreApp
{
    public static class Operations
    {
        public const int CanSee = 1;
        public const int CanUpdate = 2;
    }

    public class AppUserService:UserService<int,int,int,int,bool>
    {
        public AppUserService():base(new DemoAppPermissionStrategy(),new DemoAppSerializationStrategy())
        {
        }

        public void Configure(string fileLocation)
        {
            //on change save it to where you want : file,redis,sql,mongo etc...
            Changed += (sender, e) => (sender as AppUserService)?.DumpTo(fileLocation);

            //Initialize on startup
            //TODO in prod do this
            //if (File.Exists(fileLocation))
            //    LoadFrom(fileLocation);
            //TODO demo purpose-reset after each restart
            LoadDemoSettings();
        }

        private void LoadDemoSettings()
        {
            //TODO DEMO purpose only,
            //adding some demo users and groups and setting permissions 
            EnsureUser(1);
            EnsureGroup(1);
            EnsureOperation(1);
            EnsureOperation(2);
            EnsureOperation(3);
            EnsureUser(1, 1);
            SetGroupPermission(1, 1, true);
            SetGroupPermission(1, 2, false);
        }
    }

    public class DemoAppPermissionStrategy : IPermissionSelectStrategy<int, int, bool>
    {
        public bool Decide(bool exclusivePermission, (int, int, bool)[] groupPermissions)
        {
            //if exclusive set then return it
            return exclusivePermission;
        }

        public bool Decide((int, int, bool)[] groupPermissions)
        {
            //else at least 1 of groups allow  it, allow 
            if (groupPermissions.Any(x => x.Item3))
            {
                return true;
            }

            return false;
        }
    }

    public class DemoAppSerializationStrategy:IntegerSerializationStrategy<bool>
    {
        public override void Deserialize(BinaryReader reader, out bool permission)
        {
            permission = reader.ReadBoolean();
        }

        public override void Serialize(BinaryWriter writer, bool permission)
        {
            writer.Write(permission);
        }
    }
}