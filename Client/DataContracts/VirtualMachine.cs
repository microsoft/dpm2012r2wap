// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

using SPF=Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.SpfTenant;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class VirtualMachine
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string SubscriptionId { get; set; }

        [DataMember]
        public Guid VmId { get; set; }

        [DataMember]
        public string ComputerName { get; set; }

        [DataMember]
        public Guid StampId { get; set; }

        [DataMember]
        public Guid CloudId { get; set; }

        [DataMember]
        public string CloudVMRoleName { get; set; }

        [DataMember]
        public string VmStatus { get; set; }

        [DataMember]
        public string VmStatusString { get; set; }

        [DataMember]
        public UserAndRole Owner { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string ActionStatus { get; set; }

        [DataMember]
        public int RecoveryPoints { get; set; }

        [DataMember]
        public string BackupPolicy { get; set; }

        [DataMember]
        public string LastRecoveryPoint { get; set; }

        public VirtualMachine()
        {
            Owner = new UserAndRole();
        }

        public VirtualMachine(SPF.VirtualMachine vm)
        {
            Guid emptyId = Guid.Empty;
            if (vm.VMId == null)
            {
                this.VmId = emptyId;
            }
            else
            {
                this.Id = vm.ID;
                this.SubscriptionId = vm.Owner.RoleID.ToString();
                this.VmId = (Guid)vm.VMId;
                this.Name = vm.Name;
                this.CloudId = (Guid)vm.CloudId;
                //this.CloudVMRoleName = vm.CloudVMRoleName;
                this.ComputerName = vm.ComputerName;
                this.Owner = new UserAndRole();
                this.Owner.RoleId = (Guid)vm.Owner.RoleID;
                this.Owner.RoleName = vm.Owner.RoleName;
                this.Owner.UserName = vm.Owner.UserName;
                this.StampId = vm.StampId;
                this.VmStatus = vm.Status;
                this.VmStatusString = vm.StatusString;
                this.Status = "NotProtected";
                this.ActionStatus = "None";
            }
        }
    }
}
