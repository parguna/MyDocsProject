using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyDocsProject.Data;

/* This is used if database provider does't define
 * IMyDocsProjectDbSchemaMigrator implementation.
 */
public class NullMyDocsProjectDbSchemaMigrator : IMyDocsProjectDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
