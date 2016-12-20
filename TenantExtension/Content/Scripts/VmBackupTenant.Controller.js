/*globals window,jQuery,cdm,VmBackupTenantExtension,waz,Exp*/
(function ($, global, undefined) {
    "use strict";

    var baseUrl = "/VmBackupTenant",
        listFileSharesUrl = baseUrl + "/ListFileShares",
        listVirtualMachinesUrl = baseUrl + "/ListVirtualMachines",
        listProtectedMachinesUrl = baseUrl + "/ListProtectedVirtualMachines",
        addProtectionToVM = baseUrl + "/AddVirtualMachineToProtectionGroup",
        backupVM = baseUrl + "/BackupVirtualMachine",
        removeProtectionFromVM = baseUrl + "/RemoveProtectionFromVirtualMachine",
        restoreVM = baseUrl + "/RestoreVirtualMachine",
        resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.TenantExtension.Resource"),
        domainType = "VmBackup";

    function navigateToListView() {
        Shell.UI.Navigation.navigate("#Workspaces/{0}/vmbackup".format(VmBackupTenantExtension.name));
    }

    function getFileShares(subscriptionIds) {
        return makeAjaxCall(listFileSharesUrl, { subscriptionIds: subscriptionIds }).data;
    }

    function getVirtualMachines(subscriptionIds) {
        return Exp.Data.getData({
            ajaxData: {
                subscriptionIds: subscriptionId
            },
            url: listVirtualMachinesUrl,
            forceCacheRefresh: true
        });
        //return makeAjaxCall(listVirtualMachinesUrl, { subscriptionIds: subscriptionIds }).data;
    }

    function getAllVirtualMachinesDataSetInfo() {
        return {
            url: listVirtualMachinesUrl,
            dataSetName: listVirtualMachinesUrl,
            ajaxData: {
                subscriptionIds: SqlServerExtension.subscriptions
            },
        };
    }

    function getAllVirtualMachinesDataSet() {
        var dataSetInfo = getAllVirtualMachinesDataSetInfo();
        return Exp.Data.getLocalDataSet(dataSetInfo.dataSetName);
    }

    function setVirtualMachineDataFastPolling(isFast) {
        Exp.Data.setFastPolling(listVirtualMachinesUrl, isFast);
    }

    function getSubscriptionIds(subsFromService) {
        var subs = [];
        for (var i = 0; i < subsFromService.length; i++) {
            subs[i] = subsFromService[i].id;
        }

        return subs;
    }

    function forceRefreshAllVirtualMachinesDataSet() {
        var subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("vmbackup");

        Exp.Data.getData({
            url: listVirtualMachinesUrl,
            ajaxData: {
                subscriptionIds: getSubscriptionIds(subscriptionRegisteredToService)
            },
            forceCacheRefresh: true
        });
    }

    function protectVirtualMachine(subscriptionId, vmId, vmmId, vmName) {
        return waz.dataWrapper.create(getAllVirtualMachinesDataSet().data, "VmId")
           .update(
           // Updated row
           {
               VmId: vmId,
               Status: "NotProtected",
               ActionStatus: "Protecting"
           },
           // Promise
            makeAjaxCall(addProtectionToVM,
            {
                subscriptionId: subscriptionId,
                vmId: vmId,
                vmmId: vmmId,
                vmName: vmName
            }),
            resources.StatusSubmittingJob );
    }

    function removeProtectionFromVirtualMachine(virtualMachine) {
        return waz.dataWrapper.create(getAllVirtualMachinesDataSet().data, "VmId")
        .update(
        {
            VmId: virtualMachine.VmId,
            Status: "Protected",
            ActionStatus: "RemovingProtection"
        },
            makeAjaxCall(removeProtectionFromVM,
            {
                subscriptionId: virtualMachine.SubscriptionId,
                vmId: virtualMachine.VmId,
            }),
            resources.StatusSubmittingJob);
    }

    function backupVirtualMachine(virtualMachine) {
        return waz.dataWrapper.create(getAllVirtualMachinesDataSet().data, "VmId")
        .update(
        {
            VmId: virtualMachine.VmId,
            Status: "Protected",
            ActionStatus: "BackingUp"
        },
            makeAjaxCall(backupVM,
            {
                subscriptionId: virtualMachine.SubscriptionId,
                vmId: virtualMachine.VmId,
            }),
            resources.StatusSubmittingJob);
    }

    function restoreVirtualMachine(virtualMachine, recoverySourceId) {
        return waz.dataWrapper.create(getAllVirtualMachinesDataSet().data, "VmId")
        .update(
        {
            VmId: virtualMachine.VmId,
            Status: "Protected",
            ActionStatus: "Restoring"
        },
            makeAjaxCall(restoreVM,
            {
                subscriptionId: virtualMachine.SubscriptionId,
                vmId: virtualMachine.VmId,
                recoverySourceId: recoverySourceId
            }),
            resources.StatusSubmittingJob);
    }

    function makeAjaxCall(url, data) {
        return Shell.Net.ajaxPost({
            url: url,
            data: data
        });
    }

    function getLocalPlanDataSet() {
        return Exp.Data.getLocalDataSet(planListUrl);
    }

    function getfileSharesData(subscriptionId) {
        return Exp.Data.getData("fileshare{0}".format(subscriptionId), {
            ajaxData: {
                subscriptionIds: subscriptionId
            },
            url: listFileSharesUrl,
            forceCacheRefresh: true
        });
    }

    // TODO: Can we use the waz.dataWrapper in the sample?
    function createFileShare(subscriptionId, fileShareName, size, fileServerName) {
        return new waz.dataWrapper(Exp.Data.getLocalDataSet(listFileSharesUrl))
            .add(
            {
                Name: fileShareName,
                SubscriptionId: subscriptionId,
                Size: size,
                FileServerName: fileServerName
            },
            Shell.Net.ajaxPost({
                data: {
                    subscriptionId: subscriptionId,
                    Name: fileShareName,
                    Size: size,
                    FileServerName: fileServerName
                },
                url: baseUrl + "/CreateFileShare"
            })
        );
    }

    global.VmBackupTenantExtension = global.VmBackupTenantExtension || {};
    global.VmBackupTenantExtension.Controller = {
        createFileShare: createFileShare,
        listFileSharesUrl: listFileSharesUrl,
        listVirtualMachinesUrl: listVirtualMachinesUrl,
        listProtectedMachinesUrl: listProtectedMachinesUrl,
        forceRefreshAllVirtualMachinesDataSet: forceRefreshAllVirtualMachinesDataSet,
        setVirtualMachineDataFastPolling: setVirtualMachineDataFastPolling,
        getVirtualMachines: getVirtualMachines,
        protectVirtualMachine: protectVirtualMachine,
        removeProtectionFromVirtualMachine: removeProtectionFromVirtualMachine,
        backupVirtualMachine: backupVirtualMachine,
        restoreVirtualMachine: restoreVirtualMachine,
        getFileShares: getFileShares,
        getLocalPlanDataSet: getLocalPlanDataSet,
        getfileSharesData: getfileSharesData,
        navigateToListView: navigateToListView
    };
})(jQuery, this);
