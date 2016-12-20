using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    public class BackupServerVersion
    {
        public int Major { get; set; }

        public int Minor { get; set; }

        public int Build { get; set; }

        public int Revision { get; set; }

        public string VersionString
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
            }
        }
    }
}
