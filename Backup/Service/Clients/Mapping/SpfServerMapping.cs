//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.ApiClient.DataContracts;
using Microsoft.WindowsAzure.Server.Common;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients.Mapping
{
    public class SpfServerMapping
    {
        private static SymmetricEncryptor encryptor = new SymmetricEncryptor(EncryptionConstants.EncryptionKey, EncryptionConstants.EncryptionAlgorithm);

        public static SpfServer CreateSpfServer(SqlDataReader sqlDataReader)
        {
            SpfServer server = null;

            if (sqlDataReader.Read())
            {
                server = GenerateSpfServer(sqlDataReader, false);
            }

            return server;
        }

        public static SpfServer CreateSpfServerWithoutPassword(SqlDataReader sqlDataReader)
        {
            SpfServer server = null;

            if (sqlDataReader.Read())
            {
                server = GenerateSpfServer(sqlDataReader, true);
            }

            return server;
        }

        private static SpfServer GenerateSpfServer(SqlDataReader sqlDataReader, bool removePassword = true)
        {
            SpfServer server = new SpfServer();
            server.AdminUrl = (string)sqlDataReader["AdminUrl"];
            server.UserName = (string)sqlDataReader["UserName"];

            //Decrypt the password
            byte[] encryptedPassword = (byte[])sqlDataReader["Password"];
            server.Password = removePassword ? string.Empty : encryptor.DecryptData(encryptedPassword);

            return server;
        }
    }
}