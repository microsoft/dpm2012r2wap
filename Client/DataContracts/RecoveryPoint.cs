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
    public class RecoveryPoint
    {
        [DataMember]
        public Guid VmId { get; set; }

        [DataMember]
        public Guid RecoveryPointId { get; set; }

        [DataMember]
        public DateTime RepresentedPointInTime { get; set; }

        [DataMember]
        public string Location { get; set; }

    }
}
