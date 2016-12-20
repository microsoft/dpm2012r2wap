//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients.Mapping
{
    public class VirtualMachineMapping
    {

        public static VmBackupTarget CreateVmBackupTarget(SqlDataReader sqlDataReader)
        {
            VmBackupTarget target = new VmBackupTarget();

            if (sqlDataReader.Read())
            {
                target = GenerateVmBackupTarget(sqlDataReader);
            }

            return target;
        }

        public static List<VmBackupTarget> CreateVmBackupTargetList(SqlDataReader sqlDataReader)
        {
            List<VmBackupTarget> targets = new List<VmBackupTarget>();

            while (sqlDataReader.Read())
            {
                targets.Add(GenerateVmBackupTarget(sqlDataReader));
            }

            return targets;
        }

        public static VirtualMachineList CreateVirtualMachineList(SqlDataReader sqlDataReader)
        {
            VirtualMachineList virtualMachines = new VirtualMachineList();

            while (sqlDataReader.Read())
            {
                virtualMachines.Add(GenerateVirtualMachine(sqlDataReader));
            }

            return virtualMachines;
        }


        private static VirtualMachine GenerateVirtualMachine(SqlDataReader sqlDataReader)
        {
            VirtualMachine vm = new VirtualMachine();
            
            vm.Id = (Guid)sqlDataReader["VmmId"];
            vm.Name = (string)sqlDataReader["VmName"];
            vm.Owner = new UserAndRole();
            vm.Owner.UserName = (string)sqlDataReader["AccountAdminId"];
            vm.Owner.RoleId = (Guid)sqlDataReader["SubscriptionId"];
            vm.SubscriptionId = ((Guid)sqlDataReader["SubscriptionId"]).ToString();
            vm.Status = (string)sqlDataReader["VirtualMachineStateName"];
            vm.VmId = (Guid)sqlDataReader["HyperVId"];
            vm.ActionStatus = (string)sqlDataReader["ActionStateName"];
            return vm;
        }

        private static VmBackupTarget GenerateVmBackupTarget(SqlDataReader sqlDataReader)
        {
            VmBackupTarget target = new VmBackupTarget();

            target.BackupServerName = (string)sqlDataReader["BackupServerName"];
            target.UserName = (string)sqlDataReader["BackupServerUserName"];
            target.Password = (string)sqlDataReader["BackupServerPassword"];
            target.ProtectionGroupName = (string)sqlDataReader["ProtectionGroupName"];

            return target;
        }

    }
}