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
    /// Backup Servers Controller Class
    /// </summary>
    public class BackupServersController : ActivityApiController
    {
        private ConfigStorageClient configStorageClient;
        private DpmServerClient dpmServerClient;
        private BackupServersProvider backupServersProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupServersController"/> class.
        /// </summary>
        /// <param name="configStorageClient">The config storage client</param>
        /// <param name="dpmServerClient">The DPM server client</param>
        public BackupServersController(ConfigStorageClient configStorageClient, DpmServerClient dpmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.dpmServerClient = dpmServerClient;
            this.backupServersProvider = new BackupServersProvider(configStorageClient, dpmServerClient);
        }

        /// <summary>
        /// Posts a backup server
        /// </summary>
        /// <param name="backupServer">The backup server to post</param>
        /// <returns>Post response</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> CreateBackupSever(BackupServer backupServer)
        {
            BackupServer server = await this.backupServersProvider.CreateBackupServer(backupServer);

            return Request.CreateResponse<BackupServer>(HttpStatusCode.Created, server);
        }

        /// <summary>
        /// Gets a backup server collection
        /// </summary>
        /// <returns>Backup server collection</returns>
        [HttpGet]
        public async Task<BackupServerList> ListBackupServers()
        {
            return await this.backupServersProvider.ListBackupServers();
        }

        /// <summary>
        /// Updates the Backup Server connection 
        /// </summary>
        /// <param name="server">The server to update</param>
        /// <param name="serverId">The server identifier</param>
        /// <returns>The updated server</returns>
        [HttpPut]
        public async Task<BackupServer> UpdateBackupServer(BackupServer server, string serverId)
        {
            return await this.backupServersProvider.UpdateBackupServer(server, serverId);
        }

        /// <summary>
        /// Deletes a Backup Server
        /// </summary>
        /// <param name="serverId">The server identifier</param>
        [HttpDelete]
        public async Task DeleteBackupServer(string serverId)
        {
            await this.backupServersProvider.DeleteBackupServer(serverId);
        }
    }
}