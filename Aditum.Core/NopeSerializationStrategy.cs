using System;
using System.IO;

namespace Aditum.Core
{
    public class NopeSerializationStrategy<TUserId, TGroupId, TOperationId, TPermission> : ISerializeStrategy<TUserId, TGroupId, TOperationId, TPermission>
    {
        public void Serialize(BinaryWriter writer, TUserId userId)
        {
        }

        public void Serialize(BinaryWriter writer, TGroupId userId)
        {
        }

        public void Serialize(BinaryWriter writer, TOperationId userId)
        {
        }

        public void Serialize(BinaryWriter writer, TPermission userId)
        {
        }

        public void Deserialize(BinaryReader reader, out TUserId userId)
        {
            userId = default;
        }

        public void Deserialize(BinaryReader reader, out TGroupId groupId)
        {
            groupId = default;
        }

        public void Deserialize(BinaryReader reader, out TOperationId operationId)
        {
            operationId= default;
        }

        public void Deserialize(BinaryReader reader, out TPermission permission)
        {
            permission= default;
        }
    }
}