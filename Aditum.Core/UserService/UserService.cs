using System;
using System.Collections.Generic;
using System.Linq;

namespace Aditum.Core
{
    public partial class UserService<TUserId,TGroupId,TGroupTypeId,TOperationId, TPermission> 
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
        where TGroupTypeId : IComparable<TGroupTypeId>
    {
        #region Modify
        
        public void SetUserExclusivePermission(TUserId userId, TOperationId operationId, TPermission permission)
        {
            try
            {
                ReadLock(true);
                if (!_userIds.Contains(userId)) throw AditumException.NoMatchFound("User", userId);

                bool Predicate((TUserId UserId, TOperationId OperationId, TPermission permission) x) =>
                    x.OperationId.Equals(operationId) && x.UserId.Equals(userId);

                var hasExtraPermission =
                    _userExtraPermissions.Any(Predicate);
                WriteLock();
                if (hasExtraPermission)
                {
                    var oldPermission = _userExtraPermissions.First(Predicate);
                    if (!oldPermission.Equals(permission))
                    {
                        _userExtraPermissions.Remove(oldPermission);
                    }
                }
                _userExtraPermissions.Add((userId, operationId, permission));
                OnChanged();
            }
            catch (AditumException e)
            {
                OnAditumExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        public void SetGroupPermission(TGroupId groupId, TOperationId operationId, TPermission permission)
        {
            try
            {
                ReadLock(true);

                if (!_groupIds.Contains(groupId)) throw AditumException.NoMatchFound("Group", groupId);

                bool Predicate((TGroupId GroupId, TOperationId OperationId, TPermission permission) x) => x.OperationId.Equals(operationId) && x.GroupId.Equals(groupId);

                var hasGroupPermission =
                    _groupPermissions.Any(Predicate);
                WriteLock();
                if (hasGroupPermission)
                {
                    var oldPermission = _groupPermissions.First(Predicate);
                    _groupPermissions.Remove(oldPermission);
                }
                _groupPermissions.Add((groupId, operationId, permission));
                OnChanged();
            }
            catch (AditumException e)
            {
                OnAditumExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        #endregion

        #region Remove

        public void RemoveUserFromGroup(TUserId userId, TGroupId groupId)
        {
            ReadLock(true);
            try
            {
                if (!_userIds.Contains(userId)) throw AditumException.NoMatchFound("User", userId);
                if (!_groupIds.Contains(groupId)) throw AditumException.NoMatchFound("Group", groupId);
                if (!_userGroups.Any(x => x.UserId.Equals(userId) && x.GroupId.Equals(groupId)))
                {
                    return;
                }
                WriteLock();
                _userGroups.Remove((userId, groupId));
                OnChanged();
            }
            catch (AditumException e)
            {
                OnAditumExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        public void UnSetUserExtraPermission(TUserId userId,TOperationId operationId)
        {
            try
            {
                ReadLock(true);
                if (!_userIds.Contains(userId)) throw AditumException.NoMatchFound("User", userId);

                bool Predicate((TUserId UserId, TOperationId OperationId, TPermission permission) x) => x.OperationId.Equals(operationId) && x.UserId.Equals(userId);

                var hasGroupPermission =
                    _userExtraPermissions.Any(Predicate);
                if (hasGroupPermission)
                {
                    WriteLock();
                    var oldPermission = _userExtraPermissions.First(Predicate);
                    _userExtraPermissions.Remove(oldPermission);
                    OnChanged();
                }
            }
            catch (AditumException e)
            {
                OnAditumExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        public void UnSetGroupPermission(TGroupId groupId, TOperationId operationId)
        {
            try
            {
                ReadLock(true);

                if (!_groupIds.Contains(groupId)) throw AditumException.NoMatchFound("Group", groupId);

                bool Predicate((TGroupId GroupId, TOperationId OperationId, TPermission permission) x) => x.OperationId.Equals(operationId) && x.GroupId.Equals(groupId);

                var hasGroupPermission = _groupPermissions.Any(Predicate);
                if (hasGroupPermission)
                {
                    var oldPermission = _groupPermissions.First(Predicate);
                    WriteLock();
                    _groupPermissions.Remove(oldPermission);
                    OnChanged();
                }
            }
            catch (AditumException e)
            {
                OnAditumExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        #endregion

        private void OnChanged()
        {
            try
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
            catch (AditumException exception)
            {
                OnAditumExceptionOccured(exception);
            }
        }

        private void OnAditumExceptionOccured(AditumException e)
        {
            ExceptionOccured?.Invoke(this, e);
        }

        #region Query

        public TPermission GetUserPermission(TUserId userId,TOperationId operationId)
        {
            try
            {
                ReadLock(false);
                if (!_userIds.Contains(userId)) throw AditumException.NoMatchFound("User", userId);
                var userGroups = _userGroups.Where(x => x.UserId.Equals(userId)).Select(x => x.GroupId).ToArray();
                var groupPermissions = _groupPermissions
                    .Where(x => userGroups.Contains(x.GroupId) && x.OperationId.Equals(operationId))
                    .Select(x => (x.GroupId, x.Permission))
                    .Join(_groupTypes, x => x.GroupId, y => y.GroupId,
                        (x, y) => (x.GroupId, y.GroupTypeId, x.Permission))
                    .ToArray();

                bool Predicate((TUserId UserId, TOperationId OperationId, TPermission Permission) x) => x.UserId.Equals(userId) && x.OperationId.Equals(operationId);
                var hasExclusiveUserPermission = _userExtraPermissions.Any(Predicate);
                if (hasExclusiveUserPermission)
                {
                   var exclusivePermission = _userExtraPermissions.Where(Predicate)
                        .Select(x => x.Permission)
                        .First();
                   return PermissionSelectStrategy.Decide(exclusivePermission,groupPermissions);
                }
                return PermissionSelectStrategy.Decide(groupPermissions);
            }
            catch (AditumException e)
            {
                OnAditumExceptionOccured(e);
                return default;
            }
            finally
            {
                ExitReadLockIfExists(false);
            }
        }

        public TPermission GetGroupPermission(TGroupId groupId, TOperationId operationId)
        {
            Func<(TGroupId GroupId, TOperationId OperationId, TPermission Permission), bool> expr = x =>
                x.GroupId.Equals(groupId) && x.OperationId.Equals(operationId);

            ReadLock(false);
            var permission = _groupPermissions.Any(expr) ? _groupPermissions.First(expr).Permission : default;
            ExitReadLockIfExists(false);
            return permission;
        }

        public bool UserExists(TUserId userId)
        {
            ReadLock(false);
            var result = _userIds.Contains(userId);
            ExitReadLockIfExists(false);
            return result;
        }

        public bool GroupExists(TGroupId groupId)
        {
            ReadLock(false);
            var result = _groupIds.Contains(groupId);
            ExitReadLockIfExists(false);
            return result;
        }
        
        public bool GroupTypeExists(TGroupTypeId groupTypeId)
        {
            ReadLock(false);
            var result = _groupTypeIds.Contains(groupTypeId);
            ExitReadLockIfExists(false);
            return result;
        }

        public bool OperationExists(TOperationId operationId)
        {
            ReadLock(false);
            var result = _operationIds.Contains(operationId);
            ExitReadLockIfExists(false);
            return result;
        }
        public bool IsUserInGroup(TUserId userId,TGroupId groupId)
        {
            ReadLock(false);
            var result = _userGroups.Any(x => x.GroupId.Equals(groupId) && x.UserId.Equals(userId));
            ExitReadLockIfExists(false);
            return result;
        }
        public bool IsGroupOfType(TGroupId groupId,TGroupTypeId groupTypeId)
        {
            ReadLock(false);
            var result = _groupTypes.Any(x => x.GroupId.Equals(groupId) && x.GroupTypeId.Equals(groupTypeId));
            ExitReadLockIfExists(false);
            return result;
        }
        public IReadOnlyList<TUserId> EnumerateUserIds()
        {
            ReadLock(false);
            var result = _userIds.AsReadOnly();
            ExitReadLockIfExists(false);
            return result;
        }
        public IReadOnlyList<TGroupId> EnumerateGroupIds()
        {
            ReadLock(false);
            var result = _groupIds.AsReadOnly();
            ExitReadLockIfExists(false);
            return result;
        }
        public IReadOnlyList<TOperationId> EnumerateOperationIds()
        {
            ReadLock(false);
            var result = _operationIds.AsReadOnly();
            ExitReadLockIfExists(false);
            return result;
        }
        #endregion

    }
}