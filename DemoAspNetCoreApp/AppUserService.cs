using System.IO;
using Aditum;
using Aditum.Core;

namespace DemoAspNetCoreApp
{
    public static class Operations
    {
        public const int CanSee = 1;
        public const int CanUpdate = 2;
    }
    public class AppUserService:UserService<int,int,int,bool>
    {
        public AppUserService():base(new AppSerializationStrategy())
        {
           
        }

        public void Configure(string fileLocation)
        {
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
            EnsureUserId(1);
            EnsureGroupId(1);
            EnsureOperationId(1);
            EnsureOperationId(2);
            EnsureOperationId(3);
            EnsureUserIsInGroup(1, 1);
            SetGroupPermission(1, 1, true);
            SetGroupPermission(1, 2, false);
            
        }
    }

    public class AppSerializationStrategy:IntegerSerializationStrategy<bool>
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