using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    internal static class SqlCommandFactory
    {
        //Select
        public const string SelectVirtualMachineCollectionProc = "[VMBackup].[SelectVirtualMachineCollection]";
        public const string SelectProtetionGroupsBySubscriptionProc = "[VMBackup].[SelectProtectionGroupsBySubscription]";
        public const string SelectProtectionGroupByVirtualMachineProc = "[VMBackup].[SelectProtectionGroupByVirtualMachine]";
        public const string SelectServerGroupByIdProc = "[VMBackup].[SelectServerGroupById]";
        public const string SelectServerGroupCollectionProc = "[VMBackup].[SelectServerGroupCollection]";
        public const string SelectBackupServerCollectionProc = "[VMBackup].[SelectBackupServerCollection]";
        public const string SelectSpfServerProc = "[VMBackup].[SelectSpfServer]";
        public const string SelectVmmServerCollectionProc = "[VMBackup].[SelectVmmServerCollection]";

        //Insert
        public const string InsertVirtualMachineRecordProc = "[VMBackup].[InsertVirtualMachineRecord]";
        public const string InsertServerGroupProc = "[VMBackup].[InsertServerGroup]";
        public const string InsertBackupServerProc = "[VMBackup].[InsertBackupServer]";
        public const string InsertOrUpdateSpfServerProc = "[VMBackup].[InsertOrUpdateSpfServer]";
        public const string InsertVmmServerProc = "[VMBackup].[InsertVmmServer]";

        //Update
        public const string CompleteVirtualMachineRecordProc = "[VMBackup].[CompleteVirtualMachineRecord]";
        public const string UpdateVirtualMachineActionStateProc = "[VMBackup].[UpdateVirtualMachineActionState]";
        public const string UpdateVmmServerProc = "[VMBackup].[UpdateVmmServer]";
        public const string UpdateBackupServerProc = "[VMBackup].[UpdateBackupServer]";

        //Delete
        public const string DeleteVirtualMachineRecordProc = "[VMBackup].[DeleteVirtualMachineRecord]";
        public const string DeleteBackupServerProc = "[VMBackup].[DeleteBackupServer]";
        public const string DeleteServerGroupProc = "[VMBackup].[DeleteServerGroup]"; 

        public static SqlParameter[] SelectVirtualMachineCollectionParameters(Guid subscriptionId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId)
            };
        }

        public static SqlParameter[] InsertVirtualMachineRecordParameters(Guid subscriptionId, Guid vmmId, Guid hyperVId, string vmName)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId),
                new SqlParameter("@vmmId", vmmId),
                new SqlParameter("@hyperVId", hyperVId),
                new SqlParameter("@vmName", vmName)
            };
        }

        public static SqlParameter[] CompleteVirtualMachineRecordParameters(Guid subscriptionId, Guid hyperVId, Guid dataSourceId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId),
                new SqlParameter("@hyperVId", hyperVId),
                new SqlParameter("@dataSourceId", dataSourceId)
            };
        }

        public static SqlParameter[] SelectProtetionGroupsBySubscriptionParameters(Guid subscriptionId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId)
            };
        }

        public static SqlParameter[] DeleteVirtualMachineRecordParameters(Guid subscriptionId, Guid hyperVId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId),
                new SqlParameter("@hyperVId", hyperVId),
            };
        }

        public static SqlParameter[] SelectProtectionGroupByVirtualMachineParameters(Guid subscriptionId, Guid hyperVId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId),
                new SqlParameter("@hyperVId", hyperVId),
            };
        }

        public static SqlParameter[] UpdateirtualMachineActionStateParameters(Guid subscriptionId, Guid hyperVId, int actionStateId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@subscriptionId", subscriptionId),
                new SqlParameter("@hyperVId", hyperVId),
                new SqlParameter("@actionStateId", actionStateId)
            };
        }

        public static SqlParameter[] InsertServerGroupParameters(Guid groupId, string groupName, bool azureBackupEnabled)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@groupId", groupId),
                new SqlParameter("@groupName", groupName),
                new SqlParameter("@azureBackupEnabled", azureBackupEnabled)
            };
        }

        public static SqlParameter[] InsertBackupServerParameters(Guid serverId, string serverName, string userName, byte[] password, Guid groupId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@serverId", serverId),
                new SqlParameter("@serverName", serverName),
                new SqlParameter("@userName", userName),
                new SqlParameter("@password", System.Data.SqlDbType.VarBinary, -1) { Value = password }, 
                new SqlParameter("@serverGroupId", groupId)
            };
        }

        public static SqlParameter[] SelectServerGroupByIdParameters(Guid groupId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@groupId", groupId)
            };
        }

        public static SqlParameter[] SelectServerGroupCollectionParameters()
        {
            return new SqlParameter[] 
            { 
            };
        }

        public static SqlParameter[] SelectBackupServerCollectionParameters()
        {
            return new SqlParameter[] 
            { 
            };
        }

        public static SqlParameter[] InsertOrUpdateSpfServerParameters(string adminUrl, string userName, byte[] password)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@adminUrl", adminUrl),
                new SqlParameter("@userName", userName),
                new SqlParameter("@password", System.Data.SqlDbType.VarBinary, -1) { Value = password }
            };
        }

        public static SqlParameter[] SelectSpfServerParameters()
        {
            return new SqlParameter[] 
            { 
            };
        }

        public static SqlParameter[] SelectVmmServerCollectionParameters()
        {
            return new SqlParameter[] 
            { 
            };
        }

        public static SqlParameter[] InsertVmmServerParameters(Guid stampId, string serverName, string userName, byte[] password)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@stampId", stampId),
                new SqlParameter("@serverName", serverName),
                new SqlParameter("@userName", userName),
                new SqlParameter("@password", System.Data.SqlDbType.VarBinary, -1) { Value = password }
            };
        }

        public static SqlParameter[] UpdateVmmServerParameters(Guid stampId, string userName, byte[] password)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@stampId", stampId),
                new SqlParameter("@userName", userName),
                new SqlParameter("@password", System.Data.SqlDbType.VarBinary, -1) { Value = password }
            };
        }

        public static SqlParameter[] UpdateBackupServerParameters(Guid serverId, string userName, byte[] password)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@serverId", serverId),
                new SqlParameter("@userName", userName),
                new SqlParameter("@password", System.Data.SqlDbType.VarBinary, -1) { Value = password }
            };
        }

        public static SqlParameter[] DeleteBackupServerParameters(Guid serverId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@serverId", serverId)
            };
        }

        public static SqlParameter[] DeleteServerGroupParameters(Guid groupId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@groupId", groupId)
            };
        }
    }
}