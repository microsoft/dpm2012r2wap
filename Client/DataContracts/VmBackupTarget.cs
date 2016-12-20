using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    public sealed class VmBackupTarget
    {
        public string ProtectionGroupName { get; set; }

        public string BackupServerName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
