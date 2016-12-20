CREATE PROCEDURE [VMBackup].[UpdateBackupServer]
 	 @serverId uniqueidentifier
    ,@userName nvarchar(128)
    ,@password varbinary(2048)

WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION


	IF NOT EXISTS (SELECT ServerId FROM [VMBackup].[BackupServers] WHERE [ServerId] = @serverId)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	UPDATE [VMBackup].[BackupServers]
		SET [UserName] = @userName
			,[Password] = @password
			,[State] = 1
		WHERE ServerId = @serverId

	SELECT serv.[ServerId], serv.[ServerName], serv.[UserName], serv.[Password], bss.[BackupServerStateName], grp.[GroupId], grp.[GroupName], ProtectionGroupCount=ISNULL(Count(pg.Id), 0), VirtualMachineCount=ISNULL(Count(vm.Id), 0)
		FROM [VMBackup].[BackupServers] serv 
			INNER JOIN [VMBackup].[ServerGroups] grp on grp.Id = serv.ServerGroupId
			INNER JOIN [VMBackup].[BackupServerState] bss on bss.Id = serv.[State]
			LEFT OUTER JOIN [VMBackup].[ProtectionGroups] pg on pg.BackupServerId = serv.Id
			LEFT OUTER JOIN [VMBackup].[VirtualMachines] vm on vm.ProtectionGroupId = pg.Id
		WHERE serv.[ServerId] = @serverId
		GROUP BY serv.[ServerId], serv.[ServerName], serv.[UserName], serv.[Password], bss.[BackupServerStateName], grp.[GroupId], grp.[GroupName]

COMMIT TRANSACTION
END

