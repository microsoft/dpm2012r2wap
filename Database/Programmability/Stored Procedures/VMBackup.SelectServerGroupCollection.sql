CREATE PROCEDURE [VMBackup].[SelectServerGroupCollection]
WITH EXECUTE AS CALLER
AS
	SELECT GroupId, GroupName, AzureBackupEnabled, BackupServerCount=ISNULL(Count(serv.Id),0) , ProtectionGroupCount=ISNULL(Count(pg.Id), 0), VirtualMachineCount=ISNULL(Count(vm.Id), 0)
		FROM [VMBackup].[ServerGroups] grp
			LEFT OUTER JOIN [VMBackup].[BackupServers] serv on serv.ServerGroupId = grp.Id
			LEFT OUTER JOIN [VMBackup].[ProtectionGroups] pg on pg.BackupServerId = serv.Id
			LEFT OUTER JOIN [VMBackup].[VirtualMachines] vm on vm.ProtectionGroupId = pg.Id
		GROUP BY GroupId, GroupName, AzureBackupEnabled