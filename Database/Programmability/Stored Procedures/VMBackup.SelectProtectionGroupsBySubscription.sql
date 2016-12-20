CREATE PROCEDURE [VMBackup].[SelectProtectionGroupsBySubscription]
	@subscriptionId uniqueidentifier
WITH EXECUTE AS CALLER
AS
BEGIN
	BEGIN TRANSACTION

	--Reset Completed / Failed Action States after a period of time
	UPDATE [VMBackup].VirtualMachines
		SET ActionStateId = 0
		   ,ActionStateDate = GETDATE()
	WHERE ActionStateId in (2, 4, 6, 8, 9, 10, 11) AND ActionStateDate < DATEADD(HOUR, -1, GETDATE());

	SELECT [pg].ProtectionGroupName
		  ,BackupServerName = [backupServers].ServerName
		  ,BackupServerUserName = [backupservers].UserName
		  ,BackupServerPassword = CAST([backupservers].[Password] as nvarchar(128))
		FROM VMBackup.Subscriptions sub
			INNER JOIN VMBackup.ProtectionGroups pg on pg.SubscriptionId = sub.Id
			INNER JOIN VMBackup.BackupServers backupServers on pg.BackupServerId = backupServers.Id
		WHERE sub.SubscriptionId = @subscriptionId
	
	COMMIT TRANSACTION
END