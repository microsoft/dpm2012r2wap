//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.Service;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service
{
    public class Validator
    {

        /// <summary>
        /// Validates the subscription id.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <returns>Subscription Guid</returns>
        public static Guid ValidateSubscriptionId(string subscriptionId)
        {
            Guid id;
            bool parseGuid = Guid.TryParse(subscriptionId, out id);

            if (!parseGuid)
            {
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.InvalidSubscriptionFormat, subscriptionId);
                throw Utility.CreateHttpResponseException(message, HttpStatusCode.BadRequest);
            }

            return id;
        }
    }
}