using Volo.Abp.Account;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Docs;
using Volo.Docs.Admin;

namespace MyDocsProject;

[DependsOn(
    typeof(MyDocsProjectDomainSharedModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule)
)]
[DependsOn(typeof(DocsApplicationContractsModule))]
    [DependsOn(typeof(DocsAdminApplicationContractsModule))]
    public class MyDocsProjectApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyDocsProjectDtoExtensions.Configure();
    }
}
