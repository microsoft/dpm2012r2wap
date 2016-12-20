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
    public class BackupServerMapping
    {

        private static SymmetricEncryptor encryptor = new SymmetricEncryptor(EncryptionConstants.EncryptionKey, EncryptionConstants.EncryptionAlgorithm);

        public static BackupServer CreateBackupServer(SqlDataReader sqlDataReader)
        {
            BackupServer server = new BackupServer();

            if (sqlDataReader.Read())
            {
                server = GenerateBackupServer(sqlDataReader, false);
            }

            return server;
        }

        public static BackupServer CreateBackupServerWithoutPassword(SqlDataReader sqlDataReader)
        {
            BackupServer server = new BackupServer();

            if (sqlDataReader.Read())
            {
                server = GenerateBackupServer(sqlDataReader, true);
            }

            return server;
        }

        public static BackupServerList CreateBackupServerCollectionWithoutPassword(SqlDataReader sqlDataReader)
        {
            BackupServerList servers = new BackupServerList();
            while (sqlDataReader.Read())
            {
                servers.Add(GenerateBackupServer(sqlDataReader, true));
            }

            return servers;
        }

        private static BackupServer GenerateBackupServer(SqlDataReader sqlDataReader, bool removePassword = true)
        {
            BackupServer server = new BackupServer();

            server.ServerId = (Guid)sqlDataReader["ServerId"];
            server.ServerName = (string)sqlDataReader["ServerName"];
            server.UserName = (string)sqlDataReader["UserName"];
            
            //Decrypt the password
            byte[] encryptedPassword = (byte[])sqlDataReader["Password"];
            server.Password = removePassword ? string.Empty : encryptor.DecryptData(encryptedPassword);

            server.State = (string)sqlDataReader["BackupServerStateName"];
            server.GroupId = (Guid)sqlDataReader["GroupId"];
            server.GroupName = (string)sqlDataReader["GroupName"];
            server.ProtectionGroupCount = (int)sqlDataReader["ProtectionGroupCount"];
            server.VirtualMachineCount = (int)sqlDataReader["VirtualMachineCount"];
            return server;
        }

    }
}