CREATE PROCEDURE [VMBackup].[SelectBackupServerCollection]
WITH EXECUTE AS CALLER
AS
	SELECT serv.[ServerId], serv.[ServerName], serv.[UserName], serv.[Password], bss.[BackupServerStateName], grp.[GroupId], grp.[GroupName], ProtectionGroupCount=ISNULL(Count(pg.Id), 0), VirtualMachineCount=ISNULL(Count(vm.Id), 0)
		FROM [VMBackup].[BackupServers] serv 
			INNER JOIN [VMBackup].[ServerGroups] grp on grp.Id = serv.ServerGroupId
			INNER JOIN [VMBackup].[BackupServerState] bss on bss.Id = serv.[State]
			LEFT OUTER JOIN [VMBackup].[ProtectionGroups] pg on pg.BackupServerId = serv.Id
			LEFT OUTER JOIN [VMBackup].[VirtualMachines] vm on vm.ProtectionGroupId = pg.Id
		GROUP BY serv.[ServerId], serv.[ServerName], serv.[UserName], serv.[Password], bss.[BackupServerStateName], grp.[GroupId], grp.[GroupName]