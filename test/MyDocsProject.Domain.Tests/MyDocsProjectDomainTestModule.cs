using Volo.Abp.Modularity;

namespace MyDocsProject;

[DependsOn(
    typeof(MyDocsProjectDomainModule),
    typeof(MyDocsProjectTestBaseModule)
)]
public class MyDocsProjectDomainTestModule : AbpModule
{

}
