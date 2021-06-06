using System;
using System.Collections.Generic;
using System.Threading;

namespace Aditum.Core
{
    public partial class UserService<TUserId,TGroupId,TGroupTypeId,TOperationId, TPermission> 
                    : IUserService<TUserId, TGroupId, TGroupTypeId, TOperationId, TPermission> 
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
        where TGroupTypeId : IComparable<TGroupTypeId>
    {
        /// <summary>
        /// List of user ids
        /// </summary>
        private readonly List<TUserId> _userIds = new List<TUserId>();
        
        /// <summary>
        /// List of group ids
        /// </summary>

        private readonly List<TGroupId> _groupIds = new List<TGroupId>();

        /// <summary>
        /// List of operation ids
        /// </summary>

        private readonly List<TOperationId> _operationIds = new List<TOperationId>();
        
        /// <summary>
        /// List of group type ids
        /// </summary>

        private readonly List<TGroupTypeId> _groupTypeIds = new List<TGroupTypeId>();


        /// <summary>
        /// Here we store Which UserId belongs to which GroupId
        /// </summary>
        private readonly List<(TUserId UserId, TGroupId GroupId)> _userGroups = new List<(TUserId, TGroupId)>();

        /// <summary>
        /// Here we store Which GroupId has which `Permission` permission on OperationId
        /// </summary>
        private readonly List<(TGroupId GroupId, TOperationId OperationId, TPermission Permission)> _groupPermissions =
            new List<(TGroupId GroupId, TOperationId OperationId, TPermission permission)>();

        /// <summary>
        /// Here we store Which UserId has which exclusive `Permission` permission on OperationId
        /// </summary>
        private readonly List<(TUserId UserId, TOperationId OperationId, TPermission Permission)> _userExtraPermissions
            = new List<(TUserId UserId, TOperationId OperationId, TPermission Permission)>();


        /// <summary>
        /// Here we store Which GroupId belongs to which GroupTypeId
        /// </summary>
        private readonly List<(TGroupId GroupId, TGroupTypeId GroupTypeId)> _groupTypes =
            new List<(TGroupId GroupId, TGroupTypeId GroupTypeId)>();


        /// <summary>
        /// When someone changes settings this event will fire
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// When some business exception occurs this event will fire
        /// Add custom handler to this event and throw if you dont want to bury exceptions or just log them
        /// </summary>
        public event EventHandler<AditumException> ExceptionOccured;

        

        /// <summary>
        /// This will control synchronization between getting and setting settings across multiple threads
        /// </summary>
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        /// <summary>
        /// If you want to use custom Id types implement this interface for legacy serialization
        /// </summary>
        public ISerializeStrategy<TUserId, TGroupId, TGroupTypeId, TOperationId, TPermission> SerializeStrategy { get; }

        /// <summary>
        /// If a user has multiple groups and each group has different permission of given operation
        /// this strategy will be used to select one among them
        /// </summary>
        public IPermissionSelectStrategy<TGroupId, TGroupTypeId,TPermission> PermissionSelectStrategy
        {
            get;
        }

        public UserService(
            IPermissionSelectStrategy<TGroupId, TGroupTypeId, TPermission>
                permissionSelectStrategy,
            ISerializeStrategy<TUserId, TGroupId, TGroupTypeId, TOperationId, TPermission> serializeStrategy
        )
        {
            PermissionSelectStrategy = permissionSelectStrategy;
            SerializeStrategy = serializeStrategy;
        }
    }
}