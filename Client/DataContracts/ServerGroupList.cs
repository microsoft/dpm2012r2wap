// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    [CollectionDataContract(Name = "ServerGroups", ItemName = "ServerGroup", Namespace = Constants.DataContractNamespaces.Default)]
    public class ServerGroupList : List<ServerGroup>
    {
    }
}
