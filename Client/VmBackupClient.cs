// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzure.Management.Common;
using SPF=Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.SpfTenant;
using System.Net.Http.Formatting;
using System.ServiceModel.Syndication;
using Microsoft.WindowsAzure.Management;
using System.Net;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient
{
    /// <summary>
    /// This is client of Hello World Resource Provider 
    /// This client is used by admin and tenant extensions to make call to Hello World Resource Provider
    /// In real world you should have seperate clients of admin and tenant extensions    
    /// </summary>
    public class VmBackupClient
    {        
        public const string RegisteredServiceName = "vmbackup";
        public const string RegisteredPath = "services/" + RegisteredServiceName;
        public const string AdminSettings = RegisteredPath + "/settings";
        public const string AdminProducts = RegisteredPath + "/products";
        public const string AdminFileServers = RegisteredPath + "/fileservers";

        public const string ServerGroups = RegisteredPath + "/servergroups";
        public const string ServerGroup = RegisteredPath + "/servergroups/{0}";

        public const string BackupServers = RegisteredPath + "/backupservers";
        public const string BackupServer = RegisteredPath + "/backupservers/{0}";

        public const string VmmServers = RegisteredPath + "/vmmservers";
        public const string VmmServer = RegisteredPath + "/vmmservers/{0}";

        public const string SpfServer = RegisteredPath + "/spfserver";

        public const string TenantVirtualMachines = "{0}/" + RegisteredPath + "/virtualmachines";
        public const string TenantVirtualMachine = "{0}/" + RegisteredPath + "/virtualmachines/{1}";
        public const string RestoreTenantVirtualMachine = "{0}/" + RegisteredPath + "/virtualmachines/{1}/restore/{2}";

        public const string FileShares = "{0}/" + RegisteredPath + "/fileshares";

        public const string SpfVMs = "{0}/services/systemcenter/VMM/";

        public Uri BaseEndpoint { get; set; }
        public HttpClient httpClient;
        public RestClient restClient;

        /// <summary>
        /// This constructor takes BearerMessageProcessingHandler which reads token as attach to each request
        /// </summary>
        /// <param name="baseEndpoint"></param>
        /// <param name="handler"></param>
        public VmBackupClient(Uri baseEndpoint, MessageProcessingHandler handler)
        {
            if (baseEndpoint == null) 
            {
                throw new ArgumentNullException("baseEndpoint"); 
            }

            this.BaseEndpoint = baseEndpoint;

          
            this.httpClient = new HttpClient(handler);
            this.httpClient.Timeout = TimeSpan.FromSeconds(500);
            this.restClient = new RestClient(handler: handler, timeout: System.Threading.Timeout.InfiniteTimeSpan);
            this.restClient = new RestClient(handler);

            var user = System.Threading.Thread.CurrentPrincipal;
            this.httpClient.DefaultRequestHeaders.Add("x-ms-principal-id", user.Identity.Name);

        }

        public VmBackupClient(Uri baseEndpoint, string bearerToken, TimeSpan? timeout = null)
        {
            if (baseEndpoint == null) 
            { 
                throw new ArgumentNullException("baseEndpoint"); 
            }

            this.BaseEndpoint = baseEndpoint;

            this.httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            this.restClient = new RestClient();
            if (timeout.HasValue)
            {
                this.httpClient.Timeout = timeout.Value;
            }

            var user = System.Threading.Thread.CurrentPrincipal;
            this.httpClient.DefaultRequestHeaders.Add("x-ms-principal-id", user.Identity.Name);
        }
       
        #region Admin APIs

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceToPost"></param>
        /// <returns></returns>
        public Task<ServerGroup> CreateServerGroupAsync(ServerGroup resourceToPost)
        {
            if (resourceToPost == null)
            {
                throw new ArgumentNullException("resourceToPost");
            }

            var requestUrl = this.CreateRequestUri(VmBackupClient.ServerGroups);
            return this.restClient.PostResourceAsync<ServerGroup, VmBackupErrorResource>(resourceToPost, requestUrl)
                .HandleException<ServerGroup>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<ServerGroupList> ListServerGroups()
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.ServerGroups);
            return this.restClient.GetResourceAsync<ServerGroupList, VmBackupErrorResource>(requestUrl)
                .HandleException<ServerGroupList>(this.HandleAggregateException);
        }

        public async Task DeleteServerGroupAsync(Guid groupId)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException("groupId");
            }

            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.ServerGroup, groupId));
            await this.restClient.DeleteResourceAsync<VmBackupErrorResource>(requestUrl)
                .HandleException<bool>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceToPost"></param>
        /// <returns></returns>
        public Task<BackupServer> CreateBackupServerAsync(BackupServer resourceToPost)
        {
            if (resourceToPost == null)
            {
                throw new ArgumentNullException("resourceToPost");
            }

            var requestUrl = this.CreateRequestUri(VmBackupClient.BackupServers);
            return this.restClient.PostResourceAsync<BackupServer, VmBackupErrorResource>(resourceToPost, requestUrl)
                .HandleException<BackupServer>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceToPost"></param>
        /// <returns></returns>
        public Task<BackupServer> UpdateBackupServerAsync(BackupServer resourceToPost)
        {
            if (resourceToPost == null)
            {
                throw new ArgumentNullException("resourceToPost");
            }

            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.BackupServer, resourceToPost.ServerId));
            return this.restClient.PutResourceAsync<BackupServer, VmBackupErrorResource>(resourceToPost, requestUrl)
                .HandleException<BackupServer>(this.HandleAggregateException);
        }

        public async Task DeleteBackupServerAsync(Guid serverId)
        {
            if (serverId == null)
            {
                throw new ArgumentNullException("serverId");
            }

            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.BackupServer, serverId));
            await this.restClient.DeleteResourceAsync<VmBackupErrorResource>(requestUrl)
                .HandleException<bool>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<BackupServerList> ListBackupServers()
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.BackupServers);
            return this.restClient.GetResourceAsync<BackupServerList, VmBackupErrorResource>(requestUrl)
                .HandleException<BackupServerList>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<VmmServerList> ListVmmServers()
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.VmmServers);
            return this.restClient.GetResourceAsync<VmmServerList, VmBackupErrorResource>(requestUrl)
                .HandleException<VmmServerList>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceToPost"></param>
        /// <returns></returns>
        public Task<VmmServer> CreateVmmServerAsync(VmmServer resourceToPost)
        {
            if (resourceToPost == null)
            {
                throw new ArgumentNullException("resourceToPost");
            }

            var requestUrl = this.CreateRequestUri(VmBackupClient.VmmServers);
            return this.restClient.PostResourceAsync<VmmServer, VmBackupErrorResource>(resourceToPost, requestUrl)
                .HandleException<VmmServer>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceToPost"></param>
        /// <returns></returns>
        public Task<VmmServer> UpdateVmmServerAsync(VmmServer resourceToPost)
        {
            if (resourceToPost == null)
            {
                throw new ArgumentNullException("resourceToPost");
            }

            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.VmmServer, resourceToPost.StampId));
            return this.restClient.PutResourceAsync<VmmServer, VmBackupErrorResource>(resourceToPost, requestUrl)
                .HandleException<VmmServer>(this.HandleAggregateException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceToPost"></param>
        /// <returns></returns>
        public Task<SpfServer> SetSpfServerAsync(SpfServer resourceToPost)
        {
            if (resourceToPost == null)
            {
                throw new ArgumentNullException("resourceToPost");
            }

            var requestUrl = this.CreateRequestUri(VmBackupClient.SpfServer);
            return this.restClient.PostResourceAsync<SpfServer, VmBackupErrorResource>(resourceToPost, requestUrl)
                .HandleException<SpfServer>(this.HandleAggregateException);
        }

        /// <summary>
        /// GetAdminSettings returns Hello World Resource Provider endpoint information if its registered with Admin API
        /// </summary>
        /// <returns></returns>
        public async Task<AdminSettings> GetAdminSettingsAsync()
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.AdminSettings);

            // For simplicity, we make a request synchronously.
            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AdminSettings>();
        }

        /// <summary>
        /// UpdateAdminSettings registers Hello World Resource Provider endpoint information with Admin API
        /// </summary>
        /// <returns></returns>
        public async Task UpdateAdminSettingsAsync(AdminSettings newSettings)
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.AdminSettings);
            var response = await this.httpClient.PutAsJsonAsync<AdminSettings>(requestUrl.ToString(), newSettings);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// GetFileServerList return list of file servers hosted in Hello World Resource Provider
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileServer>> GetFileServerListAsync()
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.AdminFileServers));

            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<FileServer>>();
        }

        /// <summary>
        /// UpdateFileServer updates existing file server information in Hello World Resource Provider
        /// </summary>        
        public async Task UpdateFileServerAsync(FileServer fileServer)
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.AdminFileServers);
            var response = await this.httpClient.PutAsJsonAsync<FileServer>(requestUrl.ToString(), fileServer);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// AddFileServer adds new file server in Hello World Resource Provider
        /// </summary>        
        public async Task AddFileServerAsync(FileServer fileServer)
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.AdminFileServers);
            var response = await this.httpClient.PutAsJsonAsync<FileServer>(requestUrl.ToString(), fileServer);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// GetProductList return list of products stored in Hello World Resource Provider
        /// </summary>        
        public async Task<List<Product>> GetProductListAsync()
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.AdminProducts));

            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<List<Product>>();
        }

        /// <summary>
        /// UpdateProduct updates existing product information in Hello World Resource Provider
        /// </summary>        
        public async Task UpdateProductAsync(Product product)
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.AdminProducts);
            var response = await this.httpClient.PutAsJsonAsync<Product>(requestUrl.ToString(), product);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// AddProduct adds new product in Hello World Resource Provider
        /// </summary>        
        public async Task AddProductAsync(Product product)
        {
            var requestUrl = this.CreateRequestUri(VmBackupClient.AdminProducts);
            var response = await this.httpClient.PostAsXmlAsync<Product>(requestUrl.ToString(), product);
            response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Tenant APIs

        public async Task<List<VirtualMachine>> ListProtectedVirtualMachines(string subscriptionId, bool includeDetails = false)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.TenantVirtualMachines, subscriptionId));

            UriBuilder builder = new UriBuilder(requestUrl);
            if (!includeDetails)
            {
                builder.Query += "IncludeDetails=false";
            }
            List<VirtualMachine> vms = new List<VirtualMachine>();

            VirtualMachineList protectedVms = await this.restClient.GetResourceAsync<VirtualMachineList, VmBackupErrorResource>(builder.Uri);
            vms.AddRange(protectedVms);

            return vms;
        }

        public async Task<List<VirtualMachine>> ListVirtualMachnes(string subscriptionId)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.TenantVirtualMachines, subscriptionId));
            List<VirtualMachine> vms = new List<VirtualMachine>();
            List<VirtualMachine> spfVms = new List<VirtualMachine>();
            try
            {
                VirtualMachineList protectedVms = await this.restClient.GetResourceAsync<VirtualMachineList, VmBackupErrorResource>(requestUrl);
                vms.AddRange(protectedVms);

                requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.SpfVMs, subscriptionId));
                SPF.VMM spfInstance = new SPF.VMM(requestUrl);
                spfInstance.SendingRequest += spfInstance_SendingRequest;
                spfInstance.IgnoreMissingProperties = true;
                spfInstance.Credentials = System.Net.CredentialCache.DefaultCredentials;

                spfVms.AddRange(spfInstance.VirtualMachines.Select(vm => new VirtualMachine(vm)));
                 
                foreach (VirtualMachine spfVm in spfVms)
                {
                    var foundVm =
                        (from vm in protectedVms
                        where vm.VmId == spfVm.VmId
                        select vm).FirstOrDefault();
                    if (foundVm == null && spfVm.VmId != Guid.Empty)
                    {
                                vms.Add(spfVm);
                    }
                    else
                    {
                        foundVm.VmStatus = spfVm.VmStatus;
                        foundVm.VmStatusString = spfVm.VmStatusString;
                    }
                }

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return vms;
        }

        public async Task<VirtualMachine> AddVirtualMachineToProtectionGroup(string subscriptionId, VirtualMachine vm)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.TenantVirtualMachines, subscriptionId));
            return await this.restClient.PostResourceAsync<VirtualMachine, VmBackupErrorResource>(vm, requestUrl)
                .HandleException<VirtualMachine>(this.HandleAggregateException);
        }

        public async Task RemoveVirtualMachineFromProtectionGroup(string subscriptionId, string vmId)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.TenantVirtualMachine, subscriptionId, vmId));
            await this.restClient.DeleteResourceAsync<VmBackupErrorResource>(requestUrl)
                .HandleException<bool>(this.HandleAggregateException);
        }

        public async Task BackupVirtualMachine(string subscriptionId, string vmId, VirtualMachine vm)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.TenantVirtualMachine, subscriptionId, vmId));
            await this.restClient.PutResourceAsync<VirtualMachine, VmBackupErrorResource>(vm, requestUrl)
                .HandleException<VirtualMachine>(this.HandleAggregateException);
        }

        public async Task<List<RecoveryPoint>> ListRecoveryPoints(string subscriptionId, string vmId)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.TenantVirtualMachine, subscriptionId, vmId));
            RecoveryPointList recoveryPoints = await this.restClient.GetResourceAsync<RecoveryPointList, VmBackupErrorResource>(requestUrl)
                .HandleException<RecoveryPointList>(this.HandleAggregateException);

            return recoveryPoints.ToList();
        }

        public async Task RestoreVirtualMachine(string subscriptionId, string vmId, string recoverySourceId, VirtualMachine vm)
        {
            var requestUrl = this.CreateRequestUri(string.Format(CultureInfo.InvariantCulture, VmBackupClient.RestoreTenantVirtualMachine, subscriptionId, vmId, recoverySourceId));
            await this.restClient.PutResourceAsync<VirtualMachine, VmBackupErrorResource>(vm, requestUrl)
                .HandleException<VirtualMachine>(this.HandleAggregateException);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Common method for making GET calls
        /// </summary>        
        private async Task<T> GetAsync<T>(Uri requestUrl)
        {         
            var response = await this.httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        /// <summary>
        /// Common method for making POST calls
        /// </summary>        
        private async Task PostAsync<T>(Uri requestUrl, T content)
        {            
            var response = await this.httpClient.PostAsXmlAsync<T>(requestUrl.ToString(), content);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Common method for making PUT calls
        /// </summary>        
        private async Task PutAsync<T>(Uri requestUrl, T content)
        {
            System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(500));
            var response = await this.httpClient.PutAsJsonAsync<T>(requestUrl.ToString(), content, source.Token);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Common method for making Request Uri's
        /// </summary>        
        private Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(this.BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

        private static string CreateUri(string subscriptionId)
        {
            return string.Format(CultureInfo.InvariantCulture, VmBackupClient.FileShares, subscriptionId);
        }

        private ResourceProviderClientException HandleAggregateException(AggregateException aex)
        {
            RestClientException<VmBackupErrorResource> restClientException = aex.InnerException as RestClientException<VmBackupErrorResource>;
            HttpRequestException httpRequestException = aex.InnerException as HttpRequestException;

            if (httpRequestException != null)
            {
                WebException webEx = httpRequestException.InnerException as WebException;
                if (webEx != null)
                {
                    return new ResourceProviderClientException(webEx.Message, webEx) { HttpStatusCode = HttpStatusCode.BadRequest };
                }

                return new ResourceProviderClientException(httpRequestException.Message, httpRequestException) { HttpStatusCode = HttpStatusCode.BadRequest };
            }

            if (restClientException != null)
            {
                VmBackupErrorResource error = restClientException.MessageContent;

                if (error != null)
                {
                    return new ResourceProviderClientException(error.Message) { HttpStatusCode = restClientException.StatusCode, ErrorCode = error.Code, State = error.State, Severity = error.Severity };
                }
                else
                {
                    return new ResourceProviderClientException("ServerResources.InternalError") { HttpStatusCode = HttpStatusCode.InternalServerError, ErrorCode = "Constants.UnknownErrorCode" };
                }
            }

            return new ResourceProviderClientException(aex.InnerException.Message, aex.InnerException) { HttpStatusCode = HttpStatusCode.InternalServerError };
        }

        private void spfInstance_SendingRequest(object sender, System.Data.Services.Client.SendingRequestEventArgs e)
        {
            // Set Claim-based auth header
            var bearerToken = AuthenticationHelper.GetPrincipalBearerToken();

            // TODO: We should fail here when I remove the basic auth fallback
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                e.RequestHeaders.Add("Authorization", "Bearer " + bearerToken);
            }

            e.RequestHeaders.Add("Accept-Language", System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

            // Set common headers, including x-ms-principal-id
            RequestCorrelationContext current = RequestCorrelationContext.Current;
            current.Transfer(e.RequestHeaders);
        }

        #endregion
    }
}
