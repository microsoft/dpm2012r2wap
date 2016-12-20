CREATE PROCEDURE [VMBackup].[InsertVmmServer]
 	 @stampId uniqueidentifier
    ,@serverName nvarchar(128)
    ,@userName nvarchar(128)
    ,@password varbinary(2048)

WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION


	IF EXISTS (SELECT StampId FROM [VMBackup].[VmmServers] WHERE [ServerName] = @serverName)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	INSERT INTO [VMBackup].[VmmServers]
			   ([StampId]
			   ,[ServerName]
			   ,[UserName]
			   ,[Password]
			   ,[State])
		 VALUES
			   (@stampId
			   ,@serverName
			   ,@userName
			   ,@password
			   ,1)

	SELECT [StampId], [ServerName], [UserName], [Password]
		FROM [VMBackup].[VmmServers] 
		WHERE StampId = @stampId

COMMIT TRANSACTION
END
