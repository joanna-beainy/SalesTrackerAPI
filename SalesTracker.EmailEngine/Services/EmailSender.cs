using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SalesTracker.EmailEngine.Settings;
using SalesTracker.EmailEngine.Interfaces;
using SalesTracker.InfraStructure.Responses;

namespace SalesTracker.EmailEngine.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public EmailSender(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendSummaryEmailAsync(DailySalesData summary)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail),
                Subject = $"📊 Daily Sales Summary – {DateTime.Today:MMMM dd, yyyy}",
                IsBodyHtml = true,
                Body = GenerateEmailBody(summary)
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

            await client.SendMailAsync(message);
        }

        private string GenerateEmailBody(DailySalesData? summary)
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

    }
}
