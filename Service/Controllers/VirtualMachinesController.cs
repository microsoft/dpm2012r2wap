//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.WindowsAzure.Server.Common;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Providers;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Controllers
{
    /// <summary>
    /// Virtual Machines Controller Class
    /// </summary>
    public class VirtualMachinesController : ActivityApiController
    {
        private ConfigStorageClient configStorageClient;
        private DpmServerClient dpmServerClient;
        private VirtualMachinesProvider virtualMachinesProvider;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualMachinesController"/> class.
        /// </summary>
        /// <param name="configStorageClient">The config storage client</param>
        /// <param name="dpmServerClient">The DPM server client</param>
        public VirtualMachinesController(ConfigStorageClient configStorageClient, DpmServerClient dpmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.dpmServerClient = dpmServerClient;
            this.virtualMachinesProvider = new VirtualMachinesProvider(configStorageClient, dpmServerClient);
        }

        /// <summary>
        /// Backs up a virtual machine
        /// </summary>
        /// <param name="virtualMachineToBackup">The virtual machine to back up</param>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <param name="id">The virtual machine identifier</param>
        /// <returns>The virtual machine</returns>
        [HttpPut]
        public async Task<VirtualMachine> BackupVirtualMachine(VirtualMachine virtualMachineToBackup, string subscriptionId, string id)
        {
            return await this.virtualMachinesProvider.BackupVirtualMachine(virtualMachineToBackup, subscriptionId, id);
        }

        /// <summary>
        /// Gets the collection of recovery points for a virtual machine
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <param name="id">The virtual machine identifier</param>
        /// <returns>The recovery point collection</returns>
        [HttpGet]
		public async Task<RecoveryPointList> ListVirtualMachineRestorePoints (string subscriptionId, string id)
        {
            return await this.virtualMachinesProvider.ListVirtualMachineRestorePoints(subscriptionId, id);
        }

        /// <summary>
        /// Get the virtual machine collection for a subscription
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <returns>The virtual machine collection</returns>
        [HttpGet]
        public async Task<VirtualMachineList> ListVirtualMachines(string subscriptionId)
        {
            var queryParams = Request.RequestUri.ParseQueryString();

            bool includeDetails = true;
            if (queryParams.Count > 0)
            {
                includeDetails = this.ParseBooleanQuery(queryParams, "IncludeDetails");
            }

            return await virtualMachinesProvider.ListVirtualMachines(subscriptionId, includeDetails);
        }

        /// <summary>
        /// Protects an unprotected virtual machine
        /// </summary>
        /// <param name="virtualMachineToProtect">The virtual machine to protect</param>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <returns>Post response</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> ProtectVirtualMachine(VirtualMachine virtualMachineToProtect, string subscriptionId)
        {
            VirtualMachine vm = await this.virtualMachinesProvider.AddVmToProtectionGroup(virtualMachineToProtect);
            return Request.CreateResponse<VirtualMachine>(HttpStatusCode.Created, vm);
        }

        /// <summary>
        /// Removes the virtual machine from protection
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <param name="id">The virtual machine identifier</param>
        [HttpDelete]
        public async Task RemoveVirtualMachineProtection (string subscriptionId, string id)
        {
            await this.virtualMachinesProvider.RemoveVirtualMachineProtection(subscriptionId, id);
        }

        /// <summary>
        /// Restores a virtual machine to a recovery point
        /// </summary>
        /// <param name="virtualMachineToRestore">The virtual machine to restore</param>
        /// <param name="subscriptionId">The subscription identifier</param>
        /// <param name="id">The virtual machine identifier</param>
        /// <param name="recoverySourceId">The recovery source identifier</param>
        [HttpPut]
        public async Task RestoreVirtualMachine(VirtualMachine virtualMachineToRestore, string subscriptionId, string id, string recoverySourceId)
        {
            await this.virtualMachinesProvider.RestoreVirtualMachine(virtualMachineToRestore, subscriptionId, id, recoverySourceId);
        }


        private bool ParseBooleanQuery(NameValueCollection query, string key)
        {
            bool value = false;

            if (query.HasKeys() && query.AllKeys.Contains(key) && bool.TryParse(query[key], out value))
            {
            }
            
            return value;
        }
    }
}