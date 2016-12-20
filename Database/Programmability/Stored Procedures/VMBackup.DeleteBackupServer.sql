CREATE PROCEDURE [VMBackup].[DeleteBackupServer]
	@serverId uniqueidentifier
WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION

	IF NOT EXISTS (SELECT ServerId FROM [VMBackup].[BackupServers] WITH (HOLDLOCK, ROWLOCK, UPDLOCK) WHERE [ServerId] = @serverId)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	-- Check for protection groups
	IF EXISTS (SELECT serv.ServerId 
				FROM [VMBackup].[BackupServers] serv 
					INNER JOIN [VMBackup].[ProtectionGroups] pg on pg.BackupServerId = serv.Id
				WHERE serv.ServerId = @serverId)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56002', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	DELETE [VMBackup].[BackupServers] WHERE ServerId = @serverId

COMMIT TRANSACTION
END
