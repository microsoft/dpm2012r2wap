using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Models
{
    public class BackupServerModel : IAdminResourceModel
    {
        public Guid ServerId { get; set; }

        public string ServerName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string State { get; set; }

        public Guid GroupId { get; set; }

        public string GroupName { get; set; }

        public int ProtectionGroupCount { get; set; }

        public int VirtualMachineCount { get; set; }

        public string id
        {
            get
            {
                return ServerId.ToString();
            }
        }

        public string Type
        {
            get 
            { 
                return "BackupServer"; 
            }
        }

        public string DisplayName
        {
            get 
            {
                return ServerName;
            }
        }

        public string Status
        {
            get 
            { 
                return State; 
            }
        }

        public string Location
        {
            get
            {
                return "Default";
            }
        }

        public BackupServerModel()
        {
        }

        public BackupServerModel(BackupServer server)
        {
            this.ServerName = server.ServerName;
            this.ServerId = server.ServerId;
            this.State = server.State;
            this.UserName = server.UserName;
            this.ProtectionGroupCount = server.ProtectionGroupCount;
            this.VirtualMachineCount = server.VirtualMachineCount;
            this.GroupId = server.GroupId;
            this.GroupName = server.GroupName;
        }
    }
}
