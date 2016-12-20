CREATE PROCEDURE [VMBackup].[InsertOrUpdateSpfServer]
	 @adminUrl nvarchar(128)
    ,@userName nvarchar(128)
	,@password varbinary(2048)
WITH EXECUTE AS CALLER
AS
BEGIN
BEGIN TRANSACTION

	IF EXISTS(SELECT AdminUrl FROM [VMBackup].[SpfServer] WITH(TABLOCKX))
	BEGIN
		UPDATE [VMBackup].[SpfServer]
			SET [AdminUrl] = @adminUrl
			   ,[UserName] = @userName
			   ,[Password] = @password
			   ,[State] = 1
	END
	ELSE
	BEGIN 
		INSERT INTO [VMBackup].[SpfServer]
				   ([AdminUrl]
				   ,[UserName]
				   ,[Password]
				   ,[State])
			 VALUES
				   (@adminUrl
				   ,@userName
				   ,@password
				   ,1)
	END

	SELECT [AdminUrl], [UserName], [Password], [State]
		FROM [VMBackup].[SpfServer]

COMMIT TRANSACTION
END
