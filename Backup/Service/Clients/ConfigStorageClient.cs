using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Server.Common;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    public class ConfigStorageClient: StorageClient
    {
        public ConfigStorageClient(string connectionString)
            : base(connectionString)
        {
        }

        public Task<TResult> SelectVirtaulMachineCollectionAsync<TResult>(
            Guid subscriptionId,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectVirtualMachineCollectionProc,
                translateFunc,
                SqlCommandFactory.SelectVirtualMachineCollectionParameters(subscriptionId));
        }

        public Task<TResult> InsertVirtaulMachineRecordAsync<TResult>(
            Guid subscriptionId,
            Guid vmmId,
            Guid hyperVId,
            string vmName,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.InsertVirtualMachineRecordProc,
                translateFunc,
                SqlCommandFactory.InsertVirtualMachineRecordParameters(subscriptionId, vmmId, hyperVId, vmName));
        }

        public Task CompleteVirtualMachineRecordAsync(Guid subscriptionId, Guid hyperVId, Guid dataSourceId)
        {
            return this.ExecuteNonQueryProcedureAsync(
                SqlCommandFactory.CompleteVirtualMachineRecordProc,
                SqlCommandFactory.CompleteVirtualMachineRecordParameters(subscriptionId, hyperVId, dataSourceId));
        }

        public Task<TResult> SelectProtetionGroupsBySubscriptionAsync<TResult>(
            Guid subscriptionId,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectProtetionGroupsBySubscriptionProc,
                translateFunc,
                SqlCommandFactory.SelectProtetionGroupsBySubscriptionParameters(subscriptionId));
        }

        public Task DeleteVirtualMachineRecordAsync(Guid subscriptionId, Guid hyperVId)
        {
            return this.ExecuteNonQueryProcedureAsync(
                SqlCommandFactory.DeleteVirtualMachineRecordProc,
                SqlCommandFactory.DeleteVirtualMachineRecordParameters(subscriptionId, hyperVId));
        }

        public Task<TResult> SelectProtetionGroupByVirtualmachineAsync<TResult>(
            Guid subscriptionId,
            Guid hyperVId,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectProtectionGroupByVirtualMachineProc,
                translateFunc,
                SqlCommandFactory.SelectProtectionGroupByVirtualMachineParameters(subscriptionId, hyperVId));
        }

        public Task UpdateVirtualMachineActionStateAsync(Guid subscriptionId, Guid hyperVId, int actionStateId)
        {
            return this.ExecuteNonQueryProcedureAsync(
                SqlCommandFactory.UpdateVirtualMachineActionStateProc,
                SqlCommandFactory.UpdateirtualMachineActionStateParameters(subscriptionId, hyperVId, actionStateId));
        }

        public Task<TResult> InsertServerGroupAsync<TResult>(
            Guid groupId,
            string groupName,
            bool azureBackupEnabled,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.InsertServerGroupProc,
                translateFunc,
                SqlCommandFactory.InsertServerGroupParameters(groupId, groupName, azureBackupEnabled));
        }

        public Task<TResult> InsertBackupServerAsync<TResult>(
            Guid serverId,
            string serverName,
            string userName,
            byte[] password,
            Guid groupId,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.InsertBackupServerProc,
                translateFunc,
                SqlCommandFactory.InsertBackupServerParameters(serverId, serverName, userName, password, groupId));
        }

        public Task<TResult> SelectServerGroupByIdAsync<TResult>(
            Guid groupId,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectServerGroupByIdProc,
                translateFunc,
                SqlCommandFactory.SelectServerGroupByIdParameters(groupId));
        }

        public Task<TResult> SelectServerGroupCollectionAsync<TResult>(
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectServerGroupCollectionProc,
                translateFunc,
                SqlCommandFactory.SelectServerGroupCollectionParameters());
        }

        public Task<TResult> SelectBackupServerCollectionAsync<TResult>(
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectBackupServerCollectionProc,
                translateFunc,
                SqlCommandFactory.SelectBackupServerCollectionParameters());
        }

        public Task<TResult> InsertOrUpdateSpfServerAsync<TResult>(
            string adminUrl,
            string userName,
            byte[] password,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.InsertOrUpdateSpfServerProc,
                translateFunc,
                SqlCommandFactory.InsertOrUpdateSpfServerParameters(adminUrl, userName, password));
        }

        public Task<TResult> SelectSpfServerAsync<TResult>(
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectSpfServerProc,
                translateFunc,
                SqlCommandFactory.SelectSpfServerParameters());
        }

        public Task<TResult> SelectVmmServerCollectionAsync<TResult>(
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.SelectVmmServerCollectionProc,
                translateFunc,
                SqlCommandFactory.SelectVmmServerCollectionParameters());
        }

        public Task<TResult> InsertVmmServerAsync<TResult>(
            Guid stampId,
            string serverName,
            string userName,
            byte[] password,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.InsertVmmServerProc,
                translateFunc,
                SqlCommandFactory.InsertVmmServerParameters(stampId, serverName, userName, password));
        }

        public Task<TResult> UpdateVmmServerAsync<TResult>(
            Guid stampId,
            string userName,
            byte[] password,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.UpdateVmmServerProc,
                translateFunc,
                SqlCommandFactory.UpdateVmmServerParameters(stampId, userName, password));
        }

        public Task<TResult> UpdateBackupServerAsync<TResult>(
            Guid serverId,
            string userName,
            byte[] password,
            Func<SqlDataReader, TResult> translateFunc) where TResult : class
        {
            return this.ExecuteProcedureReaderAsync<TResult>(
                SqlCommandFactory.UpdateBackupServerProc,
                translateFunc,
                SqlCommandFactory.UpdateBackupServerParameters(serverId, userName, password));
        }

        public Task DeleteBackupServerAsync(Guid serverId)
        {
            return this.ExecuteNonQueryProcedureAsync(
                SqlCommandFactory.DeleteBackupServerProc,
                SqlCommandFactory.DeleteBackupServerParameters(serverId));
        }

        public Task DeleteServerGroupAsync(Guid groupId)
        {
            return this.ExecuteNonQueryProcedureAsync(
                SqlCommandFactory.DeleteServerGroupProc,
                SqlCommandFactory.DeleteServerGroupParameters(groupId));
        }
    }
}