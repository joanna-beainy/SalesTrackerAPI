using SalesTracker.InfraStructure.Responses;
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
    }
}
