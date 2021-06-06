using System.IO;

namespace Aditum.Core
{
    public interface ISerializeStrategy<TUserId, TGroupId,TGroupTypeId, TOperationId, TPermission>
    {
        void Serialize(BinaryWriter writer, TUserId userId);
        void Serialize(BinaryWriter writer, TGroupId groupId);
        void Serialize(BinaryWriter writer, TGroupTypeId groupTypeId);
        void Serialize(BinaryWriter writer, TOperationId operationId);
        void Serialize(BinaryWriter writer, TPermission permission);
        
        void Deserialize(BinaryReader reader, out TUserId userId);
        void Deserialize(BinaryReader reader, out TGroupId groupId);
        void Deserialize(BinaryReader reader, out TGroupTypeId groupTypeId);
        void Deserialize(BinaryReader reader, out TOperationId operationId);
        void Deserialize(BinaryReader reader, out TPermission permission);
    }
}