using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Models
{
    public class SpfServerModel 
    {
        public string AdminUrl { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public SpfServerModel(SpfServer server)
        {
            this.AdminUrl = server.AdminUrl;
            this.UserName = server.UserName;
        }

    }
}
