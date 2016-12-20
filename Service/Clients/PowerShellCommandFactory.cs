using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    public class PowerShellCommandFactory
    {
        static Assembly _assembly = Assembly.GetExecutingAssembly();
        static string includesScript;

        //TODO: This includes a hack (just under the import) to refresh all datasources because we don't have access to what host the VM lives on.
        //The fix is either in an updated SPF (already asked) or requiring us to register all VMM servers and querying VMM directly for the info.
        private const string AddVirtualMachineToProtectionGroupScript = "AddVirtualMachineToProtectionGroup.ps1";
        private const string BackupVirtualMachineScript = "BackupVirtualMachine.ps1";
        private const string LatestDataSourceDataScript = "LatestDataSourceData.ps1";
        private const string ListDataSourceRecoveryPointsScript = "ListDataSourceRecoveryPoints.ps1";
        private const string RemoveProtectionFromVirtualMachneScript = "RemoveProtectionFromVirtualMachine.ps1";
        private const string RestoreVirtualMachineScript = "RestoreVirtualMachine.ps1";
        private const string CheckDpmVersionAndConnectivityScript = "CheckDpmVersionAndConnectivity.ps1";
        private const string CheckDpmAzureSubscriptionScript = "CheckDpmAzureSubscription.ps1";

        private const string CheckVmmConnectivityScript = "CheckVmmConnectivity.ps1";

        static PowerShellCommandFactory()
        {
            // Get the includes file that has common functions like error handling
            string includesFile = "Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.PowerShellScripts.Includes.ps1";
            using (var reader = new StreamReader(_assembly.GetManifestResourceStream(includesFile)))
            {
                includesScript = reader.ReadToEnd();
                reader.Close();
            }
        }

        #region DPM Scripts

        public static string GetAddVirtualMachineToProtectionGroupScript(string dpmServerName, string protectionGroup, string virtualMachineId)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName),
                new PowerShellScriptParameter("@protectionGroup", protectionGroup),
                new PowerShellScriptParameter("@vmId", virtualMachineId)
            };

            return GetScript(AddVirtualMachineToProtectionGroupScript, parameters);
            //return String.Format(AddVirtualMachineToProtectionGroupScript, dpmServerName, protectionGroup, virtualMachineId);
        }

        public static string GetLatestDataSourceDataScript(string dpmServerName, string protectionGroup)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName),
                new PowerShellScriptParameter("@protectionGroup", protectionGroup)
            };

            return GetScript(LatestDataSourceDataScript, parameters);
        }

        public static string GetRemoveProtectionFromVirtualMachneScript(string dpmServerName, string protectionGroup, string virtualMachineId)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName),
                new PowerShellScriptParameter("@protectionGroup", protectionGroup),
                new PowerShellScriptParameter("@vmId", virtualMachineId)
            };

            return GetScript(RemoveProtectionFromVirtualMachneScript, parameters);
        }

        public static string GetBackupVirtualMachineScript(string dpmServerName, string protectionGroup, string virtualMachineId)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName),
                new PowerShellScriptParameter("@protectionGroup", protectionGroup),
                new PowerShellScriptParameter("@vmId", virtualMachineId)
            };

            return GetScript(BackupVirtualMachineScript, parameters);
        }

        public static string GetListDataSourceRecoveryPointsScript(string dpmServerName, string protectionGroup, string virtualMachineId)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName),
                new PowerShellScriptParameter("@protectionGroup", protectionGroup),
                new PowerShellScriptParameter("@vmId", virtualMachineId)
            };

            return GetScript(ListDataSourceRecoveryPointsScript, parameters);
        }

        public static string GetRestoreVirtualMachineScript(string dpmServerName, string protectionGroup, string virtualMachineId, string recoverySourceId)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName),
                new PowerShellScriptParameter("@protectionGroup", protectionGroup),
                new PowerShellScriptParameter("@vmId", virtualMachineId),
                new PowerShellScriptParameter("@recoverySourceId", recoverySourceId)
            };

            return GetScript(RestoreVirtualMachineScript, parameters);
        }

        public static string GetCheckDpmVersionAndConnectivityScript(string dpmServerName)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName)
            };

            return GetScript(CheckDpmVersionAndConnectivityScript, parameters);
        }

        public static string GetCheckDpmAzureSubscriptionScript(string dpmServerName)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", dpmServerName)
            };

            return GetScript(CheckDpmAzureSubscriptionScript, parameters);
        }

        #endregion

        #region VMM Scripts

        public static string GetCheckVmmConnectivityScript(string vmmServerName)
        {
            PowerShellScriptParameter[] parameters = new PowerShellScriptParameter[] {
                new PowerShellScriptParameter("@server", vmmServerName)
            };

            return GetScript(CheckVmmConnectivityScript, parameters);
        }

        #endregion

        private static string GetScript(string name, PowerShellScriptParameter[] parameters)
        {
            StringBuilder scriptBuilder = new StringBuilder(includesScript);

            // Get the script
            string scriptName = "Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.PowerShellScripts." + name;
            using (var reader = new StreamReader(_assembly.GetManifestResourceStream(scriptName)))
            {
                scriptBuilder.Append(reader.ReadToEnd());
                reader.Close();
                foreach (PowerShellScriptParameter parameter in parameters)
                {
                    scriptBuilder.Replace(parameter.Name, parameter.Value.ToString());
                }
                return scriptBuilder.ToString();
            }
        }
    }
}