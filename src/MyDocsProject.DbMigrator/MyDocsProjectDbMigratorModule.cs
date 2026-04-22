using MyDocsProject.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyDocsProject.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyDocsProjectEntityFrameworkCoreModule),
    typeof(MyDocsProjectApplicationContractsModule)
)]
public class MyDocsProjectDbMigratorModule : AbpModule
{
}
