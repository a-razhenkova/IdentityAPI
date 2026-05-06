using Application;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Shared;

namespace WebApi
{
    public static class LoggerSetup
    {
        public static WebApplicationBuilder AddLogger(this WebApplicationBuilder builder)
        {
            Logger logger = builder.CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            return builder;
        }

        private static Logger CreateLogger(this WebApplicationBuilder builder)
        {
            var logSettings = builder.Configuration.GetRequiredSection<LogSettings>(nameof(AppSettings.Log));

            var loggerConfig = new LoggerConfiguration()
                .SetMinimumLevel()
                .WriteToConsole();

            loggerConfig = loggerConfig.WriteToFile(logSettings);

            loggerConfig = loggerConfig.Enrich.FromLogContext()
                .Enrich.WithProperty(LoggerContextProperty.Environment.ToString(), builder.Environment.EnvironmentName)
                .Enrich.WithProperty(LoggerContextProperty.Version.ToString(), WebApiAssembly.GetVersion())
                .Enrich.WithProperty(LoggerContextProperty.MachineName.ToString(), Environment.MachineName);

            return loggerConfig.CreateLogger();
        }

        private static LoggerConfiguration WriteToConsole(this LoggerConfiguration loggerConfig)
        {
            return loggerConfig.WriteTo.Async(cfg =>
            {
                cfg.Console(outputTemplate: Shared.Constants.SerilogOutputTemplate,
                    restrictedToMinimumLevel: LogEventLevel.Verbose);
            });
        }

        private static LoggerConfiguration WriteToFile(this LoggerConfiguration loggerConfig, LogSettings logSettings)
        {
            return loggerConfig.WriteTo.Async(cfg =>
            {
                cfg.File(new CompactJsonFormatter(),
                    path: logSettings.File.Path,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    rollingInterval: RollingInterval.Hour,
                    retainedFileCountLimit: null,
                    fileSizeLimitBytes: logSettings.File.MaxMbSize * 1024 * 1024,
                    rollOnFileSizeLimit: true);
            });
        }

        private static LoggerConfiguration SetMinimumLevel(this LoggerConfiguration loggerConfig)
        {
            return loggerConfig.MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Query", LogEventLevel.Fatal)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Fatal);
        }
    }
}