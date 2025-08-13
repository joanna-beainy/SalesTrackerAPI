using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesTracker.EmailEngine.Interfaces;
using SalesTracker.InfraStructure.Interfaces;
namespace SalesTracker.EmailEngine.Background
{
    public class DailySummarySender : IHostedService
    {
        private readonly ILogger<DailySummarySender> _logger;
        private readonly IServiceProvider _provider;

        public DailySummarySender(ILogger<DailySummarySender> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var saleRepo = scope.ServiceProvider.GetRequiredService<ISaleRepository>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            try
            {
                var today = DateTime.Today;
                var summary = await saleRepo.GetAggregatedSalesByDateAsync(today);
                _logger.LogInformation("📊 Summary retrieved: {Summary}", summary == null ? "null" : "valid");

                await emailSender.SendSummaryEmailAsync(summary);
                _logger.LogInformation("📧 Sales summary email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Failed to send sales summary email.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
