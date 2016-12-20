using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Resources;
using Microsoft.Azure.Portal.DynamicContent;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources
{
    [Export(typeof(IResourceManagers))]
    public class ResourceContent : IResourceManagers
    {
        /// <summary>
        /// Gets the ResourceManagers for this dll
        /// </summary>
        public IEnumerable<ResourceManager> ResourceManagers
        {
            get
            {
                yield return AdminExtension.Resource.ResourceManager;
                yield return TenantExtension.Resource.ResourceManager;
            }
        }
    }
}
