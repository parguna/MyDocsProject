using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyDocsProject.Data;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace MyDocsProject.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Compose's SQL Server healthcheck already gates this container's startup, but that
        // doesn't cover a restart or a brief blip after the healthcheck passed — wait for a
        // real ADO.NET connection here too. Deliberately a plain connection probe, not EF
        // Core's EnableRetryOnFailure: that retrying execution strategy is incompatible with
        // ABP's own explicit-transaction Unit-of-Work usage during data seeding (confirmed by
        // testing — it throws "does not support user-initiated transactions").
        await WaitForDatabaseAsync(cancellationToken);

        using (var application = await AbpApplicationFactory.CreateAsync<MyDocsProjectDbMigratorModule>(options =>
        {
           options.Services.ReplaceConfiguration(_configuration);
           options.UseAutofac();
           options.Services.AddLogging(c => c.AddSerilog());
           options.AddDataMigrationEnvironment();
        }))
        {
            await application.InitializeAsync();

            await application
                .ServiceProvider
                .GetRequiredService<MyDocsProjectDbMigrationService>()
                .MigrateAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    private async Task WaitForDatabaseAsync(CancellationToken cancellationToken)
    {
        // Probe the server itself, not the app's target database — on a genuinely fresh SQL
        // Server the app's database doesn't exist yet (the migrator's job is to create it), so
        // connecting with the real "Database=..." connection string would always fail here
        // with a "cannot open database" login error that has nothing to do with server
        // readiness. Redirect the probe connection to "master", which always exists.
        var builder = new SqlConnectionStringBuilder(_configuration.GetConnectionString("Default"))
        {
            InitialCatalog = "master"
        };
        var probeConnectionString = builder.ConnectionString;
        const int maxAttempts = 30;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using var connection = new SqlConnection(probeConnectionString);
                await connection.OpenAsync(cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                if (attempt == maxAttempts)
                {
                    Log.Error(ex, "Could not connect to the database after {MaxAttempts} attempts, giving up.", maxAttempts);
                    throw;
                }

                Log.Warning("Database not reachable yet (attempt {Attempt}/{MaxAttempts}): {Message}", attempt, maxAttempts, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
