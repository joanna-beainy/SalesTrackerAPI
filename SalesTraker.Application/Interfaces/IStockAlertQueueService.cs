using SalesTracker.InfraStructure.Models.Entities;
using SalesTracker.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Application.Interfaces
{
    public interface IStockAlertQueueService
    {
        Task EnqueueAsync(LowStockAlertMessage message);
    }
}
