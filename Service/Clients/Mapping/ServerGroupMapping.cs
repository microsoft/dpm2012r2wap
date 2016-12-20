//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients.Mapping
{
    public class ServerGroupMapping
    {
        public static ServerGroup CreateServerGroup(SqlDataReader sqlDataReader)
        {

            if (sqlDataReader.Read())
            {
                ServerGroup group = GenerateServerGroup(sqlDataReader);
                return group;
            }
            else
            {
                return null;
            }
        }

        public static ServerGroupList CreateServerGroupCollection(SqlDataReader sqlDataReader)
        {
            ServerGroupList groups = new ServerGroupList();
            while (sqlDataReader.Read())
            {
                groups.Add(GenerateServerGroup(sqlDataReader));
            }

            return groups;
        }


        private static ServerGroup GenerateServerGroup(SqlDataReader sqlDataReader)
        {
            ServerGroup group = new ServerGroup();

            group.GroupId = (Guid)sqlDataReader["GroupId"];
            group.GroupName = (string)sqlDataReader["GroupName"];
            group.AzureBackupEnabled = (bool)sqlDataReader["AzureBackupEnabled"];
            group.BackupServerCount = (int)sqlDataReader["BackupServerCount"];
            group.ProtectionGroupCount = (int)sqlDataReader["ProtectionGroupCount"];
            group.VirtualMachineCount = (int)sqlDataReader["VirtualMachineCount"];

            return group;
        }

    }
}