using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    public enum BackupServerState
    {
        Active = 1,
        Removing = 2,
        UnableToConnect = 3
    }
}
