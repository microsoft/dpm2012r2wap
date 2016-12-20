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
    public class VmmServerMapping
    {
        private static SymmetricEncryptor encryptor = new SymmetricEncryptor(EncryptionConstants.EncryptionKey, EncryptionConstants.EncryptionAlgorithm);

        public static VmmServer CreateVmmServer(SqlDataReader sqlDataReader)
        {
            VmmServer server = null;

            if (sqlDataReader.Read())
            {
                server = GenerateVmmServer(sqlDataReader, false);
            }

            return server;
        }

        public static VmmServer CreateVmmServerWithoutPassword(SqlDataReader sqlDataReader)
        {
            VmmServer server = null;

            if (sqlDataReader.Read())
            {
                server = GenerateVmmServer(sqlDataReader, true);
            }

            return server;
        }

        public static VmmServerList CreateVmmServerCollectonWithoutPassword(SqlDataReader sqlDataReader)
        {
            VmmServerList servers = new VmmServerList();

            while (sqlDataReader.Read())
            {
                servers.Add(GenerateVmmServer(sqlDataReader, true));
            }

            return servers;
        }

        private static VmmServer GenerateVmmServer(SqlDataReader sqlDataReader, bool removePassword = true)
        {
            VmmServer server = new VmmServer();
            server.StampId = (Guid)sqlDataReader["StampId"];
            server.ServerName = (string)sqlDataReader["ServerName"];
            server.UserName = (string)sqlDataReader["UserName"];

            //Decrypt the password
            byte[] encryptedPassword = (byte[])sqlDataReader["Password"];
            server.Password = removePassword ? string.Empty : encryptor.DecryptData(encryptedPassword);

            server.State = "Registered";

            return server;
        }
    }
}