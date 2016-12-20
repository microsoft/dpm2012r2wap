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
    public class VirtualMachinesProvider
    {
        private ConfigStorageClient configStorageClient;
        private DpmServerClient dpmServerClient;
        private MemoryCache cache;

        public VirtualMachinesProvider(ConfigStorageClient configStorageClient, DpmServerClient dpmServerClient)
        {
            this.configStorageClient = configStorageClient;
            this.dpmServerClient = dpmServerClient;
            this.cache = MemoryCache.Default;
        }

        public async Task<VirtualMachineList> ListVirtualMachines(string subscriptionId, bool includeDetails)
        {
            Guid subId = Validator.ValidateSubscriptionId(subscriptionId);

            try
            {
                VirtualMachineList vms = await this.configStorageClient.SelectVirtaulMachineCollectionAsync(subId, VirtualMachineMapping.CreateVirtualMachineList);

                if (includeDetails)
                {
                    List<VmBackupDataSourceDetails> dataSources = (List<VmBackupDataSourceDetails>)cache[subscriptionId];

                    if (dataSources == null)
                    {
                        dataSources = new List<VmBackupDataSourceDetails>();
                        List<VmBackupTarget> targets = await this.configStorageClient.SelectProtetionGroupsBySubscriptionAsync(subId, VirtualMachineMapping.CreateVmBackupTargetList);

                        foreach (VmBackupTarget target in targets)
                        {
                            dataSources.AddRange(await this.dpmServerClient.GetLatestDataSourceData(target.BackupServerName, target.ProtectionGroupName, target.UserName, target.Password));
                        }
                        cache.Add(subscriptionId, dataSources, DateTime.Now.AddMinutes(5));
                    }
                    foreach (VmBackupDataSourceDetails dataSource in dataSources)
                    {
                        var foundVm =
                            (from vm in vms
                             where vm.VmId == dataSource.HyperVId
                             select vm).FirstOrDefault();
                        if (foundVm != null)
                        {
                            foundVm.BackupPolicy = dataSource.Schedule;
                            foundVm.LastRecoveryPoint = dataSource.LatestRecorveryPoint;
                            foundVm.RecoveryPoints = dataSource.TotalRecoveryPoints;
                        }
                    }
                }
                return vms;
            }
            catch (SqlException e)
            {
                VmBackupLog.Current.WriteErrorMessage("ListVirtualMachines", VmBackupEventId.UnexpectedTenantException, e, subId);
                throw Utility.CreateHttpResponseException(ErrorMessages.ListVirtualMachinesFailed, HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("ListVirtualMachines", VmBackupEventId.UnexpectedTenantException, e, subId);
                throw Utility.CreateHttpResponseException(ErrorMessages.ListVirtualMachinesFailed, HttpStatusCode.BadRequest);
            }
        }

        public async Task<VirtualMachine> AddVmToProtectionGroup(VirtualMachine vm)
        {
            try
            {
                Guid subId = Validator.ValidateSubscriptionId(vm.SubscriptionId);

                //Create the database record and get the backup target
                VmBackupTarget target = await this.configStorageClient.InsertVirtaulMachineRecordAsync<VmBackupTarget>(subId, vm.Id, vm.VmId, vm.Name, VirtualMachineMapping.CreateVmBackupTarget);

                //Fire and forget as this could be a long running process - For some reason just calling an async function did not work so using an actual thread.
                var t = new System.Threading.Thread(() => this.AddVirtualMachineToProtectionGroupAsync(subId, vm, target));
                t.Start();

                vm.Status = "NotProtected";
                vm.ActionStatus = VirtualMachineActionState.Protecting.ToString();
                return vm;
            }
            catch (SqlException e)
            {
                string exceptionMessage;
                const int NoProtectionGroupFound = 56001;
                const int VirtualMachineAlreadyBeingAdded = 56002;

                int messageNumber;
                bool parseSucceeded = int.TryParse(e.Message, out messageNumber);

                if (parseSucceeded && VirtualMachineAlreadyBeingAdded == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.AnotherVirtualMachineAlreadyBeingAdded, vm.Name);
                    VmBackupLog.Current.WriteErrorMessage("AddVmToProtectionGroup", VmBackupEventId.TooManyProtectionGroupAddRequests, null, vm.SubscriptionId, vm.Name);
                    throw Utility.CreateHttpResponseException(
                        exceptionMessage,
                        HttpStatusCode.Conflict);
                }
                else if (parseSucceeded && NoProtectionGroupFound == messageNumber)
                {
                    exceptionMessage = string.Format(CultureInfo.CurrentCulture, ErrorMessages.NoProtectionGroupFound, vm.Name);
                    VmBackupLog.Current.WriteErrorMessage("AddVmToProtectionGroup", VmBackupEventId.ProtectionGroupNotFound, null, vm.SubscriptionId);
                    throw Utility.CreateHttpResponseException(
                        exceptionMessage,
                        HttpStatusCode.BadRequest);
                }
                else
                {
                    VmBackupLog.Current.WriteErrorMessage("AddVmToProtectionGroup", VmBackupEventId.UnexpectedTenantException, e, vm.SubscriptionId);
                    throw;
                }

            }
        }

        public async Task RemoveVirtualMachineProtection(string subscriptionId, string id)
        {
            try
            {
                Guid subId = Validator.ValidateSubscriptionId(subscriptionId);
                Guid vmId = Guid.Parse(id);

                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subId, vmId, (int)VirtualMachineActionState.RemovingProtection);

                //Fire and forget as this could be a long running process - For some reason just calling an async function did not work so using an actual thread.
                var t = new System.Threading.Thread(() => this.RemoveVirtualMachineProtectionAsync(subId, vmId, id));
                t.Start();

            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("RemoveVirtualMachineProtection", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                throw Utility.CreateHttpResponseException(ErrorMessages.DeleteVirtualMachineFailed, HttpStatusCode.BadRequest);
            }

        }

        public async Task<VirtualMachine> BackupVirtualMachine(VirtualMachine virtualMachineToBackup, string subscriptionId, string id)
        {
            try
            {
                Guid subId = Validator.ValidateSubscriptionId(subscriptionId);
                Guid vmId = Guid.Parse(id);

                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subId, vmId, (int)VirtualMachineActionState.BackingUp);

                //Fire and forget as this could be a long running process - For some reason just calling an async function did not work so using an actual thread.
                var t = new System.Threading.Thread(() => this.BackupVirtualMachineAsync(subId, virtualMachineToBackup, id));
                t.Start();

                virtualMachineToBackup.ActionStatus = VirtualMachineActionState.BackingUp.ToString();
                return virtualMachineToBackup;
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("BackupVirtualMachine", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.BackupVirtualMachineFailed, virtualMachineToBackup.Name);
                throw Utility.CreateHttpResponseException(message, HttpStatusCode.BadRequest);
            }

        }

        public async Task<RecoveryPointList> ListVirtualMachineRestorePoints(string subscriptionId, string id)
        {
            try
            {
                Guid subId = Validator.ValidateSubscriptionId(subscriptionId);
                Guid vmId = Guid.Parse(id);

                VmBackupTarget target = await this.configStorageClient.SelectProtetionGroupByVirtualmachineAsync<VmBackupTarget>(subId, vmId, VirtualMachineMapping.CreateVmBackupTarget);
                RecoveryPointList recoveryPoints = await this.dpmServerClient.ListRecoveryPoints(target.BackupServerName, target.ProtectionGroupName, id, target.UserName, target.Password);

                return recoveryPoints;
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("ListVirtualMachineRestorePoints", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                string message = ErrorMessages.ListVirtualMachineRestorePointsFailed;
                throw Utility.CreateHttpResponseException(message, HttpStatusCode.BadRequest);
            }
        }

        public async Task RestoreVirtualMachine(VirtualMachine virtualMachineToRestore, string subscriptionId, string id, string recoverySourceId)
        {
            try
            {
                Guid subId = Validator.ValidateSubscriptionId(subscriptionId);
                Guid vmId = Guid.Parse(id);

                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subId, vmId, (int)VirtualMachineActionState.Restoring);

                //Fire and forget as this could be a long running process - For some reason just calling an async function did not work so using an actual thread.
                var t = new System.Threading.Thread(() => this.RestoreVirtualMachineAsync(subId, vmId, id, recoverySourceId));
                t.Start();

            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("RestoreVirtualMachine", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                throw Utility.CreateHttpResponseException(ErrorMessages.RestoreVirtualMachineFailed, HttpStatusCode.BadRequest);
            }
        }

        private async void AddVirtualMachineToProtectionGroupAsync(Guid subscriptionId, VirtualMachine vm, VmBackupTarget target)
        {
            bool protectionError = false;

            try
            {
                //Protect the VM and get back the data soruce ID
                Guid id = await this.dpmServerClient.AddVirtualMachineToProtectionGroup(target.BackupServerName, target.ProtectionGroupName, vm.VmId.ToString(), target.UserName, target.Password);

                //Complete the record
                await this.configStorageClient.CompleteVirtualMachineRecordAsync(subscriptionId, vm.VmId, id);
                cache.Remove(subscriptionId.ToString());
            }
            catch (System.Management.Automation.RuntimeException e)
            {
                switch (e.ErrorRecord.FullyQualifiedErrorId)
                {
                    case "10005":
                        VmBackupLog.Current.WriteErrorMessage("AddVirtualMachineToProtectionGroupAsync", VmBackupEventId.ProtectionGroupJobDeadlockDetecked, null, vm.SubscriptionId, target.ProtectionGroupName);
                        break;
                    default:
                        VmBackupLog.Current.WriteErrorMessage("AddVirtualMachineToProtectionGroupAsync", VmBackupEventId.UnexpectedTenantException, e, vm.SubscriptionId);
                        break;
                }
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("AddVirtualMachineToProtectionGroupAsync", VmBackupEventId.UnexpectedTenantException, e, vm.SubscriptionId);
                protectionError = true;
            }

            if (protectionError)
            {
                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subscriptionId, vm.VmId, (int)VirtualMachineActionState.ProtectionFailed);
            }

        }

        private async void BackupVirtualMachineAsync(Guid subscriptionId, VirtualMachine vm, string id)
        {
            bool backupError = false;

            try
            {
                VmBackupTarget target = await this.configStorageClient.SelectProtetionGroupByVirtualmachineAsync<VmBackupTarget>(subscriptionId, vm.VmId, VirtualMachineMapping.CreateVmBackupTarget);
                await this.dpmServerClient.BackupVirtualMachine(target.BackupServerName, target.ProtectionGroupName, id, target.UserName, target.Password);
                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subscriptionId, vm.VmId, (int)VirtualMachineActionState.BackupComplete);
                cache.Remove(subscriptionId.ToString());
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("BackupVirtualMachineAsync", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                backupError = true;
            }

            if (backupError)
            {
                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subscriptionId, vm.VmId, (int)VirtualMachineActionState.BackupFailed);
            }
        }

        private async void RemoveVirtualMachineProtectionAsync(Guid subscriptionId, Guid vmId, string id)
        {
            bool removeProtectionError = false;

            try
            {
                VmBackupTarget target = await this.configStorageClient.SelectProtetionGroupByVirtualmachineAsync<VmBackupTarget>(subscriptionId, vmId, VirtualMachineMapping.CreateVmBackupTarget);
                await this.dpmServerClient.RemoveVirtualMachineFromProtectionGroup(target.BackupServerName, target.ProtectionGroupName, id, target.UserName, target.Password);
                await this.configStorageClient.DeleteVirtualMachineRecordAsync(subscriptionId, vmId);
                cache.Remove(subscriptionId.ToString());
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("RemoveVirtualMachineProtectionAsync", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                removeProtectionError = true;
            }

            if (removeProtectionError)
            {
                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subscriptionId, vmId, (int)VirtualMachineActionState.RemoveProtectionFailed);
            }
        }

        private async void RestoreVirtualMachineAsync(Guid subscriptionId, Guid vmId, string id, string recoverySourceId)
        {
            bool restoreError = false;

            try
            {
                VmBackupTarget target = await this.configStorageClient.SelectProtetionGroupByVirtualmachineAsync<VmBackupTarget>(subscriptionId, vmId, VirtualMachineMapping.CreateVmBackupTarget);
                await this.dpmServerClient.RestoreVirtualMachine(target, id, recoverySourceId);
                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subscriptionId, vmId, (int)VirtualMachineActionState.RestoreComplete);
            }
            catch (Exception e)
            {
                VmBackupLog.Current.WriteErrorMessage("RestoreVirtualMachineAsync", VmBackupEventId.UnexpectedTenantException, e, subscriptionId);
                restoreError = true;
            }

            if (restoreError)
            {
                await this.configStorageClient.UpdateVirtualMachineActionStateAsync(subscriptionId, vmId, (int)VirtualMachineActionState.RestoreFailed);
            }
        }

    }


}