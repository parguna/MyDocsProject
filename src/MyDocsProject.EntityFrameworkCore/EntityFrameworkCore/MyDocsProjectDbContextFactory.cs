using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyDocsProject.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MyDocsProjectDbContextFactory : IDesignTimeDbContextFactory<MyDocsProjectDbContext>
{
    public MyDocsProjectDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        MyDocsProjectEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<MyDocsProjectDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new MyDocsProjectDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MyDocsProject.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}
