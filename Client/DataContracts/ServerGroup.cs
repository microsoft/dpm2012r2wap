// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class ServerGroup
    {
        [DataMember]
        public Guid GroupId { get; set; }

        [DataMember]
        public string GroupName { get; set; }

        [DataMember]
        public bool AzureBackupEnabled { get; set; }

        [DataMember]
        public int BackupServerCount { get; set; }

        [DataMember]
        public int ProtectionGroupCount { get; set; }

        [DataMember]
        public int VirtualMachineCount { get; set; }
    }
}
