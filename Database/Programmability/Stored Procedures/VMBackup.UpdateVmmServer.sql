CREATE PROCEDURE [VMBackup].[UpdateVmmServer]
 	 @stampId uniqueidentifier
    ,@userName nvarchar(128)
    ,@password varbinary(2048)

WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION


	IF NOT EXISTS (SELECT StampId FROM [VMBackup].[VmmServers] WHERE [StampId] = @stampId)
	BEGIN
		ROLLBACK TRANSACTION 
		RAISERROR (N'56001', 16, 1); -- (msg_str, severity, state)
		RETURN;
	END

	UPDATE [VMBackup].[VmmServers]
		SET [UserName] = @userName
			,[Password] = @password
			,[State] = 1
		WHERE StampId = @stampId

	SELECT [StampId], [ServerName], [UserName], [Password]
		FROM [VMBackup].[VmmServers] 
		WHERE StampId = @stampId

COMMIT TRANSACTION
END
