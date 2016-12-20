using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients
{
    public abstract class PowerShellClient : IDisposable
    {
        private static Dictionary<string, RunspacePool> RunspacePools = new Dictionary<string, RunspacePool>();

        protected Task<Collection<PSObject>> RunScriptAsync(string script, string server, string userName, string password)
        {
            Task<Collection<PSObject>> t = Task.Factory.StartNew(() =>
            {
                using (var powershell = System.Management.Automation.PowerShell.Create())
                {
                    try
                    {
                        //powershell.Runspace = CreateRemoteRunspace(server, userName, password);
                        powershell.RunspacePool = GetRunspacePool(server, userName, password);
                        //powershell.Runspace.Open();
                        powershell.AddScript(script);
                        var results = powershell.Invoke();
                        return results;
                    }
                    finally
                    {
                        //if (powershell.Runspace != null)
                        //{
                        //    powershell.Runspace.Close();
                        //}
                    }
                }
            });

            return t;
        }

        protected RunspacePool GetRunspacePool(string server, string userName, string password)
        {
            if (RunspacePools.ContainsKey(server))
            {
                return RunspacePools[server];
            }
            else
            {
                System.Security.SecureString pwd = new System.Security.SecureString();
                foreach (char c in password.ToCharArray())
                {
                    pwd.AppendChar(c);
                }
                var remoteComputer = new Uri(String.Format("{0}://{1}:5985/wsman", "http", server));
                var connection = new WSManConnectionInfo(remoteComputer, "", new PSCredential(userName, pwd));
                connection.AuthenticationMechanism = AuthenticationMechanism.Credssp;
                RunspacePool pool = RunspaceFactory.CreateRunspacePool(1, 5, connection);
                pool.ApartmentState = System.Threading.ApartmentState.MTA;
                pool.Open();
                RunspacePools[server] = pool;
                return pool;
            }
        }

        protected Runspace CreateRemoteRunspace(string server, string userName, string password)
        {
            System.Security.SecureString pwd = new System.Security.SecureString();
            foreach (char c in password.ToCharArray())
            {
                pwd.AppendChar(c);
            }
            var remoteComputer = new Uri(String.Format("{0}://{1}:5985/wsman", "http", server));
            var connection = new WSManConnectionInfo(remoteComputer, "", new PSCredential(userName, pwd));
            connection.AuthenticationMechanism = AuthenticationMechanism.Credssp;
            Runspace r = RunspaceFactory.CreateRunspace(connection);
            return r;
            //    RunspacePool pool = RunspaceFactory.CreateRunspacePool(1,1, connection);
            //    RunspacePools[server] = pool;
            //    pool.Open();
            //    return pool;
            //}
        }

        public void Dispose()
        {
            foreach (RunspacePool pool in RunspacePools.Values)
            {
                pool.Dispose();
            }
        }
    }
}