using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesTracker.EmailEngine.Interfaces;
using SalesTracker.Shared.Messages;
using System.Text;
using System.Text.Json;

namespace SalesTracker.EmailEngine.Background
{
    public class StockAlertProcessor : BackgroundService
    {
        private readonly ILogger<StockAlertProcessor> _logger;
        private readonly QueueClient _queueClient;
        private readonly IEmailSender _emailSender;

        public StockAlertProcessor(
            ILogger<StockAlertProcessor> logger,
            QueueClient queueClient,
            IEmailSender emailSender)
        {
            _logger = logger;
            _queueClient = queueClient;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("📦 StockAlertProcessor started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var response = await _queueClient.ReceiveMessagesAsync(
                        maxMessages: 5,
                        visibilityTimeout: TimeSpan.FromSeconds(30),
                        cancellationToken: stoppingToken);

                    foreach (var msg in response.Value)
                    {
                        try
                        {
                            _logger.LogInformation("📥 Received queue message: {MessageText}", msg.MessageText);

                           
                            string json;
                            try
                            {
                                json = Encoding.UTF8.GetString(Convert.FromBase64String(msg.MessageText));
                                _logger.LogInformation("🔍 Decoded Base64 message.");
                            }
                            catch (FormatException)
                            {
                                json = msg.MessageText; 
                                _logger.LogInformation("🔍 Message is plain JSON.");
                            }

                            var alert = JsonSerializer.Deserialize<LowStockAlertMessage>(json);
                            if (alert == null)
                            {
                                _logger.LogWarning("⚠️ Deserialized alert is null. Skipping.");
                                continue;
                            }

                            _logger.LogInformation("📨 Sending low stock alert for product {ProductName}", alert.ProductName);
                            await _emailSender.SendLowStockAlertAsync(alert);

                            await _queueClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt, stoppingToken);
                            _logger.LogInformation("✅ Message deleted for product {ProductName}", alert.ProductName);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Failed to process alert message: {MessageText}", msg.MessageText);
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Poll interval
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "🚨 Error while polling queue.");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Backoff on failure
                }
            }

            _logger.LogInformation("🛑 StockAlertProcessor stopped.");
        }
    }
}
