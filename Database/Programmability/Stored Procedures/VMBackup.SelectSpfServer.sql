CREATE PROCEDURE [VMBackup].[SelectSpfServer]
WITH EXECUTE AS CALLER
AS
	SELECT [AdminUrl], [UserName], [Password], [State]
		FROM [VMBackup].[SpfServer]