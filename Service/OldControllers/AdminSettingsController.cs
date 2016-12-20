//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Web.Http;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Old;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Controllers
{
    public class AdminSettingsController : ApiController
    {
        public static AdminSettings settings;

        static AdminSettingsController()
        {
            settings = new AdminSettings
            {
                EndpointAddress = "http://dummyservice",
                Username = "testUser",
                Password = "Password"
            };
        }

        [HttpGet]
        public AdminSettings GetAdminSettings()
        {
           return settings;
        }

        [HttpPut]
        public void UpdateAdminSettings(AdminSettings newSettings)
        {
            if (newSettings == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.NullInput); 
            }

            settings = newSettings;
        }
    }
}
