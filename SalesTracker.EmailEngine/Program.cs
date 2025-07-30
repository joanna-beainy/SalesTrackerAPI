using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration.Binder;
using SalesTracker.EmailEngine;
using SalesTracker.InfraStructure.Data;
using SalesTracker.InfraStructure.Interfaces;
using SalesTracker.InfraStructure.Repositories;
using SalesTracker.EmailEngine.Interfaces;
using SalesTracker.EmailEngine.Settings;
using SalesTracker.EmailEngine.Services;


class Program
{
    static async Task Main(string[] args)
    {
        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Set up dependency injection
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(logging =>
        {
            logging.AddConsole();
        });

        services.AddSingleton<IConfiguration>(configuration);

        // Bind SMTP settings
        services.Configure<SmtpSettings>(options => configuration.GetSection("SmtpSettings").Bind(options));

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register application and infrastructure services
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IEmailSender, EmailSender>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Create scope for scoped services
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var logger = scopedProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var saleRepo = scopedProvider.GetRequiredService<ISaleRepository>();
            var emailSender = scopedProvider.GetRequiredService<IEmailSender>();

            var today = DateTime.Today;
            var summary = await saleRepo.GetAggregatedSalesByDateAsync(today);

            await emailSender.SendSummaryEmailAsync(summary);

            logger.LogInformation("📧 Sales summary email sent successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "🚨 Failed to send sales summary email.");
        }
    }
}
