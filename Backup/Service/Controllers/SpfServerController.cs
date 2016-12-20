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
    /// SPF Server Controller Class
    /// </summary>
    public class SpfServerController : ActivityApiController
    {
        private ConfigStorageClient configStorageClient;
        private VmmServerClient vmmServerClient;
        private VmmServersProvider vmmServersProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="VmmServersController"/> class.
        /// </summary>
        /// <param name="configStorageClient">The config storage client</param>
        /// <param name="vmmServerClient">The VMM server client</param>
        public SpfServerController(ConfigStorageClient configStorageClient, VmmServerClient vmmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.vmmServerClient = vmmServerClient;
            this.vmmServersProvider = new VmmServersProvider(configStorageClient, vmmServerClient);
        }

        /// <summary>
        /// Posts the SPF server
        /// </summary>
        /// <param name="server">The server to post</param>
        /// <returns>Post response</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SetSpfSever(SpfServer server)
        {
            SpfServer spfServer = await this.vmmServersProvider.SetSpfServer(server);

            return Request.CreateResponse<SpfServer>(HttpStatusCode.Created, spfServer);
        }
    }
}