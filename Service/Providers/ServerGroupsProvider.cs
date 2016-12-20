using System;
using System.Collections.Generic;
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

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Providers
{
    public class ServerGroupsProvider
    {
        private ConfigStorageClient configStorageClient;

        public ServerGroupsProvider(ConfigStorageClient configStorageClient)
        {
            this.configStorageClient = configStorageClient;
        }

        public async Task<ServerGroup> CreateServerGroup(ServerGroup serverGroup)
        {
            Guid groupId = Guid.NewGuid();

            try
            {
                return await this.configStorageClient.InsertServerGroupAsync<ServerGroup>(groupId, serverGroup.GroupName, serverGroup.AzureBackupEnabled, ServerGroupMapping.CreateServerGroup);
            }
            catch (SqlException e)
            {
                string exceptionMessage;
                const int GroupIdAlreadyUsed = 56001;
                const int GroupNameAlreadyUsed = 56002;

                int messageNumber;
                bool parseSucceeded = int.TryParse(e.Message, out messageNumber);

                if (parseSucceeded && GroupIdAlreadyUsed == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateServerGroupFailed, serverGroup.GroupName);
                    VmBackupLog.Current.WriteErrorMessage("CreateServerGroup", VmBackupEventId.ServerGroupIdAlreadyInUse, null, groupId);
                    throw Utility.CreateHttpResponseException(
                        exceptionMessage,
                        HttpStatusCode.Conflict);
                }
                else if (parseSucceeded && GroupNameAlreadyUsed == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.CreateServerGroupFailed, serverGroup.GroupName);
                    VmBackupLog.Current.WriteErrorMessage("CreateServerGroup", VmBackupEventId.ServerGroupNameAlreadyInUse, null, serverGroup.GroupName);
                    throw Utility.CreateHttpResponseException(
                        exceptionMessage,
                        HttpStatusCode.Conflict);
                }
                else
                {
                    VmBackupLog.Current.WriteErrorMessage("CreateServerGroup", VmBackupEventId.UnexpectedAdminException, e);
                    throw;
                }
            }
        }

        public async Task<ServerGroup> GetServerGroup(string id)
        {
            Guid groupId = Guid.Parse(id);
            return await this.configStorageClient.SelectServerGroupByIdAsync<ServerGroup>(groupId, ServerGroupMapping.CreateServerGroup);
        }

        public async Task<ServerGroupList> ListServerGroups()
        {
            return await this.configStorageClient.SelectServerGroupCollectionAsync<ServerGroupList>(ServerGroupMapping.CreateServerGroupCollection);
        }

        public async Task DeleteServerGroup(string id)
        {
            try
            {
                Guid groupId = Guid.Parse(id);
                await this.configStorageClient.DeleteServerGroupAsync(groupId);
            }
            catch (SqlException e)
            {
                string exceptionMessage;
                const int GroupNotFound = 56001;
                const int GroupHasServers = 56002;

                int messageNumber;
                bool parseSucceeded = int.TryParse(e.Message, out messageNumber);

                if (parseSucceeded && GroupNotFound == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.DeleteServerGroupNotFound, id);
                    VmBackupLog.Current.WriteErrorMessage("DeleteServerGroup", VmBackupEventId.DeleteServerGroupNotFound, null, id);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.BadRequest);
                }
                else if (parseSucceeded && GroupHasServers == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.ServerGroupHasServers, id);
                    VmBackupLog.Current.WriteErrorMessage("DeleteServerGroup", VmBackupEventId.ServerGroupHasServers, null, id);
                    throw Utility.CreateHttpResponseException(exceptionMessage, HttpStatusCode.Conflict);
                }
                else
                {
                    VmBackupLog.Current.WriteErrorMessage("DeleteServerGroup", VmBackupEventId.UnexpectedAdminException, e);
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