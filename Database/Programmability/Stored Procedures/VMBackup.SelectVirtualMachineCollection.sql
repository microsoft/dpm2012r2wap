CREATE PROCEDURE [VMBackup].[SelectVirtualMachineCollection]
	@subscriptionId uniqueidentifier
WITH EXECUTE AS CALLER
AS
BEGIN
    SET NOCOUNT ON;

	SELECT [DataSourceId]
		  ,[VmmId]
		  ,[HyperVId]
		  ,[VmName]
		  ,vmState.[VirtualMachineStateName]
		  ,actionState.[ActionStateName]
		  ,[RecoveryPointCount]
		  ,[LastRecoveryPoint]
		  ,pg.[ProtectionGroupId]
		  ,sub.[SubscriptionId]
		  ,sub.[AccountAdminId]
	FROM [VMBackup].[VirtualMachines] vm
		INNER JOIN [VMBackup].[ProtectionGroups] pg ON vm.ProtectionGroupId = pg.Id
		INNER JOIN [VMBackup].[Subscriptions] sub ON pg.SubscriptionId = sub.Id
		INNER JOIN [VMBackup].[VirtualMachineState] vmState ON vm.VirtualMachineStateId = vmState.Id
		INNER JOIN [VMBackup].[ActionState] actionState on vm.ActionStateId = actionState.Id
	WHERE sub.SubscriptionId = @subscriptionId
	

END