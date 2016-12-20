using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    public sealed class VmBackupDataSourceDetails
    {
        public Guid DataSourceId { get; set; }

        public Guid HyperVId { get; set; }

        public string Schedule { get; set; }

        public int TotalRecoveryPoints { get; set; }

        public string LatestRecorveryPoint { get; set; }
    }
}
