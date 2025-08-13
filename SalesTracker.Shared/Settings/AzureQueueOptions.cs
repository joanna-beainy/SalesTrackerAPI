using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Shared.Settings
{
    public class AzureQueueOptions
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
