// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    [CollectionDataContract(Name = "VirtualMachines", ItemName = "VirtualMachine", Namespace = Constants.DataContractNamespaces.Default)]
    public class VirtualMachineList : List<VirtualMachine>
    {
    }
}
