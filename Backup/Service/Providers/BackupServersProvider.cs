using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.Service;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients.Mapping;
using Microsoft.WindowsAzure.Server.Common;
using System.Management.Automation;
using System.Management.Automation.Remoting;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Providers
{
    public class BackupServersProvider
    {
        private ConfigStorageClient configStorageClient;
        private DpmServerClient dpmServerClient;
        private MemoryCache cache;
        private SymmetricEncryptor encryptor;
 
        public BackupServersProvider(ConfigStorageClient configStorageClient, DpmServerClient dpmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.dpmServerClient = dpmServerClient;
            this.cache = MemoryCache.Default;
            this.encryptor = new SymmetricEncryptor(EncryptionConstants.EncryptionKey, EncryptionConstants.EncryptionAlgorithm);
        }

        public async Task<BackupServer> CreateBackupServer(BackupServer server)
        {
            try
            {
                ServerGroup group = await this.configStorageClient.SelectServerGroupByIdAsync<ServerGroup>(server.GroupId, ServerGroupMapping.CreateServerGroup);
                if (group == null)
                {
                    string exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateBackupServerFailedGroupNotFound, server.ServerName);
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.ServerGroupNotFound, null, server.GroupId);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }

                BackupServerVersion version = await this.dpmServerClient.CheckServerVersionAndConnectivity(server.ServerName, server.UserName, server.Password);
                if (version.Major < 4 || (version.Major == 4 && version.Minor < 2))
                {
                    string exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateBackupServerFailedBadVersionNumber, server.ServerName, version.VersionString);
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.DpmVersionTooLow, null, server.ServerName, version.VersionString);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }

                if (group.AzureBackupEnabled)
                {
                    await this.dpmServerClient.CheckAzureSubscription(server.ServerName, server.UserName, server.Password);
                }

                byte[] encryptionResult = encryptor.EncryptData(server.Password);
                BackupServer createdServer = await this.configStorageClient.InsertBackupServerAsync<BackupServer>(Guid.NewGuid(), server.ServerName, server.UserName, encryptionResult, server.GroupId, BackupServerMapping.CreateBackupServerWithoutPassword);
                return createdServer;
            }
            catch (PSRemotingTransportException e)
            {
                string exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CannotConnectToServer, server.ServerName);
                VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.CannnotConnectToServer, e, server.ServerName);
                throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
            }
            catch (RuntimeException e)
            {
                string exceptionMessage;
                const int AzureSubscriptionNotFound = 10001;
                const int AzureSubscriptionNotActive = 10002;

                int messageNumber;
                bool parseSucceeded = int.TryParse(e.ErrorRecord.FullyQualifiedErrorId, out messageNumber);

                if (parseSucceeded && AzureSubscriptionNotFound == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateBackupServerFailedAzureSubscriptionNotFound, server.ServerName);
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.AzureSubscriptionNotFound, null, server.GroupId, server.ServerName);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }
                else if (parseSucceeded && AzureSubscriptionNotActive == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateBackupServerFailedAzureSubscriptionNotReady, server.ServerName);
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.AzureSubscriptionNotReady, null, server.GroupId, server.ServerName);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }
                else
                {
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.UnexpectedAdminException, e);
                    throw;
                }
            }
            catch (SqlException e)
            {
                string exceptionMessage;
                const int GroupNotFound = 56001;
                const int ServerNameAlreadyUsed = 56002;

                int messageNumber;
                bool parseSucceeded = int.TryParse(e.Message, out messageNumber);

                if (parseSucceeded && GroupNotFound == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateBackupServerFailedGroupNotFound, server.ServerName);
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.ServerGroupNotFound, null, server.GroupId);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }
                else if (parseSucceeded && ServerNameAlreadyUsed == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateBackupServerFailedNameInUse, server.ServerName);
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.ServerNameAlreadyInUse, null, server.ServerName);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.Conflict);
                }
                else
                {
                    VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.UnexpectedAdminException, e);
                    throw;
                }
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("CreateBackupServer", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }
        }

        public async Task<BackupServerList> ListBackupServers()
        {
            return await this.configStorageClient.SelectBackupServerCollectionAsync<BackupServerList>(BackupServerMapping.CreateBackupServerCollectionWithoutPassword);
        }

        public async Task<BackupServer> UpdateBackupServer(BackupServer server, string id)
        {
            try
            {
                Guid serverId = Guid.Parse(id);
                if (serverId != server.ServerId)
                {
                    throw new Exception("Bad ID!");
                }

                await this.dpmServerClient.CheckServerVersionAndConnectivity(server.ServerName, server.UserName, server.Password);

                byte[] encryptionResult = encryptor.EncryptData(server.Password);
                return await this.configStorageClient.UpdateBackupServerAsync<BackupServer>(
                    server.ServerId,
                    server.UserName,
                    encryptionResult,
                    BackupServerMapping.CreateBackupServerWithoutPassword);
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("UpdateBackupServer", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }
        }

        public async Task DeleteBackupServer(string id)
        {
            try
            {
                Guid serverId = Guid.Parse(id);
                await this.configStorageClient.DeleteBackupServerAsync(serverId);
            }
            catch (SqlException e)
            {
                string exceptionMessage;
                const int ServerNotFound = 56001;
                const int ServerHasProtectionGroups = 56002;

                int messageNumber;
                bool parseSucceeded = int.TryParse(e.Message, out messageNumber);

                if (parseSucceeded && ServerNotFound == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.DeleteBackupServerNotFound, id);
                    VmBackupLog.Current.WriteErrorMessage("DeleteBackupServer", VmBackupEventId.DeleteBackupServerNotFound, null, id);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }
                else if (parseSucceeded && ServerHasProtectionGroups == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.BackupServerHasProtectionGroups, id);
                    VmBackupLog.Current.WriteErrorMessage("DeleteBackupServer", VmBackupEventId.BackupServerHasProtectionGroups, null, id);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.Conflict);
                }
                else
                {
                    VmBackupLog.Current.WriteErrorMessage("DeleteBackupServer", VmBackupEventId.UnexpectedAdminException, e);
                    throw;
                }
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("DeleteBackupServer", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }
        }
    }
}