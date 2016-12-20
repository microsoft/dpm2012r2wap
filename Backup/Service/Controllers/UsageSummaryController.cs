using Microsoft.WindowsAzure.Server.Common;
using Microsoft.WindowsAzurePack.ApiClients.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Controllers
{
    public class UsageSummaryController : ActivityApiController
    {

        /// <summary>
        /// Gets the usage summary by subscription id.
        /// </summary>
        [HttpGet]
        public async Task<ServiceUsageSummary> GetUsageSummaryBySubscriptionId(string subscriptionId)
        {
            Guid id;
            bool parseSucceeded = Guid.TryParse(subscriptionId, out id);
            ServiceUsageSummary usageSummary = new ServiceUsageSummary()
            {
                ServiceName = "VmBackup",
                ServiceDisplayName = "Virtual Machine Backup",
                RetrievedSuccessfully = true,
                Usages = new List<Usage>()
            };

            return usageSummary;
        }
    }
}