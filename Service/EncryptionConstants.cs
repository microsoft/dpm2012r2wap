using Microsoft.WindowsAzure.Server.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service
{
    public static class EncryptionConstants
    {
        public const string EncryptionKeySetting = "EncryptionKey";
        public const string EncryptionAlgorithmSetting = "SymmetricEncryptionAlgorithm";

        /// <summary>
        /// Gets the encryption key
        /// </summary>
        public static byte[] EncryptionKey
        {
            get
            {
                var encryptionKey = ConfigurationManager.AppSettings[EncryptionConstants.EncryptionKeySetting];

                if (string.IsNullOrEmpty(encryptionKey))
                {
                    throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "ConfigurationInfoNotFound {0}", EncryptionConstants.EncryptionKeySetting));
                }

                return Convert.FromBase64String(encryptionKey);
            }
        }

        /// <summary>
        /// Gets the encryption algorithm
        /// </summary>
        public static string EncryptionAlgorithm
        {
            get
            {
                var encryptionAlgorithm = ConfigurationManager.AppSettings[EncryptionConstants.EncryptionAlgorithmSetting];

                if (string.IsNullOrEmpty(encryptionAlgorithm))
                {
                    throw new ApplicationException(string.Format(CultureInfo.CurrentCulture, "ConfigurationInfoNotFound {0}", EncryptionConstants.EncryptionAlgorithmSetting));
                }

                return encryptionAlgorithm;
            }
        }
    }

}