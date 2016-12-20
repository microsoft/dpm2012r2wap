using System;
using System.Collections.Generic;
using System.Data.Services.Client;
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
using SPFAdmin = Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.SpfAdmin;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Providers
{
    public class VmmServersProvider
    {

        private const string spfAdminUri = "SC2012R2/Admin/Microsoft.Management.Odata.svc/";

        private ConfigStorageClient configStorageClient;
        private VmmServerClient vmmServerClient;
        private SymmetricEncryptor encryptor;

        public VmmServersProvider(ConfigStorageClient configStorageClient, VmmServerClient vmmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.vmmServerClient = vmmServerClient;
            this.encryptor = new SymmetricEncryptor(EncryptionConstants.EncryptionKey, EncryptionConstants.EncryptionAlgorithm);
        }

        public async Task<VmmServerList> ListVmmServers()
        {
            try
            {
                VmmServerList servers = await this.configStorageClient.SelectVmmServerCollectionAsync<VmmServerList>(VmmServerMapping.CreateVmmServerCollectonWithoutPassword);
                SpfServer server = await this.configStorageClient.SelectSpfServerAsync<SpfServer>(SpfServerMapping.CreateSpfServer);
                if (server != null)
                {
                    var adminContext = GetAdminContext(server);
                    foreach (SPFAdmin.Stamp stamp in adminContext.Stamps)
                    {
                        var foundServer = (from s in servers where s.StampId == stamp.ID select s).FirstOrDefault();
                        if (foundServer == null)
                        {
                            servers.Add(new VmmServer() { StampId = stamp.ID, ServerName = stamp.Name, State = "NotRegistered" });
                        }
                    }
                }
                return servers;
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("ListVmmServers", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }
        }

        public async Task<VmmServer> InsertVmmServer(VmmServer server)
        {
            try
            {
                await this.vmmServerClient.CheckVmmConnectivity(server);

                byte[] encryptionResult = encryptor.EncryptData(server.Password);
                return await this.configStorageClient.InsertVmmServerAsync<VmmServer>(
                    server.StampId,
                    server.ServerName,
                    server.UserName,
                    encryptionResult,
                    VmmServerMapping.CreateVmmServerWithoutPassword);
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("InsertVmmServer", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }
        }

        public async Task<VmmServer> UpdateVmmServer(VmmServer server, string id)
        {
            try
            {
                Guid stampId = Guid.Parse(id);
                if (stampId != server.StampId)
                {
                    throw new Exception("Bad ID!");
                }

                await this.vmmServerClient.CheckVmmConnectivity(server);

                byte[] encryptionResult = encryptor.EncryptData(server.Password);
                return await this.configStorageClient.UpdateVmmServerAsync<VmmServer>(
                    server.StampId,
                    server.UserName,
                    encryptionResult,
                    VmmServerMapping.CreateVmmServerWithoutPassword);
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("UpdateVmmServer", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }
        }

        public async Task<SpfServer> SetSpfServer(SpfServer server)
        {
            try
            {
                SpfAdmin.Admin context = GetAdminContext(server);
                if (context.Stamps.Count() > 0)
                {
                    byte[] encryptionResult = encryptor.EncryptData(server.Password);
                    return await this.configStorageClient.InsertOrUpdateSpfServerAsync<SpfServer>(server.AdminUrl, server.UserName, encryptionResult, SpfServerMapping.CreateSpfServerWithoutPassword);
                }
                else
                {
                    throw new Exception("Could not get stamps!");
                }
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("SetSpfServer", VmBackupEventId.UnexpectedAdminException, e);
                throw;
            }            
        }

        private SPFAdmin.Admin GetAdminContext(SpfServer server)
        {
            var context = new SPFAdmin.Admin(
                       new Uri(
                           string.Format(
                               CultureInfo.InvariantCulture,
                               "{0}/{1}",
                               server.AdminUrl.TrimEnd(new[] { '/' }),
                               spfAdminUri)));

            context.IgnoreMissingProperties = true;

            context.Credentials = new NetworkCredential(server.UserName, server.Password);
            return context;
        }

    }
}