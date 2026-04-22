using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyDocsProject.Data;
using Volo.Abp.DependencyInjection;

namespace MyDocsProject.EntityFrameworkCore;

public class EntityFrameworkCoreMyDocsProjectDbSchemaMigrator
    : IMyDocsProjectDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMyDocsProjectDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the MyDocsProjectDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MyDocsProjectDbContext>()
            .Database
            .MigrateAsync();
    }
}
