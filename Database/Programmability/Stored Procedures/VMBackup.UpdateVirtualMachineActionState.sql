CREATE PROCEDURE [VMBackup].[UpdateVirtualMachineActionState]
	@subscriptionId uniqueidentifier,
	@hyperVId uniqueidentifier,
	@actionStateId int
WITH EXECUTE AS CALLER
AS
BEGIN
	BEGIN TRANSACTION

	UPDATE [VMBackup].VirtualMachines
		SET ActionStateId = @actionStateId,
			ActionStateDate = GETDATE()
	WHERE HyperVId = @hyperVId;
	
	COMMIT TRANSACTION
END