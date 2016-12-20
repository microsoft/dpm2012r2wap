using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    public class DpmServerClient : PowerShellClient
    {
        public async Task<Guid> AddVirtualMachineToProtectionGroup(string dpmServer, string protectionGroup, string vmId, string userName, string password)
        {
            Collection<PSObject> returnData = await this.RunScriptAsync(PowerShellCommandFactory.GetAddVirtualMachineToProtectionGroupScript(dpmServer, protectionGroup, vmId), dpmServer, userName, password);
            return (Guid)returnData[0].Members["DatasourceId"].Value;
        }

        public async Task<List<VmBackupDataSourceDetails>> GetLatestDataSourceData(string dpmServer, string protectionGroup, string userName, string password)
        {
            List<VmBackupDataSourceDetails> details = new List<VmBackupDataSourceDetails>();
            Collection<PSObject> returnData = await this.RunScriptAsync(PowerShellCommandFactory.GetLatestDataSourceDataScript(dpmServer, protectionGroup), dpmServer, userName, password);
            foreach (PSObject obj in returnData)
            {
                VmBackupDataSourceDetails detail = new VmBackupDataSourceDetails();
                detail.DataSourceId = (Guid)obj.Members["DatasourceId"].Value;
                Guid id;
                bool result = Guid.TryParse((string)obj.Members["ComponentName"].Value, out id);
                if (result)
                {
                    detail.HyperVId = id;
                }
                else
                {
                    continue;
                }
                detail.Schedule = (string)obj.Members["Schedule"].Value;
                detail.TotalRecoveryPoints = (int)obj.Members["TotalRecoveryPoints"].Value;
                if (detail.TotalRecoveryPoints > 0)
                {
                    detail.LatestRecorveryPoint = ((DateTime)obj.Members["LatestRecoveryPoint"].Value).ToString();
                }
                details.Add(detail);
            }
            return details;
        }

        public async Task RemoveVirtualMachineFromProtectionGroup(string dpmServer, string protectionGroup, string vmId, string userName, string password)
        {
            await this.RunScriptAsync(PowerShellCommandFactory.GetRemoveProtectionFromVirtualMachneScript(dpmServer, protectionGroup, vmId), dpmServer, userName, password);
        }

        public async Task BackupVirtualMachine(string dpmServer, string protectionGroup, string vmId, string userName, string password)
        {
            await this.RunScriptAsync(PowerShellCommandFactory.GetBackupVirtualMachineScript(dpmServer, protectionGroup, vmId), dpmServer, userName, password);
        }

        public async Task<RecoveryPointList> ListRecoveryPoints(string dpmServer, string protectionGroup, string vmId, string userName, string password)
        {
            RecoveryPointList recoveryPoints = new RecoveryPointList();
            Collection<PSObject> returnData = await this.RunScriptAsync(PowerShellCommandFactory.GetListDataSourceRecoveryPointsScript(dpmServer, protectionGroup, vmId), dpmServer, userName, password);

            foreach (PSObject obj in returnData)
            {
                RecoveryPoint point = new RecoveryPoint();
                Guid id;
                bool result = Guid.TryParse((string)obj.Members["ComponentName"].Value, out id);
                if (result)
                {
                    point.VmId = id;
                }
                else
                {
                    continue;
                }
                point.RecoveryPointId = (Guid)obj.Members["RecoverySourceId"].Value;
                point.RepresentedPointInTime = (DateTime)obj.Members["RepresentedPointInTime"].Value;
                point.Location = ((PSObject)obj.Members["Location"].Value).ToString();

                recoveryPoints.Add(point);
            }

            return recoveryPoints;
        }

        public async Task RestoreVirtualMachine(VmBackupTarget target, string vmId, string recoverySourceId)
        {
            await this.RunScriptAsync(
                PowerShellCommandFactory.GetRestoreVirtualMachineScript(target.BackupServerName, target.ProtectionGroupName, vmId, recoverySourceId), 
                target.BackupServerName, 
                target.UserName, 
                target.Password);
        }

        public async Task<BackupServerVersion> CheckServerVersionAndConnectivity(string dpmServer, string userName, string password)
        {
            Collection<PSObject> returnData = await this.RunScriptAsync(PowerShellCommandFactory.GetCheckDpmVersionAndConnectivityScript(dpmServer), dpmServer, userName, password);
            if (returnData != null && returnData.Count > 0)
            {
                BackupServerVersion version = new BackupServerVersion()
                {
                    Major = (int)returnData[0].Members["Major"].Value,
                    Minor = (int)returnData[0].Members["Minor"].Value,
                    Build = (int)returnData[0].Members["Build"].Value,
                    Revision = (int)returnData[0].Members["Revision"].Value
                };
                return version;
            }
            else
            {
                throw new IndexOutOfRangeException("No version returned!");
            }
        }

        public async Task CheckAzureSubscription(string dpmServer, string userName, string password)
        {
            await this.RunScriptAsync(PowerShellCommandFactory.GetCheckDpmAzureSubscriptionScript(dpmServer), dpmServer, userName, password);
        }
    }
}