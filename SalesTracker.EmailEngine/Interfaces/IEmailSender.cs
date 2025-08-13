using SalesTracker.InfraStructure.Responses;
using SalesTracker.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.EmailEngine.Interfaces
{
    public interface IEmailSender
    {
        Task SendSummaryEmailAsync(DailySalesData summary);
        Task SendLowStockAlertAsync(LowStockAlertMessage alert);
    }
}
