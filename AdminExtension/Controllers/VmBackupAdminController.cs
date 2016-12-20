// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.ApiClients;
using Microsoft.WindowsAzurePack.ApiClients.DataContracts;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Models;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Common;
using System.Collections.Generic;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Controllers
{
    [RequireHttps]
    [OutputCache(Location = OutputCacheLocation.None)]
    [PortalExceptionHandler]
    public sealed class VmBackupAdminController : ExtensionController
    {
        private static readonly string adminAPIUri = OnPremPortalConfiguration.Instance.RdfeAdminUri;
        private const string ResourceProviderName = "vmbackup";

        //This model is used to show registered resource provider information
        public EndpointModel VmBackupServiceEndPoint { get; set; }


        /// <summary>
        /// Check if the resource provider has been registered
        /// </summary>
        /// <returns>A boolean value that indicates whether the resources has been registered</returns>
        [HttpPost]
        [ActionName("IsResourceProviderRegistered")]
        public async Task<JsonResult> IsResourceProviderRegistered()
        {
            try
            {
                var context = ClientFactory.AdminManagementClient;
                var resourceProviders = await context.ListResourceProvidersAsync();
                if (resourceProviders.Exists(x => string.Equals(ResourceProviderName, x.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    return this.JsonDataSet(true, pollingInterval: TimeSpan.FromHours(5), fastPollingInterval: TimeSpan.FromHours(5));
                }
                else
                {
                    return this.JsonDataSet(false, pollingInterval: TimeSpan.FromHours(5), fastPollingInterval: TimeSpan.FromHours(5));
                }
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("ListServerGroups")]
        public async Task<JsonResult> ListServerGroups()
        {
            try
            {
                var groupList = await ClientFactory.VmBackupClient.ListServerGroups();
                List<ServerGroupModel> groups = groupList.Select(g => new ServerGroupModel(g)).ToList();
                return this.JsonDataSet(groups);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("CreateServerGroup")]
        public async Task<JsonResult> CreateServerGroup(string groupName, bool azureBackupEnabled)
        {
            Requires.Argument<string>("groupName", groupName).NotNullOrEmpty();

            try
            {
                ServerGroup groupToPost = new ServerGroup();
                groupToPost.GroupName = groupName;
                groupToPost.AzureBackupEnabled = azureBackupEnabled;

                var serverGroup = await ClientFactory.VmBackupClient.CreateServerGroupAsync(groupToPost);
                ServerGroupModel groupModel = new ServerGroupModel(serverGroup);

                return Json(groupModel);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("DeleteServerGroup")]
        public async Task<JsonResult> DeleteServerGroup(Guid groupId)
        {
            Requires.Argument<Guid>("groupId", groupId).NotNull();

            try
            {
                await ClientFactory.VmBackupClient.DeleteServerGroupAsync(groupId);
                return Json(null);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("ListBackupServers")]
        public async Task<JsonResult> ListBackupServers()
        {
            try
            {
                var serverList = await ClientFactory.VmBackupClient.ListBackupServers();
                List<BackupServerModel> servers = serverList.Select(s => new BackupServerModel(s)).ToList();
                return this.JsonDataSet(servers);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("CreateBackupServer")]
        public async Task<JsonResult> CreateBackupServer(string serverName, string userName, string password, string groupId)
        {
            Requires.Argument<string>("serverName", serverName).NotNullOrEmpty();
            Requires.Argument<string>("userName", userName).NotNullOrEmpty();
            Requires.Argument<string>("password", password).NotNullOrEmpty();
            Requires.Argument<string>("groupId", groupId).ValidResourceId();

            try
            {
                BackupServer serverToPost = new BackupServer()
                {
                    ServerName = serverName,
                    UserName = userName,
                    Password = password,
                    GroupId = Guid.Parse(groupId)
                };
                var server = await ClientFactory.VmBackupClient.CreateBackupServerAsync(serverToPost);
                BackupServerModel serverModel = new BackupServerModel(server);

                return Json(serverModel);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("UpdateBackupServer")]
        public async Task<JsonResult> UpdateBackupServer(Guid serverId, string serverName, string userName, string password)
        {
            Requires.Argument<Guid>("serverId", serverId).NotNull();
            Requires.Argument<string>("serverName", serverName).NotNullOrEmpty();
            Requires.Argument<string>("userName", userName).NotNullOrEmpty();
            Requires.Argument<string>("password", password).NotNullOrEmpty();

            try
            {
                BackupServer serverToPost = new BackupServer()
                {
                    ServerId = serverId,
                    ServerName = serverName,
                    UserName = userName,
                    Password = password
                };
                var server = await ClientFactory.VmBackupClient.UpdateBackupServerAsync(serverToPost);
                BackupServerModel serverModel = new BackupServerModel(server);

                return Json(serverModel);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("DeleteBackupServer")]
        public async Task<JsonResult> DeleteBackupServer(Guid serverId)
        {
            Requires.Argument<Guid>("serverId", serverId).NotNull();

            try
            {
                await ClientFactory.VmBackupClient.DeleteBackupServerAsync(serverId);
                return Json(null);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("ListVmmServers")]
        public async Task<JsonResult> ListVmmServers()
        {
            try
            {
                var serverList = await ClientFactory.VmBackupClient.ListVmmServers();
                List<VmmServerModel> servers = serverList.Select(s => new VmmServerModel(s)).ToList();
                return this.JsonDataSet(servers);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("CreateVmmServer")]
        public async Task<JsonResult> CreateVmmServer(Guid stampId, string serverName, string userName, string password)
        {
            Requires.Argument<Guid>("stampId", stampId).NotNull();
            Requires.Argument<string>("serverName", serverName).NotNullOrEmpty();
            Requires.Argument<string>("userName", userName).NotNullOrEmpty();
            Requires.Argument<string>("password", password).NotNullOrEmpty();

            try
            {
                VmmServer serverToPost = new VmmServer()
                {
                    StampId = stampId,
                    ServerName = serverName,
                    UserName = userName,
                    Password = password
                };
                var server = await ClientFactory.VmBackupClient.CreateVmmServerAsync(serverToPost);
                VmmServerModel serverModel = new VmmServerModel(server);

                return Json(serverModel);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("UpdateVmmServer")]
        public async Task<JsonResult> UpdateVmmServer(Guid stampId, string serverName, string userName, string password)
        {
            Requires.Argument<Guid>("stampId", stampId).NotNull();
            Requires.Argument<string>("serverName", serverName).NotNullOrEmpty();
            Requires.Argument<string>("userName", userName).NotNullOrEmpty();
            Requires.Argument<string>("password", password).NotNullOrEmpty();

            try
            {
                VmmServer serverToPost = new VmmServer()
                {
                    StampId = stampId,
                    ServerName = serverName,
                    UserName = userName,
                    Password = password
                };
                var server = await ClientFactory.VmBackupClient.UpdateVmmServerAsync(serverToPost);
                VmmServerModel serverModel = new VmmServerModel(server);

                return Json(serverModel);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        [ActionName("SetSpfServer")]
        public async Task<JsonResult> SetSpfServer(string adminUrl, string userName, string password)
        {
            Requires.Argument<string>("adminUrl", adminUrl).NotNullOrEmpty();
            Requires.Argument<string>("userName", userName).NotNullOrEmpty();
            Requires.Argument<string>("password", password).NotNullOrEmpty();

            try
            {
                SpfServer serverToPost = new SpfServer()
                {
                    AdminUrl = adminUrl,
                    UserName = userName,
                    Password = password
                };
                var server = await ClientFactory.VmBackupClient.SetSpfServerAsync(serverToPost);
                SpfServerModel serverModel = new SpfServerModel(server);

                return Json(serverModel);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        #region OLD
        /// <summary>
        /// Gets the admin settings.
        /// </summary>
        [HttpPost]
        [ActionName("AdminSettings")]
        public async Task<JsonResult> GetAdminSettings()
        {
            try
            {
                var resourceProvider = await ClientFactory.AdminManagementClient.GetResourceProviderAsync
                                                            (VmBackupClient.RegisteredServiceName, Guid.Empty.ToString());

                this.VmBackupServiceEndPoint = EndpointModel.FromResourceProviderEndpoint(resourceProvider.AdminEndpoint);
                return this.JsonDataSet(this.VmBackupServiceEndPoint);
            }
            catch (ManagementClientException managementException)
            {
                // 404 means the Hello World resource provider is not yet configured, return an empty record.
                if (managementException.StatusCode == HttpStatusCode.NotFound)
                {
                    return this.JsonDataSet(new EndpointModel());
                }

                //Just throw if there is any other type of exception is encountered
                throw;
            }
        }

        /// <summary>
        /// Update admin settings => Register Resource Provider
        /// </summary>
        /// <param name="newSettings">The new settings.</param>
        [HttpPost]
        [ActionName("UpdateAdminSettings")]
        public async Task<JsonResult> UpdateAdminSettings(EndpointModel newSettings)
        {
            this.ValidateInput(newSettings);

            ResourceProvider vmBackupResourceProvider;
            string errorMessage = string.Empty;

            try
            {
                //Check if resource provider is already registered or not
                vmBackupResourceProvider = await ClientFactory.AdminManagementClient.GetResourceProviderAsync(VmBackupClient.RegisteredServiceName, Guid.Empty.ToString());
            }
            catch (ManagementClientException exception)
            {
                // 404 means the Hello World resource provider is not yet configured, return an empty record.
                if (exception.StatusCode == HttpStatusCode.NotFound)
                {
                    vmBackupResourceProvider = null;
                }
                else
                {
                    //Just throw if there is any other type of exception is encountered
                    throw;
                }
            }

            if (vmBackupResourceProvider != null)
            {
                //Resource provider already registered so lets update endpoint
                vmBackupResourceProvider.AdminEndpoint = newSettings.ToAdminEndpoint();
                vmBackupResourceProvider.TenantEndpoint = newSettings.ToTenantEndpoint();
                vmBackupResourceProvider.NotificationEndpoint = newSettings.ToNotificationEndpoint();
                vmBackupResourceProvider.UsageEndpoint = newSettings.ToUsageEndpoint();
            }
            else
            {
                //Resource provider not registered yet so lets register new one now
                vmBackupResourceProvider = new ResourceProvider()
                {
                    Name = VmBackupClient.RegisteredServiceName,
                    DisplayName = "Hello World",
                    InstanceDisplayName = VmBackupClient.RegisteredServiceName + " Instance",
                    Enabled = true,
                    PassThroughEnabled = true,
                    AllowAnonymousAccess = false,
                    AdminEndpoint = newSettings.ToAdminEndpoint(),
                    TenantEndpoint = newSettings.ToTenantEndpoint(),
                    NotificationEndpoint = newSettings.ToNotificationEndpoint(),
                    UsageEndpoint = newSettings.ToUsageEndpoint(),
                    MaxQuotaUpdateBatchSize = 3 // Check link http://technet.microsoft.com/en-us/library/dn520926(v=sc.20).aspx
                };
            }

            var testList = new ResourceProviderVerificationTestList()
                               {
                                   new ResourceProviderVerificationTest()
                                   {
                                       TestUri = new Uri(VmBackupAdminController.adminAPIUri + VmBackupClient.AdminSettings),
                                       IsAdmin = true
                                   }
                               };
            try
            {
                // Resource Provider Verification to ensure given endpoint and username/password is correct
                // Only validate the admin RP since we don't have a tenant subscription to do it.
                var result = await ClientFactory.AdminManagementClient.VerifyResourceProviderAsync(vmBackupResourceProvider, testList);
                if (result.HasFailures)
                {
                    throw new HttpException("Invalid endpoint or bad username/password");
                }
            }
            catch (ManagementClientException ex)
            {
                throw new HttpException("Invalid endpoint or bad username/password " + ex.Message.ToString());
            }

            //Finally Create Or Update resource provider
            Task<ResourceProvider> rpTask = (string.IsNullOrEmpty(vmBackupResourceProvider.Name) || String.IsNullOrEmpty(vmBackupResourceProvider.InstanceId))
                                                ? ClientFactory.AdminManagementClient.CreateResourceProviderAsync(vmBackupResourceProvider)
                                                : ClientFactory.AdminManagementClient.UpdateResourceProviderAsync(vmBackupResourceProvider.Name, vmBackupResourceProvider.InstanceId, vmBackupResourceProvider);

            try
            {
                await rpTask;
            }
            catch (ManagementClientException e)
            {
                throw e;
            }

            return this.Json(newSettings);
        }

        /// <summary>
        /// Gets all File Servers.
        /// </summary>
        [HttpPost]
        [ActionName("FileServers")]
        public async Task<JsonResult> GetAllFileServers()
        {
            try
            {
                var fileServers = await ClientFactory.VmBackupClient.GetFileServerListAsync();
                var fileServerModel = fileServers.Select(d => new FileServerModel(d)).ToList();
                return this.JsonDataSet(fileServerModel);
            }
            catch (HttpRequestException)
            {
                // Returns an empty collection if the HTTP request to the API fails
                return this.JsonDataSet(new FileServerList());
            }
        }

        /// <summary>
        /// Gets all Products.
        /// </summary>
        [HttpPost]
        [ActionName("Products")]
        public async Task<JsonResult> GetAllProducts()
        {
            try
            {
                var productNames = await ClientFactory.VmBackupClient.GetProductListAsync();
                var productModels = productNames.Select(d => new ProductModel(d)).ToList();
                return this.JsonDataSet(productModels);
            }
            catch (HttpRequestException)
            {
                // Returns an empty collection if the HTTP request to the API fails 
                return this.JsonDataSet(new ProductList());
            }
        }
        #endregion

        private PortalException HandleAggregateException(System.AggregateException aggregateException)
        {
            ResourceProviderClientException exception = aggregateException.InnerException as ResourceProviderClientException;

            if (exception != null)
            {
                return new PortalException(exception.Message, exception.HttpStatusCode);
            }

            return new PortalException(aggregateException.InnerException.Message, aggregateException.InnerException, System.Net.HttpStatusCode.InternalServerError);
        }

        private PortalException HandleException(Exception exception)
        {
            var e = exception as ResourceProviderClientException;

            if (e != null)
            {
                return new PortalException(exception.Message, e.HttpStatusCode);
            }

            return new PortalException(exception.Message, exception, System.Net.HttpStatusCode.InternalServerError);
        }

        private void ValidateInput(EndpointModel newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException("newSettings");
            }

            if (String.IsNullOrEmpty(newSettings.EndpointAddress))
            {
                throw new ArgumentNullException("EndpointAddress");
            }

            if (String.IsNullOrEmpty(newSettings.Username))
            {
                throw new ArgumentNullException("Username");
            }

            if (String.IsNullOrEmpty(newSettings.Password))
            {
                throw new ArgumentNullException("Password");
            }
        }
    }
}