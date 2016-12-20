CREATE PROCEDURE [VMBackup].[CompleteVirtualMachineRecord]
	@subscriptionId uniqueidentifier,
	@hyperVId uniqueidentifier,
	@dataSourceId uniqueidentifier
WITH EXECUTE AS CALLER
AS
BEGIN
	BEGIN TRANSACTION

	UPDATE [VMBackup].[VirtualMachines]
		SET  DataSourceId = @dataSourceId
			,VirtualMachineStateId = 1
			,ActionStateId = 2
			,ActionStateDate = GETDATE()
		WHERE HyperVId = @hyperVId;
		
	COMMIT TRANSACTION
END 