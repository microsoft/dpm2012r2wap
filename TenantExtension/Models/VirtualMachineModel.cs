using System;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using System.Globalization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.TenantExtension.Models
{
    public class VirtualMachineModel
    {
        // The consts below are internal data, no need to localize
        private const string TypeVmBackup = "VmBackup";
        private const string LocationDefault = "Default";

        public string Name { get; set; }

        public Guid Id { get; set; }

        public string SubscriptionId { get; set; }

        public string Type
        {
            get
            {
                return TypeVmBackup; // Internal data, no need to localize
            }
        }

        public string ComputerName { get; set; }

        public Guid StampId { get; set; }

        public Guid CloudId { get; set; }

        public string CloudVMRoleName { get; set; }

        public string VmStatus { get; set; }

        public string VmStatusString { get; set; }

        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public string UserName { get; set; }

        public string Status { get; set; }

        public string ActionStatus { get; set; }

        public int RecoveryPoints { get; set; }

        public string BackupPolicy { get; set; }

        public string LastRecoveryPoint { get; set; }

        public Guid VmId { get; set; }

        public bool canNavigate
        {
            get
            {
                return false;
            }
        }

        public string id
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "[{0}].[{1}]", this.Type, this.Name);
            }
        }

        public VirtualMachineModel()
        {
        }

        public VirtualMachineModel(VirtualMachine vm)
        {
            this.Name = vm.Name;
            this.SubscriptionId = vm.SubscriptionId;
            this.RoleId = vm.Owner.RoleId;
            this.RoleName = vm.Owner.RoleName;
            this.ComputerName = vm.ComputerName;
            this.CloudId = vm.CloudId;
            this.VmStatus = vm.VmStatus;
            this.VmStatusString = vm.VmStatusString;
            this.StampId = vm.StampId;
            this.UserName = vm.Owner.UserName;
            this.Id = vm.Id;
            this.CloudVMRoleName = vm.CloudVMRoleName;
            this.Status = vm.Status;
            this.ActionStatus = vm.ActionStatus;
            this.RecoveryPoints = vm.RecoveryPoints;
            this.BackupPolicy = vm.BackupPolicy;
            this.LastRecoveryPoint = vm.LastRecoveryPoint;
            this.VmId = vm.VmId;
        }
    }
}
