using System.Threading.Tasks;

namespace MyDocsProject.Data;

public interface IMyDocsProjectDbSchemaMigrator
{
    Task MigrateAsync();
}
