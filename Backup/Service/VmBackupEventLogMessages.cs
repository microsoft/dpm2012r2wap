using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service
{
    public class VmBackupEventLogMessages 
    {
        private static Dictionary<VmBackupEventId, string> dictionary = new Dictionary<VmBackupEventId, string>();
        static VmBackupEventLogMessages()
        {
            //Admin
            dictionary.Add(
                VmBackupEventId.UnexpectedAdminException,
                "An unexpected exception occured processing the request.");
            dictionary.Add(
                VmBackupEventId.ServerGroupIdAlreadyInUse,
                "The server group identifer '{0}' is already in use.");
            dictionary.Add(
                VmBackupEventId.ServerGroupNameAlreadyInUse,
                "The server group name '{0}' is already in use.");
            dictionary.Add(
                VmBackupEventId.ServerGroupNotFound,
                "The server group '{0}' cannot be found.");
            dictionary.Add(
                VmBackupEventId.ServerNameAlreadyInUse,
                "The server name '{0}' is already in use.");
            dictionary.Add(
                VmBackupEventId.AzureSubscriptionNotFound,
                "The group '{0}' chosen for server '{1}' requires the server to be setup with an Azure subscription but one was not found.");
            dictionary.Add(
                VmBackupEventId.AzureSubscriptionNotReady,
                "The group '{0}' chosen for server '{1}' requires the server to be setup with an Azure subscription but the subscription is not reporting that it installed and registered");
            dictionary.Add(
                VmBackupEventId.DpmVersionTooLow,
                "The DPM server '{0}' is reporting a too low version number '{1}'. The server must be 4.2.x or higher.");
            dictionary.Add(
                VmBackupEventId.CannnotConnectToServer,
                "Could not connect to server '{0}'.");
            dictionary.Add(
                VmBackupEventId.DeleteBackupServerNotFound,
                "DPM server with the ID '{0}' was not found.");
            dictionary.Add(
                VmBackupEventId.BackupServerHasProtectionGroups,
                "Could not delete the DPM server with ID '{0}' because it still had one or more protection groups. Delete the protectection groups and try again.");
            dictionary.Add(
                VmBackupEventId.DeleteServerGroupNotFound,
                "Server group with the ID '{0}' was not found.");
            dictionary.Add(
                VmBackupEventId.ServerGroupHasServers,
                "Could not delete the server group with ID '{0}' because it still had one or more DPM servers. Delete the servers and try again.");

            //Tenant
            dictionary.Add(
                VmBackupEventId.UnexpectedTenantException, 
                "An unexpected exception occured processing the request for subscription '{0}'.");
            dictionary.Add(
                VmBackupEventId.TooManyProtectionGroupAddRequests,
                "There were too many requests for adding protection for subscription '{0}'. Cannot protect virtual machine '{1}' at this time.");
            dictionary.Add(
                VmBackupEventId.ProtectionGroupNotFound,
                "Protection group not found for subscription '{0}'.");
            dictionary.Add(
                VmBackupEventId.ProtectionGroupJobDeadlockDetecked,
                "One or more jobs are currenty running for subscription '{0}', protection group '{1}' that would be impacted by running the current operation so the operation was cancelled.");
        }

        public static string GetMessage(VmBackupEventId eventId, params object[] args)
        {
            return String.Format(dictionary[eventId], args);
        }
    }
}