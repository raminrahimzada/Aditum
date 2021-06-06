namespace Aditum.Core
{
    public interface IPermissionSelectStrategy<TGroupId, TGroupTypeId, TPermission>
    {
        TPermission Decide(TPermission exclusivePermission,(TGroupId, TGroupTypeId, TPermission)[] groupPermissions);
        TPermission Decide((TGroupId, TGroupTypeId, TPermission)[] groupPermissions);
    }
}