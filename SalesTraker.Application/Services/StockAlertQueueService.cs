using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using SalesTracker.Application.Interfaces;
using SalesTracker.Shared.Messages;
using SalesTracker.Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SalesTracker.Application.Services
{
    public class StockAlertQueueService : IStockAlertQueueService
    {
        private readonly QueueClient _queueClient;

        public StockAlertQueueService(IOptions<AzureQueueOptions> options)
        {
            _queueClient = new QueueClient(options.Value.ConnectionString, options.Value.QueueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task EnqueueAsync(LowStockAlertMessage message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

                if (_queueClient == null)
                    throw new InvalidOperationException("QueueClient is not initialized.");

                if (!_queueClient.Exists())
                    throw new InvalidOperationException("Queue does not exist.");

                await _queueClient.SendMessageAsync(base64);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Queue enqueue failed: {ex.Message}");
                throw; // Optional: rethrow to bubble up
            }
        }

    }

}
