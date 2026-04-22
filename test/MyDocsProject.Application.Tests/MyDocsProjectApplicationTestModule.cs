using Volo.Abp.Modularity;

namespace MyDocsProject;

[DependsOn(
    typeof(MyDocsProjectApplicationModule),
    typeof(MyDocsProjectDomainTestModule)
)]
public class MyDocsProjectApplicationTestModule : AbpModule
{

}
