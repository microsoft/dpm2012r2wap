using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    public class VmmServerClient : PowerShellClient
    {
        public async Task CheckVmmConnectivity(VmmServer server)
        {
            await this.RunScriptAsync(PowerShellCommandFactory.GetCheckVmmConnectivityScript(server.ServerName), server.ServerName, server.UserName, server.Password);
        }
    }
}