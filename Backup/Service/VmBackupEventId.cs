using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service
{
    public enum VmBackupEventId
    {
        //Admin
        UnexpectedAdminException = 100,
        ServerGroupIdAlreadyInUse,
        ServerGroupNameAlreadyInUse,
        ServerGroupNotFound,
        ServerNameAlreadyInUse,
        AzureSubscriptionNotFound,
        AzureSubscriptionNotReady,
        DpmVersionTooLow,
        CannnotConnectToServer,
        DeleteBackupServerNotFound,
        BackupServerHasProtectionGroups,
        DeleteServerGroupNotFound,
        ServerGroupHasServers,

        //Tenant
        UnexpectedTenantException = 200,
        TooManyProtectionGroupAddRequests,
        ProtectionGroupNotFound,
        ProtectionGroupJobDeadlockDetecked
    }
}