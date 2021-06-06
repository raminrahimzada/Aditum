using System;
using System.IO;

namespace Aditum.Core
{
    public partial class UserService<TUserId,TGroupId,TGroupTypeId,TOperationId, TPermission> 
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
        where TGroupTypeId : IComparable<TGroupTypeId>
    {
        public void DumpTo(Stream stream)
        {
            if (SerializeStrategy == null)
                throw AditumException.ParameterNeeded(nameof(SerializeStrategy));

            var writer = new BinaryWriter(stream);
            //1. _userIds
            writer.Write(_userIds.Count);
            foreach (var userId in _userIds)
            {
                SerializeStrategy.Serialize(writer, userId);
            }
            //2. _groupIds
            writer.Write(_groupIds.Count);
            foreach (var groupId in _groupIds)
            {
                SerializeStrategy.Serialize(writer, groupId);
            }
            //3. _operationIds
            writer.Write(_operationIds.Count);
            foreach (var operationId in _operationIds)
            {
                SerializeStrategy.Serialize(writer, operationId);
            }
            //4. _userGroups
            writer.Write(_userGroups.Count);
            foreach (var (userId, groupId) in _userGroups)
            {
                SerializeStrategy.Serialize(writer, userId);
                SerializeStrategy.Serialize(writer, groupId);
            }
            //5. _groupPermissions
            writer.Write(_groupPermissions.Count);
            foreach (var (groupId, operationId, permission) in _groupPermissions)
            {
                SerializeStrategy.Serialize(writer, groupId);
                SerializeStrategy.Serialize(writer, operationId);
                SerializeStrategy.Serialize(writer, permission);
            }
            //6. _userExtraPermissions
            writer.Write(_userExtraPermissions.Count);
            foreach (var (userId, operationId, permission) in _userExtraPermissions)
            {
                SerializeStrategy.Serialize(writer, userId);
                SerializeStrategy.Serialize(writer, operationId);
                SerializeStrategy.Serialize(writer, permission);
            }
            //7. _groupTypeIds
            writer.Write(_groupTypeIds.Count);
            foreach (var groupTypeId in _groupTypeIds)
            {
                SerializeStrategy.Serialize(writer, groupTypeId);                
            }
            //8. _groupTypes
            writer.Write(_groupTypes.Count);
            foreach (var (groupId, groupTypeId) in _groupTypes)
            {
                SerializeStrategy.Serialize(writer, groupId);
                SerializeStrategy.Serialize(writer, groupTypeId);
            }
            writer.Flush();
            writer.Dispose();
        }

        public void DumpTo(string fileLocation)
        {
            using (var fs=File.OpenWrite(fileLocation))
            {
                DumpTo(fs);
            }
        }

        public void LoadFrom(string fileLocation)
        {
            using (var fs=File.OpenRead(fileLocation))
            {
                LoadFrom(fs);
            }
        }

        public void LoadFrom(Stream stream)
        {
            if (SerializeStrategy == null)
                throw AditumException.ParameterNeeded(nameof(SerializeStrategy));

            using (var reader = new BinaryReader(stream))
            {
                //1. _userIds
                var userIdLength = reader.ReadInt32();
                for (var i = 0; i < userIdLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TUserId userId);
                    _userIds.Add(userId);
                }

                //2. _groupIds
                var groupIdLength = reader.ReadInt32();
                for (var i = 0; i < groupIdLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TGroupId groupId);
                    _groupIds.Add(groupId);
                }

                //3. _operationIds
                var operationIdLength = reader.ReadInt32();
                for (var i = 0; i < operationIdLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TOperationId operationId);
                    _operationIds.Add(operationId);
                }

                //4. _userGroups
                var userGroupsLength = reader.ReadInt32();
                for (var i = 0; i < userGroupsLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TUserId userId);
                    SerializeStrategy.Deserialize(reader, out TGroupId groupId);
                    _userGroups.Add((userId, groupId));
                }

                //5. _groupPermissions
                var groupPermissionsLength = reader.ReadInt32();
                for (var i = 0; i < groupPermissionsLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TGroupId groupId);
                    SerializeStrategy.Deserialize(reader, out TOperationId operationId);
                    SerializeStrategy.Deserialize(reader, out TPermission permission);
                    _groupPermissions.Add((groupId, operationId, permission));
                }

                //6. _userExtraPermissions
                var userExtraPermissionsLength = reader.ReadInt32();
                for (var i = 0; i < userExtraPermissionsLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TUserId userId);
                    SerializeStrategy.Deserialize(reader, out TOperationId operationId);
                    SerializeStrategy.Deserialize(reader, out TPermission permission);
                    _userExtraPermissions.Add((userId, operationId, permission));
                }
                //7. _groupTypeIds
                var groupTypeIdsLength = reader.ReadInt32();
                for (var i = 0; i < groupTypeIdsLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TGroupTypeId groupTypeId);
                    _groupTypeIds.Add(groupTypeId);
                }
                //8. _groupTypes
                var groupTypesLength = reader.ReadInt32();
                for (var i = 0; i < groupTypesLength; i++)
                {
                    SerializeStrategy.Deserialize(reader, out TGroupId groupId);
                    SerializeStrategy.Deserialize(reader, out TGroupTypeId groupTypeId);
                    _groupTypes.Add((groupId, groupTypeId));
                }
            }
        }
    }
}