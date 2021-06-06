using System;
using System.Runtime.CompilerServices;

namespace Aditum.Core
{
    public partial class UserService<TUserId,TGroupId,TGroupTypeId,TOperationId, TPermission> 
        where TUserId:IComparable<TUserId>
        where TGroupId:IComparable<TGroupId>
        where TGroupTypeId : IComparable<TGroupTypeId>
    {

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
    }
}