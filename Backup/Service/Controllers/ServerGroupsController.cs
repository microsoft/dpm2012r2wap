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
    /// Server Groups Controller Class
    /// </summary>
    public class ServerGroupsController : ActivityApiController
    {
        private ConfigStorageClient configStorageClient;
        private ServerGroupsProvider serverGroupsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerGroupsController"/> class.
        /// </summary>
        /// <param name="configStorageClient">The config storage client</param>
        public ServerGroupsController(ConfigStorageClient configStorageClient)
        {
            this.configStorageClient = configStorageClient;
            this.serverGroupsProvider = new ServerGroupsProvider(configStorageClient);
        }

        /// <summary>
        /// Posts a server group
        /// </summary>
        /// <param name="serverGroup">The group to post</param>
        /// <returns>Post response</returns>
        [HttpPost]
        public async Task<HttpResponseMessage> CreateServerGroup(ServerGroup serverGroup)
        {
            ServerGroup group = await this.serverGroupsProvider.CreateServerGroup(serverGroup);

            return Request.CreateResponse<ServerGroup>(HttpStatusCode.Created, group);
        }

        /// <summary>
        /// Gets a server group collection
        /// </summary>
        /// <returns>The server group collection</returns>
        [HttpGet]
        public async Task<ServerGroupList> ListServerGroups()
        {
            return await this.serverGroupsProvider.ListServerGroups();
        }

        /// <summary>
        /// Deletes a Server Group
        /// </summary>
        /// <param name="groupId">The group identifier</param>
        [HttpDelete]
        public async Task DeleteServerGroup(string groupId)
        {
            await this.serverGroupsProvider.DeleteServerGroup(groupId);
        }
    }
}