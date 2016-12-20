CREATE PROCEDURE [VMBackup].[DeleteVirtualMachineRecord]
    @subscriptionId uniqueidentifier,
    @hyperVId uniqueidentifier
WITH EXECUTE AS CALLER
AS
BEGIN TRAN
	DELETE FROM [VMBackup].[VirtualMachines]
	FROM [VMBackup].[VirtualMachines] vm
		INNER JOIN [VMBackup].[ProtectionGroups] pg ON vm.ProtectionGroupId = pg.Id
		INNER JOIN [VMBackup].[Subscriptions] sub ON pg.SubscriptionId = sub.Id
	WHERE vm.HyperVId = @hyperVId and sub.SubscriptionId = @subscriptionId
COMMIT TRAN