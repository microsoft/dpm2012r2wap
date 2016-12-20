CREATE PROCEDURE [VMBackup].[InsertServerGroup]
 	 @groupId uniqueidentifier
	,@groupName nvarchar(128)
    ,@azureBackupEnabled bit

WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION

	IF EXISTS(SELECT GroupId FROM [VMBackup].[ServerGroups] WHERE GroupId = @groupId)
	BEGIN
        ROLLBACK TRANSACTION 
        RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
        RETURN;
	END

	IF EXISTS(SELECT GroupId FROM [VMBackup].[ServerGroups] WHERE GroupName = @groupName)
	BEGIN
        ROLLBACK TRANSACTION 
        RAISERROR (N'56002', 16, 1); -- (msg_str, severity, state)
        RETURN;
	END

	INSERT INTO [VMBackup].[ServerGroups]
			   ([GroupId]
			   ,[GroupName]
			   ,[AzureBackupEnabled])
		 VALUES
			   (@groupId
			   ,@groupName
			   ,@azureBackupEnabled)

	SELECT GroupId, GroupName, AzureBackupEnabled, BackupServerCount=0, ProtectionGroupCount=0, VirtualMachineCount=0
		FROM [VMBackup].[ServerGroups]
		WHERE GroupId = @groupId;

COMMIT TRANSACTION
END

