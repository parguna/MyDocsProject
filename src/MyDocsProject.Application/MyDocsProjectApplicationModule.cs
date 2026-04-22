using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.TenantManagement;
using Volo.Docs;
using Volo.Docs.Admin;

namespace MyDocsProject;

[DependsOn(
    typeof(MyDocsProjectDomainModule),
    typeof(MyDocsProjectApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
[DependsOn(typeof(DocsApplicationModule))]
    [DependsOn(typeof(DocsAdminApplicationModule))]
    public class MyDocsProjectApplicationModule : AbpModule
{

}
