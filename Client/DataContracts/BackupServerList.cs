// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    [CollectionDataContract(Name = "BackupServers", ItemName = "BackupServer", Namespace = Constants.DataContractNamespaces.Default)]
    public class BackupServerList : List<BackupServer>
    {
    }
}
