using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    public enum VirtualMachineActionState
    {
        None = 0,
        Protecting = 1,
        ProtectComplete = 2,
        BackingUp = 3,
        BackupComplete = 4,
        Restoring = 5,
        RestoreComplete = 6,
        RemovingProtection = 7,
        ProtectionFailed = 8,
        BackupFailed = 9,
        RestoreFailed = 10,
        RemoveProtectionFailed = 11
    }
}
