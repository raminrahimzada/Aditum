using System;
using System.IO;

namespace Aditum.Core
{
    public interface IUserService<TUserId, TGroupId, TGroupTypeId, TOperationId, TPermission> 
        where TUserId : IComparable<TUserId> 
        where TGroupId : IComparable<TGroupId> 
        where TGroupTypeId : IComparable<TGroupTypeId> 
    {
        // Events
        event EventHandler Changed;
        event EventHandler<AditumException> ExceptionOccured;

        // Strategies
        ISerializeStrategy<TUserId, TGroupId, TGroupTypeId, TOperationId, TPermission> SerializeStrategy { get; }
        IPermissionSelectStrategy<TGroupId, TGroupTypeId, TPermission> PermissionSelectStrategy { get; }


        // Save & Load
        void DumpTo(Stream stream);
        void DumpTo(string fileLocation);
        void LoadFrom(string fileLocation);
        void LoadFrom(Stream stream);

        // Just Ensure
        void EnsureUser(params TUserId[] userIdArray);
        void EnsureUser(TUserId userId, TGroupId groupId);
        void EnsureUser(TUserId userId, TGroupId groupId, TGroupTypeId groupTypeId);
        void EnsureGroupType(params TGroupTypeId[] groupTypeIdArray);
        void EnsureGroup(params TGroupId[] groupIdArray);
        void EnsureGroup(TGroupId groupId, TGroupTypeId groupTypeId);
        void EnsureOperation(params TOperationId[] operationIdArray);
        


        //Commands
        void SetUserExclusivePermission(TUserId userId, TOperationId operationId, TPermission permission);
        void SetGroupPermission(TGroupId groupId, TOperationId operationId, TPermission permission);
        void RemoveUserFromGroup(TUserId userId, TGroupId groupId);
        void UnSetUserExtraPermission(TUserId userId,TOperationId operationId);
        void UnSetGroupPermission(TGroupId groupId, TOperationId operationId);
        

        //Queries
        TPermission GetUserPermission(TUserId userId,TOperationId operationId);
        TPermission GetGroupPermission(TGroupId groupId, TOperationId operationId);
        bool UserExists(TUserId userId);
        bool GroupExists(TGroupId groupId);
        bool GroupTypeExists(TGroupTypeId groupTypeId);
        bool OperationExists(TOperationId operationId);
        bool IsUserInGroup(TUserId userId,TGroupId groupId);
        bool IsGroupOfType(TGroupId groupId, TGroupTypeId groupTypeId);
    }
}