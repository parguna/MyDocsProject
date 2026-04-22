using Volo.Abp.Modularity;

namespace MyDocsProject;

/* Inherit from this class for your domain layer tests. */
public abstract class MyDocsProjectDomainTestBase<TStartupModule> : MyDocsProjectTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
