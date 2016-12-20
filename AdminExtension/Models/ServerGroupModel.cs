using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Models
{
    public class ServerGroupModel : IAdminResourceModel
    {
        public Guid GroupId { get; set; }

        public string GroupName { get; set; }

        public bool AzureBackupEnabled { get; set; }

        public int BackupServerCount { get; set; }
        
        public int ProtectionGroupCount { get; set; }

        public int VirtualMachineCount { get; set; }

        public ServerGroupModel()
        {
        }

        public ServerGroupModel(ServerGroup group)
        {
            this.GroupId = group.GroupId;
            this.GroupName = group.GroupName;
            this.AzureBackupEnabled = group.AzureBackupEnabled;
            this.BackupServerCount = group.BackupServerCount;
            this.ProtectionGroupCount = group.ProtectionGroupCount;
            this.VirtualMachineCount = group.VirtualMachineCount;
        }

        public string id
        {
            get
            {
                return this.GroupName;
            }
        }

        public string Type
        {
            get 
            { 
                return "VmBackup_ServerGroup"; 
            }
        }

        public string DisplayName
        {
            get 
            { 
                return GroupName; 
            }
        }

        public string Status
        {
            get 
            {
                if (BackupServerCount == 0)
                {
                    return "Ready";
                }
                else
                {
                    return "Active";
                }
            }
        }

        public string Location
        {
            get 
            { 
                return "Default"; 
            }
        }
    }
}
