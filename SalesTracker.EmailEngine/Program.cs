using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Azure.Storage.Queues;
using SalesTracker.EmailEngine.Interfaces;
using SalesTracker.EmailEngine.Services;
using SalesTracker.EmailEngine.Background;
using SalesTracker.EmailEngine.Settings;
using SalesTracker.InfraStructure.Data;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Repositories;
using SalesTracker.Shared.Settings;

// 🔧 Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341") 
    .CreateLogger();

try
{
    Log.Information("🚀 Starting SalesTracker.EmailEngine...");

    await Host.CreateDefaultBuilder(args)
        .UseSerilog() 
        .ConfigureAppConfiguration(config =>
        {
            config.SetBasePath(AppContext.BaseDirectory);
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .ConfigureServices((context, services) =>
        {
            var configuration = context.Configuration;

            // 🔧 Bind settings
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.Configure<AzureQueueOptions>(configuration.GetSection("AzureQueue"));

            // 🔧 Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 🔧 Register services
            services.AddScoped<ISaleRepository, SaleRepository>();
            services.AddScoped<IEmailSender, EmailSender>();

            // 🔧 Queue client
            services.AddSingleton<QueueClient>(provider =>
            {
                var options = provider.GetRequiredService<IOptions<AzureQueueOptions>>().Value;
                return new QueueClient(options.ConnectionString, options.QueueName);
            });

            // 🔧 Background services
            services.AddHostedService<StockAlertProcessor>();
            services.AddHostedService<DailySummarySender>();

            // ✅ Use Serilog for logging
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });
        })
        .RunConsoleAsync();

    Log.Information("✅ SalesTracker.EmailEngine stopped gracefully.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ SalesTracker.EmailEngine terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
