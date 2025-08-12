using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesTracker.Shared.Settings
{
    public class AzureBlobOptions
    {
        public string ConnectionString { get; set; }
        public string Container { get; set; }
        public long MaxUploadBytes { get; set; }
    }
}
