CREATE PROCEDURE [VMBackup].[InsertBackupServer]
 	 @serverId uniqueidentifier
    ,@serverName nvarchar(128)
    ,@userName nvarchar(128)
    ,@password varbinary(2048)
    ,@serverGroupId uniqueidentifier

WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION

	DECLARE @groupId int;

	SELECT @groupId = Id FROM [VMBackup].[ServerGroups] WHERE GroupId = @serverGroupId;

	IF @groupId IS NULL
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	IF EXISTS (SELECT ServerId FROM [VMBackup].[BackupServers] WHERE [ServerName] = @serverName)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56002', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	INSERT INTO [VMBackup].[BackupServers]
			   ([ServerId]
			   ,[ServerName]
			   ,[UserName]
			   ,[Password]
			   ,[State]
			   ,[ServerGroupId])
		 VALUES
			   (@serverId
			   ,@serverName
			   ,@userName
			   ,@password
			   ,1
			   ,@groupId)

	SELECT serv.[ServerId], serv.[ServerName], serv.[UserName], serv.[Password], bss.[BackupServerStateName], grp.[GroupId], grp.[GroupName], ProtectionGroupCount=0, VirtualMachineCount=0
		FROM [VMBackup].[BackupServers] serv 
			INNER JOIN [VMBackup].[ServerGroups] grp on grp.Id = serv.ServerGroupId
			INNER JOIN [VMBackup].[BackupServerState] bss on bss.Id = serv.[State]
		WHERE serv.ServerId = @serverId

COMMIT TRANSACTION
END
