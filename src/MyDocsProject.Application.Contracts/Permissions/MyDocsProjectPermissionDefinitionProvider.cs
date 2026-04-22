using MyDocsProject.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MyDocsProject.Permissions;

public class MyDocsProjectPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MyDocsProjectPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(MyDocsProjectPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MyDocsProjectResource>(name);
    }
}
