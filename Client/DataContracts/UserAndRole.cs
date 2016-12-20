// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts
{
    [DataContract(Namespace = Constants.DataContractNamespaces.Default)]
    public class UserAndRole
    {
        [DataMember]
        public Guid RoleId { get; set; }

        [DataMember]
        public string RoleName { get; set; }

        [DataMember]
        public string UserName { get; set; }
    }
}
