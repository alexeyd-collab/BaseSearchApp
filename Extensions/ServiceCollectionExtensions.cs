using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using SearchApp.Constants;
using SearchApp.Models;
using SearchApp.Services;
using SearchApp.Services.Storage;

namespace SearchApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAppLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Async(a => a.File(AppConstants.Logging.LogFilePath, rollingInterval: RollingInterval.Day))
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(AppConstants.Telemetry.ServiceName))
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddConsoleExporter();
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddConsoleExporter();
                });

            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.AddConsoleExporter();
            });
        }

        public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GitHubApiSettings>(configuration.GetSection(AppConstants.Configuration.GitHubApiSettingsSection));
            services.Configure<StorageSettings>(configuration.GetSection(AppConstants.Configuration.StorageSettingsSection));
        }

        public static void AddAppHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var githubSettings = configuration.GetSection(AppConstants.Configuration.GitHubApiSettingsSection).Get<GitHubApiSettings>() ?? new GitHubApiSettings();

            services.AddHttpClient<IRepoSearchService, RepoSearchService>(client =>
            {
                client.DefaultRequestHeaders.Add(AppConstants.Headers.Accept, githubSettings.DefaultAcceptHeader);
                client.DefaultRequestHeaders.Add(AppConstants.Headers.UserAgent, githubSettings.UserAgent);
            });
        }

        public static void AddAppStorage(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddSingleton<MemorySearchResultStorage>();
            services.AddSingleton<DistributedCacheSearchResultStorage>();
            services.AddSingleton<ISearchResultStorageFactory, SearchResultStorageFactory>();
        }

        public static void AddAppCoreServices(this IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHttpContextAccessor();
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                        options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                    });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddTransient<ISessionManagerService, SessionManagerService>();
        }
    }
}
