//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

using Microsoft.WindowsAzure.Server.Common;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Providers;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Controllers
{
    /// <summary>
    /// VMM Servers Controller Class
    /// </summary>
    public class VmmServersController : ActivityApiController
    {
        private ConfigStorageClient configStorageClient;
        private VmmServerClient vmmServerClient;
        private VmmServersProvider vmmServersProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="VmmServersController"/> class.
        /// </summary>
        /// <param name="configStorageClient">The config storage client</param>
        /// <param name="vmmServerClient">The VMM server client</param>
        public VmmServersController(ConfigStorageClient configStorageClient, VmmServerClient vmmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.vmmServerClient = vmmServerClient;
            this.vmmServersProvider = new VmmServersProvider(configStorageClient, vmmServerClient);
        }

        /// <summary>
        /// Gets a VMM server collection
        /// </summary>
        /// <returns>VMM server collection</returns>
        [HttpGet]
        public async Task<VmmServerList> ListVmmServers()
        {
            return await this.vmmServersProvider.ListVmmServers();
        }

        /// <summary>
        /// Posts a VMM Server
        /// </summary>
        /// <param name="server">The server to post</param>
        /// <returns>Post response</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> InsertVmmSever(VmmServer server)
        {
            VmmServer vmmServer = await this.vmmServersProvider.InsertVmmServer(server);

            return Request.CreateResponse<VmmServer>(HttpStatusCode.Created, vmmServer);
        }

        /// <summary>
        /// Updates the VMM Server connection 
        /// </summary>
        /// <param name="server">The server to update</param>
        /// <returns>The updated server</returns>
        [HttpPut]
        public async Task<VmmServer> UpdateVmmServer(VmmServer server, string stampId)
        {
            return await this.vmmServersProvider.UpdateVmmServer(server, stampId);
        }
    }
}