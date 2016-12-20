//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Azure.Portal.Configuration;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.TenantExtension
{
    public static class ClientFactory
    {
        //Get Service Management API endpoint
        private static Uri tenantApiUri;

        private static BearerMessageProcessingHandler messageHandler;

        //This client is used to communicate with the Hello World resource provider
        private static Lazy<VmBackupClient> vmBackupRestClient = new Lazy<VmBackupClient>(
           () => new VmBackupClient(tenantApiUri, messageHandler),
           LazyThreadSafetyMode.ExecutionAndPublication);

        static ClientFactory()
        {
            tenantApiUri = new Uri(AppManagementConfiguration.Instance.RdfeUnifiedManagementServiceUri);
            messageHandler = new BearerMessageProcessingHandler(new WebRequestHandler());
        }

        public static VmBackupClient VmBackupClient
        {
            get
            {
                return vmBackupRestClient.Value;
            }
        }
    }
}
