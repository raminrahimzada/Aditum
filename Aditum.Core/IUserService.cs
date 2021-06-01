﻿using System;
using System.IO;

namespace Aditum.Core
{
    public interface IUserService<in TUserId, in TGroupId, in TOperationId, TPermission> 
        where TUserId : IComparable<TUserId> 
        where TGroupId : IComparable<TGroupId> 
    {
        event EventHandler Changed;
        event EventHandler<AditumException> ExceptionOccured;
        void DumpTo(Stream stream);
        void DumpTo(string fileLocation);
        void LoadFrom(string fileLocation);
        void LoadFrom(Stream stream);
        void EnsureUserId(TUserId userId);
        void EnsureGroupId(TGroupId groupId);
        void EnsureOperationId(TOperationId operationId);
        void EnsureUserIsInGroup(TUserId userId, TGroupId groupId);
        void SetUserExclusivePermission(TUserId userId, TOperationId operationId, TPermission permission);
        void SetGroupPermission(TGroupId groupId, TOperationId operationId, TPermission permission);
        void EnsureUserIsNotInGroup(TUserId userId, TGroupId groupId);
        void UnSetUserExtraPermission(TUserId userId,TOperationId operationId);
        void UnSetGroupPermission(TGroupId groupId, TOperationId operationId);
        TPermission GetUserPermission(TUserId userId,TOperationId operationId);
        TPermission GetGroupPermission(TGroupId groupId, TOperationId operationId);
        bool UserExists(TUserId userId);
        bool GroupExists(TGroupId groupId);
        bool OperationExists(TOperationId operationId);
        bool IsUserInGroup(TUserId userId,TGroupId groupId);
    }
}