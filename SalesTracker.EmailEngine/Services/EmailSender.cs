using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SalesTracker.EmailEngine.Interfaces;
using SalesTracker.EmailEngine.Settings;
using SalesTracker.InfraStructure.Responses;
using SalesTracker.Shared.Messages;

namespace SalesTracker.EmailEngine.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<SmtpSettings> options, ILogger<EmailSender> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendSummaryEmailAsync(DailySalesData summary)
        {
            try
            {
                _logger.LogInformation("📧 Preparing to send daily summary email...");

                var message = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail),
                    Subject = $"📊 Daily Sales Summary – {DateTime.Today:MMMM dd, yyyy}",
                    IsBodyHtml = true,
                    Body = GenerateSummaryBody(summary)
                };

                foreach (var recipient in _settings.Recipients)
                {
                    message.To.Add(recipient);
                }

                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                    EnableSsl = true
                };

                _logger.LogInformation("SMTP Host: {Host}, Port: {Port}, Sender: {Sender}, Recipients: {Recipients}",
                    _settings.Host, _settings.Port, _settings.SenderEmail, string.Join(", ", _settings.Recipients));

                await client.SendMailAsync(message);
                _logger.LogInformation("✅ Daily summary email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send daily summary email.");
            }
        }

        public async Task SendLowStockAlertAsync(LowStockAlertMessage alert)
        {
            try
            {
                _logger.LogInformation("📨 Preparing to send low stock alert for product {ProductName}", alert.ProductName);

                var message = new MailMessage
                {
                    From = new MailAddress(_settings.SenderEmail),
                    Subject = $"⚠️ Low Stock Alert – {alert.ProductName}",
                    IsBodyHtml = true,
                    Body = GenerateLowStockBody(alert)
                };

                foreach (var recipient in _settings.Recipients)
                {
                    message.To.Add(recipient);
                }

                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                    EnableSsl = true
                };

                _logger.LogInformation("SMTP Host: {Host}, Port: {Port}, Sender: {Sender}, Recipients: {Recipients}",
                    _settings.Host, _settings.Port, _settings.SenderEmail, string.Join(", ", _settings.Recipients));


                await client.SendMailAsync(message);
                _logger.LogInformation("✅ Low stock alert email sent for product {ProductName}", alert.ProductName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send low stock alert for product {ProductName}", alert.ProductName);
            }
        }

        private string GenerateSummaryBody(DailySalesData? summary)
        {
            var summaryDate = DateTime.Today.ToString("MMMM dd, yyyy");

            if (summary == null)
            {
                return $@"
                    <html>
                        <body style='font-family:Segoe UI, sans-serif;'>
                            <h2>🔔 Sales Summary for {summaryDate}</h2>
                            <p>No sales recorded for this day.</p>
                            <table style='border-collapse:collapse;'>
                                <tr><td><strong>Total Sales:</strong></td><td>$0.00</td></tr>
                                <tr><td><strong>Quantity Sold:</strong></td><td>0</td></tr>
                                <tr><td><strong>Top Product:</strong></td><td>No product</td></tr>
                            </table>
                            <p style='margin-top:20px;'>💬 Sent from SalesTracker Engine</p>
                        </body>
                    </html>";
            }

            return $@"
                <html>
                    <body style='font-family:Segoe UI, sans-serif;'>
                        <h2>🔔 Sales Summary for {summaryDate}</h2>
                        <table style='border-collapse:collapse;'>
                            <tr><td><strong>Total Sales:</strong></td><td>{summary.TotalSales:C}</td></tr>
                            <tr><td><strong>Quantity Sold:</strong></td><td>{summary.QuantitySold}</td></tr>
                            <tr><td><strong>Top Product:</strong></td><td>{summary.TopProductName} ({summary.TopProductQuantity})</td></tr>
                        </table>
                        <p style='margin-top:20px;'>💬 Sent from SalesTracker Engine</p>
                    </body>
                </html>";
        }

        private string GenerateLowStockBody(LowStockAlertMessage alert)
        {
            return $@"
                <html>
                    <body style='font-family:Segoe UI, sans-serif;'>
                        <h2>⚠️ Low Stock Alert</h2>
                        <table style='border-collapse:collapse;'>
                            <tr><td><strong>Product ID:</strong></td><td>{alert.ProductId}</td></tr>
                            <tr><td><strong>Product Name:</strong></td><td>{alert.ProductName}</td></tr>
                            <tr><td><strong>Current Stock:</strong></td><td>{alert.CurrentStock}</td></tr>
                            <tr><td><strong>Timestamp:</strong></td><td>{alert.Timestamp:yyyy-MM-dd HH:mm:ss} UTC</td></tr>
                        </table>
                        <p style='margin-top:20px;'>📦 Please restock promptly to avoid disruption.</p>
                        <p>💬 Sent from SalesTracker Engine</p>
                    </body>
                </html>";
        }
    }
}
