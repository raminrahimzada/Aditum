using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Aditum.Core
{
    public class UserService<TUserId,TGroupId,TOperationId, TPermission>
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
        where TPermission:struct
    {
        private readonly List<TUserId> _userIds = new List<TUserId>();
        private readonly List<TGroupId> _groupIds = new List<TGroupId>();
        private readonly List<TOperationId> _operationIds = new List<TOperationId>();
        public event EventHandler Changed;
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();
        private readonly List<(TUserId UserId, TGroupId GroupId)> _userGroups = new List<(TUserId, TGroupId)>();

        private readonly List<(TGroupId GroupId, TOperationId OperationId, TPermission Permission)> _groupPermissions =
            new List<(TGroupId GroupId, TOperationId OperationId,TPermission permission)>();

        private readonly List<(TUserId UserId, TOperationId OperationId, TPermission Permission)> _userExtraPermissions
            = new List<(TUserId UserId, TOperationId OperationId, TPermission Permission)>();

        public event EventHandler<Exception> ExceptionOccured;
       
        public byte[] Dump()
        {
            throw new NotImplementedException();
        }

        public void Load(ref byte[] buffer)
        {
            throw new NotImplementedException();
        }

        #region Helper

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadLock(in bool upgradable)
        {
            if (upgradable)
            {
                _readerWriterLock.EnterUpgradeableReadLock();
            }
            else
            {
                _readerWriterLock.EnterReadLock();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteLock()
        {
            _readerWriterLock.EnterWriteLock();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitWriteLockIfExists()
        {
            if (_readerWriterLock.IsWriteLockHeld)
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExitReadLockIfExists(in bool upgradable)
        {
            if (upgradable)
            {
                if (_readerWriterLock.IsUpgradeableReadLockHeld)
                    _readerWriterLock.ExitUpgradeableReadLock();
            }
            else
            {
                if (_readerWriterLock.IsReadLockHeld)
                    _readerWriterLock.ExitReadLock();
            }
        }

        #endregion

        #region Ensure
        public void EnsureUserId(TUserId userId)
        {
            ReadLock(true);
            if (!_userIds.Contains(userId))
            {
                WriteLock();
                _userIds.Add(userId);
                ExitWriteLockIfExists();
            };
            ExitReadLockIfExists(true);
        }
        public void EnsureGroupId(TGroupId groupId)
        {
            ReadLock(true);
            if (!_groupIds.Contains(groupId))
            {
                WriteLock();
                _groupIds.Add(groupId);
                ExitWriteLockIfExists();
            };
            ExitReadLockIfExists(true);
        }

        public void EnsureOperationId(TOperationId operationId)
        {
            ReadLock(true);
            if (!_operationIds.Contains(operationId))
            {
                WriteLock();
                _operationIds.Add(operationId);
                ExitWriteLockIfExists();
            };
            ExitReadLockIfExists(true);
        }

        #endregion


        #region Modify

        public void EnsureUserIsInGroup(TUserId userId, TGroupId groupId)
        {
            ReadLock(true);
            try
            {
                if (!_userIds.Contains(userId)) throw new Exception("User does not already exist:" + userId);
                if (!_groupIds.Contains(groupId)) throw new Exception("Group does not exist:" + groupId);
                if (_userGroups.Any(x => x.UserId.Equals(userId) && x.GroupId.Equals(groupId)))
                {
                    return;
                }

                WriteLock();
                _userGroups.Add((userId, groupId));
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }

        public void SetUserExtraPermission(TUserId userId, TOperationId operationId, TPermission permission)
        {
            try
            {
                ReadLock(true);
                if (!_userIds.Contains(userId)) throw new Exception("User does not exist:" + userId);

                Func<(TUserId UserId, TOperationId OperationId, TPermission permission), bool> expr = x =>
                    x.OperationId.Equals(operationId) && x.UserId.Equals(userId);

                var hasExtraPermission =
                    _userExtraPermissions.Any(expr);
                WriteLock();
                if (hasExtraPermission)
                {
                    var oldPermission = _userExtraPermissions.First(expr);
                    if (!oldPermission.Equals(permission))
                    {
                        _userExtraPermissions.Remove(oldPermission);
                    }
                }
                _userExtraPermissions.Add((userId, operationId, permission));
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
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

                if (!_groupIds.Contains(groupId)) throw new Exception("Group does not exist:" + groupId);

                Func<(TGroupId GroupId, TOperationId OperationId, TPermission permission), bool> expr = x =>
                    x.OperationId.Equals(operationId) && x.GroupId.Equals(groupId);

                var hasGroupPermission =
                    _groupPermissions.Any(expr);
                WriteLock();
                if (hasGroupPermission)
                {
                    var oldPermission = _groupPermissions.First(expr);
                    _groupPermissions.Remove(oldPermission);
                }
                _groupPermissions.Add((groupId, operationId, permission));
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        #endregion

        #region Remove

        public void EnsureUserIsNotInGroup(TUserId userId, TGroupId groupId)
        {
            ReadLock(true);
            try
            {
                if (!_userIds.Contains(userId)) throw new Exception("User does not already exist:" + userId);
                if (!_groupIds.Contains(groupId)) throw new Exception("Group does not exist:" + groupId);
                if (!_userGroups.Any(x => x.UserId.Equals(userId) && x.GroupId.Equals(groupId)))
                {
                    return;
                }
                WriteLock();
                _userGroups.Remove((userId, groupId));
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
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
                if (!_userIds.Contains(userId)) throw new Exception("User does not exist:" + userId);

                Func<(TUserId UserId, TOperationId OperationId, TPermission permission), bool> expr = x =>
                    x.OperationId.Equals(operationId) && x.UserId.Equals(userId);

                var hasGroupPermission =
                    _userExtraPermissions.Any(expr);
                if (hasGroupPermission)
                {
                    WriteLock();
                    var oldPermission = _userExtraPermissions.First(expr);
                    _userExtraPermissions.Remove(oldPermission);
                }
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
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

                if (!_groupIds.Contains(groupId)) throw new Exception("Group does not exist:" + groupId);

                Func<(TGroupId GroupId, TOperationId OperationId, TPermission permission), bool> expr = x =>
                    x.OperationId.Equals(operationId) && x.GroupId.Equals(groupId);

                var hasGroupPermission = _groupPermissions.Any(expr);
                if (hasGroupPermission)
                {
                    var oldPermission = _groupPermissions.First(expr);
                    WriteLock();
                    _groupPermissions.Remove(oldPermission);
                }
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
            }
            finally
            {
                ExitWriteLockIfExists();
                ExitReadLockIfExists(true);
            }
        }
        #endregion

        protected virtual void OnChanged(UserService<TUserId, TGroupId, TOperationId, TPermission> e)
        {
            WriteLock();
            try
            {
                Changed?.Invoke(this, null);
            }
            catch (Exception exception)
            {
                OnExceptionOccured(exception);
            }
            finally
            {
                ExitWriteLockIfExists();
            }
        }

        protected virtual void OnExceptionOccured(Exception e)
        {
            ExceptionOccured?.Invoke(this, e);
        }

        #region Query

        public TPermission GetUserPermission(TUserId userId,TOperationId operationId)
        {
            try
            {
                ReadLock(false);
                if (!_userIds.Contains(userId)) throw new Exception("User does not exist:" + userId);
                var userGroups = _userGroups.Where(x => x.UserId.Equals(userId)).Select(x => x.GroupId).ToArray();
                var groupPermissions = _groupPermissions
                    .Where(x => userGroups.Contains(x.GroupId) && x.OperationId.Equals(operationId))
                    .Select(x => x.Permission)
                    .Distinct()
                    .ToArray();
                
                Func<(TUserId UserId, TOperationId OperationId, TPermission Permission), bool> expr = x =>
                    x.UserId.Equals(userId) && x.OperationId.Equals(operationId);
                var hasExclusiveUserPermission = _userExtraPermissions.Any(expr);
                if (hasExclusiveUserPermission)
                {
                    return _userExtraPermissions.Where(expr)
                        .Select(x => x.Permission)
                        .First();
                }
                return groupPermissions.Max();
            }
            catch (Exception e)
            {
                OnExceptionOccured(e);
                return default;
            }
            finally
            {
                ExitReadLockIfExists(false);
            }

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
        
        #endregion

    }
}