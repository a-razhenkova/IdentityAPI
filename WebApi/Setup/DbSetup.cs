using Application;
using DbUp;
using DbUp.Engine;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;
using Shared;
using System.Text;

namespace WebApi
{
    public static class DbSetup
    {
        public static async Task<WebApplication> ApplyDbPendingMigrationsAndScriptsAsync(this WebApplication app)
        {
            var dbOptions = app.Configuration.GetRequiredSection<DatabaseSettings>(nameof(AppSettings.Database));

            using IServiceScope scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

            if (dbOptions.IsDbMigrationAllowed)
            {
                using (LogContext.PushProperty(LoggerContextProperty.ActionType.ToString(), LoggerContext.DbMigration))
                {
                    await identityContext.ApplyDbPendingMigrationsAsync(logger);
                }
            }

            if (dbOptions.IsDbUpAllowed)
            {
                using (LogContext.PushProperty(LoggerContextProperty.ActionType.ToString(), LoggerContext.DbUp))
                {
                    identityContext.ApplyDbPendingScriptsAsync(logger);
                }
            }

            return app;
        }

        private static async Task ApplyDbPendingMigrationsAsync(this DbContext dbContext, ILogger logger)
        {
            try
            {
                logger.LogInformation("Checking for pending migrations...");

                IEnumerable<string> migrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (migrations.Any())
                {
                    logger.LogInformation("Applying pending migrations...");

                    await dbContext.Database.MigrateAsync();

                    string dbName = dbContext.Database.GetDbConnection().Database;
                    logger.LogInformation("Applied migrations on database '{DbName}': {Migrations}", dbName, string.Join(", ", migrations));
                }

                logger.LogInformation("Database is up to date.");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }

        public static void ApplyDbPendingScriptsAsync(this DbContext dbContext, ILogger logger)
        {
            try
            {
                DatabaseAttribute dbConfig = dbContext.GetRequiredCustomAttribute<DatabaseAttribute>();
                string connectionString = dbContext.Database.GetConnectionString() ?? throw new NotImplementedException();

                var upgrader = DeployChanges.To
                    .SqlDatabase(connectionString)
                    .JournalToSqlTable(dbConfig.DefaultSchemaName, dbConfig.ScriptsHistoryTableName)
                    .WithScriptsEmbeddedInAssembly(InfrastructureAssembly.GetExecutingAssembly())
                    .Build();

                logger.LogInformation("Executing pending scripts...");

                DatabaseUpgradeResult result = upgrader.PerformUpgrade();

                if (result.Error is not null)
                    throw new InvalidOperationException($"Script '{result.ErrorScript.Name}' executed with error:\n'{result.Error.Message}'", result.Error);

                if (result.Scripts.Any())
                {
                    var message = new StringBuilder();
                    message.AppendLine("Executed scripts:");

                    foreach (SqlScript script in result.Scripts)
                        message.AppendLine(script.Name);

                    logger.LogInformation(message.ToString().TrimEnd());
                }

                logger.LogInformation("Database is up to date.");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}