using System.IO;

namespace Aditum
{
    public interface ISerializeStrategy<TUserId, TGroupId, TOperationId, TPermission>
    {
        void Serialize(BinaryWriter writer, TUserId userId);
        void Serialize(BinaryWriter writer, TGroupId userId);
        void Serialize(BinaryWriter writer, TOperationId userId);
        void Serialize(BinaryWriter writer, TPermission userId);
        
        void Deserialize(BinaryReader reader, out TUserId userId);
        void Deserialize(BinaryReader reader, out TGroupId groupId);
        void Deserialize(BinaryReader reader, out TOperationId operationId);
        void Deserialize(BinaryReader reader, out TPermission permission);
    }
}