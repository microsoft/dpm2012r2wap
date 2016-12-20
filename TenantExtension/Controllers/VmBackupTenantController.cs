//-----------------------------------------------------------------------
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Common;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.TenantExtension.Models;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using System;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.TenantExtension.Controllers
{
    [RequireHttps]
    [OutputCache(Location = OutputCacheLocation.None)]
    [PortalExceptionHandler]
    public sealed class VmBackupTenantController : ExtensionController
    {

        [HttpPost]
        public async Task<JsonResult> ListProtectedVirtualMachines(string[] subscriptionIds)
        {
            var virtualMachines = new List<VirtualMachineModel>();
            var protectedMachines = new List<VirtualMachineModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var vmsFromApi = await ClientFactory.VmBackupClient.ListProtectedVirtualMachines(subId);
                virtualMachines.AddRange(vmsFromApi.Select(vm => new VirtualMachineModel(vm)));
            }

            foreach (var vm in virtualMachines)
            {
                if (vm.Status == "Protected")
                {
                    protectedMachines.Add(vm);
                }
            }
            return this.JsonDataSet(protectedMachines);
        }

        [HttpPost]
        public async Task<JsonResult> ListVirtualMachines(string[] subscriptionIds)
        {
            var virtualMachines = new List<VirtualMachineModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                var vmsFromApi = await ClientFactory.VmBackupClient.ListVirtualMachnes(subId);
                virtualMachines.AddRange(vmsFromApi.Select(vm => new VirtualMachineModel(vm)));
            } 
            
            return this.JsonDataSet(virtualMachines);
        }

        [HttpPost]
        public async Task<JsonResult> AddVirtualMachineToProtectionGroup(string subscriptionId, string vmId, string vmmId, string vmName)
        {
            try
            {
                VirtualMachine vmToAdd = new VirtualMachine()
                {
                    SubscriptionId = subscriptionId,
                    VmId = Guid.Parse(vmId),
                    Id = Guid.Parse(vmmId),
                    Name = vmName
                };
                VirtualMachine protectedVM = await ClientFactory.VmBackupClient.AddVirtualMachineToProtectionGroup(subscriptionId, vmToAdd);
                return Json(new VirtualMachineModel(protectedVM));
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveProtectionFromVirtualMachine(string subscriptionId, string vmId)
        {
            try
            {
                await ClientFactory.VmBackupClient.RemoveVirtualMachineFromProtectionGroup(subscriptionId, vmId);
                return Json(null);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        public async Task<JsonResult> BackupVirtualMachine(string subscriptionId, string vmId)
        {
            try
            {
                VirtualMachine vmToBackup = new VirtualMachine()
                {
                    SubscriptionId = subscriptionId,
                    VmId = Guid.Parse(vmId)
                };
                await ClientFactory.VmBackupClient.BackupVirtualMachine(subscriptionId, vmId, vmToBackup);
                return Json(null);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        public async Task<JsonResult> ListRecoveryPoints(string subscriptionId, string vmId)
        {
            List<RecoveryPointModel> points = new List<RecoveryPointModel>();
            try
            {
                var recoveryPointsFromApi = await ClientFactory.VmBackupClient.ListRecoveryPoints(subscriptionId, vmId);
                points.AddRange(recoveryPointsFromApi.Select(point => new RecoveryPointModel(point)));
                return Json(points);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        [HttpPost]
        public async Task<JsonResult> RestoreVirtualMachine(string subscriptionId, string vmId, string recoverySourceId)
        {
            try
            {
                VirtualMachine vmToBackup = new VirtualMachine()
                {
                    SubscriptionId = subscriptionId,
                    VmId = Guid.Parse(vmId)
                };
                await ClientFactory.VmBackupClient.RestoreVirtualMachine(subscriptionId, vmId, recoverySourceId, vmToBackup);
                return Json(null);
            }
            catch (Exception e)
            {
                throw this.HandleException(e);
            }
        }

        /// <summary>
        /// List file shares belong to subscription
        /// NOTE: For this sample dummy entries will be displayed
        /// </summary>
        /// <param name="subscriptionIds"></param>
        /// <returns></returns>
        [HttpPost]        
        public async Task<JsonResult> ListFileShares(string[] subscriptionIds)
        {
            // Make the requests sequentially for simplicity
            var fileShares = new List<FileShareModel>();

            if (subscriptionIds == null || subscriptionIds.Length == 0)
            {
                throw new HttpException("Subscription Id not found");
            }

            foreach (var subId in subscriptionIds)
            {
                //var fileSharesFromApi = await ClientFactory.VmBackupClient.ListFileSharesAsync(subId);
                //fileShares.AddRange(fileSharesFromApi.Select(d => new FileShareModel(d)));
            }

            return this.JsonDataSet(fileShares);
        }

        /// <summary>
        /// Create new file share for subscription
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="fileShareToCreate"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult>  CreateFileShare(string subscriptionId, FileShareModel fileShareToCreate)
        {
            //await ClientFactory.VmBackupClient.CreateFileShareAsync(subscriptionId, fileShareToCreate.ToApiObject());

            return this.Json(fileShareToCreate);
        }

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
    }
}