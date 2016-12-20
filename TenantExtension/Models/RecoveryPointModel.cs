using System;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using System.Globalization;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.TenantExtension.Models
{
    public class RecoveryPointModel
    {
        public Guid VmId { get; set; }

        public Guid RecoveryPointId { get; set; }

        public DateTime RepresentedPointInTime { get; set; }

        public string Location { get; set; }

        public string DateLocation
        {
            get
            {
                return RepresentedPointInTime.ToShortDateString() + " " + RepresentedPointInTime.ToShortTimeString() + " (" + Location + ")";
            }
        }

        public RecoveryPointModel()
        {
        }

        public RecoveryPointModel(RecoveryPoint thePoint)
        {
            this.VmId = thePoint.VmId;
            this.RecoveryPointId = thePoint.RecoveryPointId;
            this.RepresentedPointInTime = thePoint.RepresentedPointInTime;
            this.Location = thePoint.Location;
        }
    }
}
