using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.AdminExtension.Models
{
    public interface IAdminResourceModel
    {
        /// <summary>
        /// Gets the Type
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the Display Name
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the Status
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Gets the Location
        /// </summary>
        string Location { get; }
    }
}
