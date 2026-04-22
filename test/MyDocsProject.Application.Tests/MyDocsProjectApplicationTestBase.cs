using Volo.Abp.Modularity;

namespace MyDocsProject;

public abstract class MyDocsProjectApplicationTestBase<TStartupModule> : MyDocsProjectTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
