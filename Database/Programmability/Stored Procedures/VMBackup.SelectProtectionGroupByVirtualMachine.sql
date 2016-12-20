CREATE PROCEDURE [VMBackup].[SelectProtectionGroupByVirtualMachine]
	@subscriptionId uniqueidentifier,
	@hyperVId uniqueidentifier
WITH EXECUTE AS CALLER
AS
	SELECT [pg].ProtectionGroupName
		  ,BackupServerName = [backupServers].ServerName
		  ,BackupServerUserName = [backupservers].UserName
		  ,BackupServerPassword = CAST([backupservers].[Password] as nvarchar(128))
		FROM VMBackup.Subscriptions sub
			INNER JOIN VMBackup.ProtectionGroups pg on pg.SubscriptionId = sub.Id
			INNER JOIN VMBackup.BackupServers backupServers on pg.BackupServerId = backupServers.Id
			INNER JOIN VMBackup.VirtualMachines vm on vm.ProtectionGroupId = pg.Id
		WHERE sub.SubscriptionId = @subscriptionId AND vm.HyperVId = @hyperVId
