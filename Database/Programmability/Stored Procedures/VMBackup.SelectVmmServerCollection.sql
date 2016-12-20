CREATE PROCEDURE [VMBackup].[SelectVmmServerCollection]

WITH EXECUTE AS CALLER
AS
	SELECT [StampId], [ServerName], [UserName], [Password]
		FROM [VMBackup].[VmmServers] 