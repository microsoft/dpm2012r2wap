CREATE PROCEDURE [VMBackup].[InsertVirtualMachineRecord]
            @subscriptionId uniqueidentifier
           ,@vmmId uniqueidentifier
           ,@hyperVId uniqueidentifier
           ,@vmName nvarchar(128)
           
WITH EXECUTE AS CALLER
AS
BEGIN
	DECLARE @protectionGroupId int,
			@protectionGroupName nvarchar(128),
			@backupServerName nvarchar(128),
			@userName nvarchar(128),
			@password nvarchar(128),
			@protectingCount int;

	BEGIN TRANSACTION
	SELECT @protectionGroupId = [pg].Id
		  ,@protectionGroupName = [pg].ProtectionGroupName
		  ,@backupServerName = [backupServers].ServerName
		  ,@userName = [backupservers].UserName
		  ,@password = CAST([backupservers].[Password] as nvarchar(128))
		FROM VMBackup.Subscriptions sub
			INNER JOIN VMBackup.ProtectionGroups pg on pg.SubscriptionId = sub.Id
			INNER JOIN VMBackup.BackupServers backupServers on pg.BackupServerId = backupServers.Id
		WHERE sub.SubscriptionId = @subscriptionId
	
	IF @protectionGroupId IS NULL
	BEGIN
        ROLLBACK TRANSACTION 
        RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
        RETURN;
    END	
	
	SELECT @protectingCount = Count(*)
		FROM VMBackup.VirtualMachines vm  WITH (ROWLOCK, UPDLOCK, HOLDLOCK)
			INNER JOIN ProtectionGroups pg ON vm.ProtectionGroupId = pg.Id
			INNER JOIN VMBackup.Subscriptions sub ON pg.SubscriptionId = sub.Id
		WHERE sub.SubscriptionId = @subscriptionId AND vm.ActionStateId = 1;
	
	IF @protectingCount > 0
	BEGIN
		ROLLBACK TRANSACTION
        RAISERROR (N'56002', 16, 1); -- (msg_str, severity, state)
        RETURN;
    END	
	IF EXISTS (SELECT [HyperVId] FROM [VMBackup].[VirtualMachines] WHERE [HyperVId] = @hyperVId)
	BEGIN
		IF EXISTS (SELECT [HyperVId] FROM [VMBackup].[VirtualMachines] WHERE [HyperVId] = @hyperVId AND [VirtualMachineStateId] = 2)
		BEGIN
			UPDATE [VMBackup].[VirtualMachines]
				SET [ProtectionGroupId] = @protectionGroupId
				   ,[ActionStateId] = 1
				   ,[ActionStateDate] = GETDATE()
				WHERE [HyperVId] = @hyperVId
		END
		ELSE
		BEGIN
			ROLLBACK TRANSACTION
			RAISERROR (N'56003', 16, 1); -- (msg_str, severity, state)
			RETURN;
		END
	END
	ELSE
	BEGIN
		INSERT INTO [VMBackup].[VirtualMachines]
				   ([VmmId]
				   ,[HyperVId]
				   ,[VmName]
				   ,[VirtualMachineStateId]
				   ,[RecoveryPointCount]
				   ,[LastRecoveryPoint]
				   ,[ProtectionGroupId]
				   ,[ActionStateId]
				   ,[ActionStateDate])
			 VALUES
				   (@vmmId
				   ,@hyperVId
				   ,@vmName
				   ,2
				   ,0
				   ,null
				   ,@protectionGroupId
				   ,1 --Protecting
				   ,GETDATE())
	END

	SELECT ProtectionGroupName = @protectionGroupName
		,BackupServerName = @backupServerName
		,BackupServerUserName = @userName
		,BackupServerPassword = @password

	COMMIT TRANSACTION
END				