using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Aditum.Core
{
    public class UserService<TUserId,TGroupId,TOperationId, TPermission> : IUserService<TUserId, TGroupId, TOperationId, TPermission> 
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
    {
        /// Main lists
        private readonly List<TUserId> _userIds = new List<TUserId>();
        private readonly List<TGroupId> _groupIds = new List<TGroupId>();
        private readonly List<TOperationId> _operationIds = new List<TOperationId>();
        
        // Join lists
        private readonly List<(TUserId UserId, TGroupId GroupId)> _userGroups = new List<(TUserId, TGroupId)>();
        private readonly List<(TGroupId GroupId, TOperationId OperationId, TPermission Permission)> _groupPermissions =
            new List<(TGroupId GroupId, TOperationId OperationId,TPermission permission)>();
        private readonly List<(TUserId UserId, TOperationId OperationId, TPermission Permission)> _userExtraPermissions
            = new List<(TUserId UserId, TOperationId OperationId, TPermission Permission)>();
        
        // Events
        public event EventHandler Changed;
        public event EventHandler<AditumException> ExceptionOccured;

        // Readonly helper fields
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();
        private readonly ISerializeStrategy<TUserId, TGroupId, TOperationId, TPermission> _strategy;

        public UserService(ISerializeStrategy<TUserId, TGroupId, TOperationId, TPermission> strategy=null)
        {
            _strategy = strategy;
        }

        public void DumpTo(Stream stream)
        {
            if (_strategy == null)
                throw AditumException.ParameterNeeded(nameof(_strategy));

            var writer = new BinaryWriter(stream);
            //1. _userIds
            writer.Write(_userIds.Count);
            foreach (var userId in _userIds)
            {
                _strategy.Serialize(writer, userId);
            }
            //2. _groupIds
            writer.Write(_groupIds.Count);
            foreach (var groupId in _groupIds)
            {
                _strategy.Serialize(writer, groupId);
            }
            //3. _operationIds
            writer.Write(_operationIds.Count);
            foreach (var operationId in _operationIds)
            {
                _strategy.Serialize(writer, operationId);
            }
            //4. _userGroups
            writer.Write(_userGroups.Count);
            foreach (var (userId, groupId) in _userGroups)
            {
                _strategy.Serialize(writer, userId);
                _strategy.Serialize(writer, groupId);
            }
            //5. _groupPermissions
            writer.Write(_groupPermissions.Count);
            foreach (var (groupId, operationId, permission) in _groupPermissions)
            {
                _strategy.Serialize(writer, groupId);
                _strategy.Serialize(writer, operationId);
                _strategy.Serialize(writer, permission);
            }
            //6. _userExtraPermissions
            writer.Write(_userExtraPermissions.Count);
            foreach (var (userId, operationId, permission) in _userExtraPermissions)
            {
                _strategy.Serialize(writer, userId);
                _strategy.Serialize(writer, operationId);
                _strategy.Serialize(writer, permission);
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
            if (_strategy == null)
                throw AditumException.ParameterNeeded(nameof(_strategy));

            using (var reader = new BinaryReader(stream))
            {
                //1. _userIds
                var userIdLength = reader.ReadInt32();
                for (var i = 0; i < userIdLength; i++)
                {
                    _strategy.Deserialize(reader, out TUserId userId);
                    _userIds.Add(userId);
                }

                //2. _groupIds
                var groupIdLength = reader.ReadInt32();
                for (var i = 0; i < groupIdLength; i++)
                {
                    _strategy.Deserialize(reader, out TGroupId groupId);
                    _groupIds.Add(groupId);
                }

                //3. _operationIds
                var operationIdLength = reader.ReadInt32();
                for (var i = 0; i < operationIdLength; i++)
                {
                    _strategy.Deserialize(reader, out TOperationId operationId);
                    _operationIds.Add(operationId);
                }

                //4. _userGroups
                var userGroupsLength = reader.ReadInt32();
                for (var i = 0; i < userGroupsLength; i++)
                {
                    _strategy.Deserialize(reader, out TUserId userId);
                    _strategy.Deserialize(reader, out TGroupId groupId);
                    _userGroups.Add((userId, groupId));
                }

                //5. _groupPermissions
                var groupPermissionsLength = reader.ReadInt32();
                for (var i = 0; i < groupPermissionsLength; i++)
                {
                    _strategy.Deserialize(reader, out TGroupId groupId);
                    _strategy.Deserialize(reader, out TOperationId operationId);
                    _strategy.Deserialize(reader, out TPermission permission);
                    _groupPermissions.Add((groupId, operationId, permission));
                }

                //6. _userExtraPermissions
                var userExtraPermissionsLength = reader.ReadInt32();
                for (var i = 0; i < userExtraPermissionsLength; i++)
                {
                    _strategy.Deserialize(reader, out TUserId userId);
                    _strategy.Deserialize(reader, out TOperationId operationId);
                    _strategy.Deserialize(reader, out TPermission permission);
                    _userExtraPermissions.Add((userId, operationId, permission));
                }
            }
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
                OnChanged(this);
                ExitWriteLockIfExists();
            }

            ExitReadLockIfExists(true);
        }
        public void EnsureGroupId(TGroupId groupId)
        {
            ReadLock(true);
            if (!_groupIds.Contains(groupId))
            {
                WriteLock();
                _groupIds.Add(groupId);
                OnChanged(this);
                ExitWriteLockIfExists();
            }
            ExitReadLockIfExists(true);
        }
        public void EnsureOperationId(TOperationId operationId)
        {
            ReadLock(true);
            if (!_operationIds.Contains(operationId))
            {
                WriteLock();
                _operationIds.Add(operationId);
                OnChanged(this);
                ExitWriteLockIfExists();
            }
            ExitReadLockIfExists(true);
        }

        #endregion


        #region Modify

        public void EnsureUserIsInGroup(TUserId userId, TGroupId groupId)
        {
            ReadLock(true);
            try
            {
                if (!_userIds.Contains(userId)) throw AditumException.NoMatchFound("User", userId);
                if (!_groupIds.Contains(groupId)) throw AditumException.NoMatchFound("Group", groupId);
                if (_userGroups.Any(x => x.UserId.Equals(userId) && x.GroupId.Equals(groupId)))
                {
                    return;
                }

                WriteLock();
                _userGroups.Add((userId, groupId));
                OnChanged(this);
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
                OnChanged(this);
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
                OnChanged(this);
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

        public void EnsureUserIsNotInGroup(TUserId userId, TGroupId groupId)
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
                OnChanged(this);
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
                    OnChanged(this);
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
                    OnChanged(this);
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

        private void OnChanged(UserService<TUserId, TGroupId, TOperationId, TPermission> e)
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
                    .Select(x => x.Permission)
                    .Distinct()
                    .ToArray();

                bool Predicate((TUserId UserId, TOperationId OperationId, TPermission Permission) x) => x.UserId.Equals(userId) && x.OperationId.Equals(operationId);
                var hasExclusiveUserPermission = _userExtraPermissions.Any(Predicate);
                if (hasExclusiveUserPermission)
                {
                    return _userExtraPermissions.Where(Predicate)
                        .Select(x => x.Permission)
                        .First();
                }
                //TODO do not use Max here 
                return groupPermissions.Max();
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