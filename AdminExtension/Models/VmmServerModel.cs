using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Models
{
    public class VmmServerModel : IAdminResourceModel
    {
        public Guid StampId { get; set; }

        public string ServerName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string State { get; set; }

                public string id
        {
            get
            {
                return StampId.ToString();
            }
        }

        public string Type
        {
            get 
            { 
                return "VmmServer"; 
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

        public VmmServerModel()
        {
        }

        public VmmServerModel(VmmServer server)
        {
            this.ServerName = server.ServerName;
            this.StampId = server.StampId;
            this.State = server.State;
            this.UserName = server.UserName;
        }

    }
}
