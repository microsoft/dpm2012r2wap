CREATE PROCEDURE [VMBackup].[DeleteServerGroup]
 	 @groupId uniqueidentifier
WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION

	IF NOT EXISTS (SELECT GroupId FROM [VMBackup].[ServerGroups] WITH (HOLDLOCK, ROWLOCK, UPDLOCK) WHERE [GroupId] = @groupId)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	-- Check for backup servers
	IF EXISTS (SELECT grp.GroupId
				FROM [VMBackup].[ServerGroups] grp 
					INNER JOIN [VMBackup].[BackupServers] serv on serv.ServerGroupId = grp.Id
				WHERE grp.GroupId = @groupId)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56002', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	DELETE [VMBackup].[ServerGroups] WHERE GroupId = @groupId

COMMIT TRANSACTION
END