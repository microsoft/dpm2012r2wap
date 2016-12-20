using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    public class PowerShellScriptParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public PowerShellScriptParameter()
        {
        }

        public PowerShellScriptParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}