using System;
using System.Linq;

namespace Aditum.Core
{

    //Ensuring is the process of for example saying : Hey, I have users with id=1,23 and groups with id=3,4 ...etc

    public partial class UserService<TUserId,TGroupId,TGroupTypeId,TOperationId, TPermission> 
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
        where TGroupTypeId : IComparable<TGroupTypeId>
    {
        /// <summary>
        /// Initially we mention that there are users with given Id arr
        /// </summary>
        /// <param name="userIdArray"></param>
        public void EnsureUser(params TUserId[] userIdArray)
        {
            ReadLock(true);
            if (userIdArray.Any(_userIds.NotContains))
            {
                WriteLock();
                foreach (var userId in userIdArray)
                {
                    if (_userIds.NotContains(userId)) _userIds.Add(userId);
                }
                OnChanged();
                ExitWriteLockIfExists();
            }

            ExitReadLockIfExists(true);
        }

        /// <summary>
        /// Initially we mention that there is a group with given Id
        /// </summary>
        /// <param name="groupIdArray"></param>
        public void EnsureGroup(params TGroupId[] groupIdArray)
        {
            ReadLock(true);
            if (groupIdArray.Any(_groupIds.NotContains))                
            {
                WriteLock();
                foreach (var groupId in groupIdArray)
                {
                    if (_groupIds.NotContains(groupId)) _groupIds.Add(groupId);
                }
                OnChanged();
                ExitWriteLockIfExists();
            }
            ExitReadLockIfExists(true);
        }

        /// <summary>
        /// Initially we mention that there is an operation with given Id
        /// </summary>
        /// <param name="operationIdArray"></param>
        public void EnsureOperation(params TOperationId[] operationIdArray)
        {
            ReadLock(true);
            if (operationIdArray.Any(_operationIds.NotContains))
            {
                WriteLock();
                foreach (var operationId in operationIdArray)
                {
                    if (_operationIds.NotContains(operationId)) _operationIds.Add(operationId);
                }
                OnChanged();
                ExitWriteLockIfExists();
            }
            ExitReadLockIfExists(true);
        }


        /// <summary>
        /// Initially we mention that there is a group type with given Id
        /// </summary>
        /// <param name="groupTypeIdArray"></param>
        public void EnsureGroupType(params TGroupTypeId[] groupTypeIdArray)
        {
            ReadLock(true);
            if (groupTypeIdArray.Any(_groupTypeIds.NotContains))
            {
                WriteLock();
                foreach (var groupTypeId in groupTypeIdArray)
                {
                    if (_groupTypeIds.NotContains(groupTypeId)) _groupTypeIds.Add(groupTypeId);
                }

                OnChanged();
                ExitWriteLockIfExists();
            }
            ExitReadLockIfExists(true);
        }

        /// <summary>
        /// Initially we mention that there is a group with given Id
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="groupTypeId"></param>
        public void EnsureGroup(TGroupId groupId,TGroupTypeId groupTypeId)
        {
            ReadLock(true);

            bool Predicate((TGroupId GroupId, TGroupTypeId GroupTypeId) x) =>
                x.GroupId.Equals(groupId) && x.GroupTypeId.Equals(groupTypeId);

            var noNeedChanges = _groupIds.Contains(groupId) && _groupTypeIds.Contains(groupTypeId) &&
                                _groupTypes.Any(Predicate);
            if (!noNeedChanges)
            {
                WriteLock();
                if(!_groupTypeIds.Contains(groupTypeId)) _groupTypeIds.Add(groupTypeId);
                if (!_groupIds.Contains(groupId)) _groupIds.Add(groupId);
                if (_groupTypes.Any(Predicate))
                {
                    var old = _groupTypes.First(Predicate);
                    _groupTypes.Remove(old);
                }
                _groupTypes.Add((groupId, groupTypeId));
                OnChanged();
                ExitWriteLockIfExists();
            }

            ExitReadLockIfExists(true);
        }

        public void EnsureUser(TUserId userId,TGroupId groupId)
        {
            ReadLock(true);

            bool Predicate((TUserId UserId, TGroupId GroupId) x) =>
                x.GroupId.Equals(groupId) && x.GroupId.Equals(groupId);

            var noNeedChanges = _userIds.Contains(userId) && _groupIds.Contains(groupId) &&
                                _userGroups.Any(Predicate);
            if (!noNeedChanges)
            {
                WriteLock();
                if (!_userIds.Contains(userId)) _userIds.Add(userId);
                if (!_groupIds.Contains(groupId)) _groupIds.Add(groupId);
                if (_userGroups.Any(Predicate))
                {
                    var old = _userGroups.First(Predicate);
                    _userGroups.Remove(old);
                }
                _userGroups.Add((userId, groupId));
                OnChanged();
                ExitWriteLockIfExists();
            }

            ExitReadLockIfExists(true);
        }

        public void EnsureUser(TUserId userId, TGroupId groupId,TGroupTypeId groupTypeId)
        {
            ReadLock(true);

            bool Predicate1((TUserId UserId, TGroupId GroupId) x) =>
                x.GroupId.Equals(groupId) && x.GroupId.Equals(groupId);

            bool Predicate2((TGroupId GroupId, TGroupTypeId GroupTypeId) x) =>
                x.GroupId.Equals(groupId) && x.GroupTypeId.Equals(groupTypeId);

            var noNeedChanges = _userIds.Contains(userId) &&
                                _groupIds.Contains(groupId) &&
                                _groupTypeIds.Contains(groupTypeId) &&
                                _userGroups.Any(Predicate1) &&
                                _groupTypes.Any(Predicate2);
            if (!noNeedChanges)
            {
                WriteLock();
                if (!_userIds.Contains(userId)) _userIds.Add(userId);
                if (!_groupIds.Contains(groupId)) _groupIds.Add(groupId);
                if (!_groupTypeIds.Contains(groupTypeId)) _groupTypeIds.Add(groupTypeId);

                if (_userGroups.Any(Predicate1))
                {
                    var old = _userGroups.First(Predicate1);
                    _userGroups.Remove(old);
                }
                if (_groupTypes.Any(Predicate2))
                {
                    var old = _groupTypes.First(Predicate2);
                    _groupTypes.Remove(old);
                }
                _userGroups.Add((userId, groupId));
                _groupTypes.Add((groupId, groupTypeId));
                OnChanged();
                ExitWriteLockIfExists();
            }

            ExitReadLockIfExists(true);
        }
    }
}