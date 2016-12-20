/// <reference path="VmBackuptenant.controller.js" />
/*globals window,jQuery,cdm,Shell,VmBackupTenantExtension,waz,Exp*/
(function ($, global, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.TenantExtension.Resource"),
        vmResources = global.Resources.getResources("Microsoft.WindowsAzure.Server.VM.TenantExtension.ClientResources"),
        grid,
        selectedRow,
        statusIcons = {
            Protected: {
                text: resources.StatusProtected,
                iconName: "complete"
            },
            NotProtected: {
                text: resources.StatusNotProtected,
                iconName: "statusInfo"
            },
            Protecting: {
                text: resources.StatusProtecting,
                iconName: "spinner"
            },
            BackingUp: {
                text: resources.StatusBackingUp,
                iconName: "spinner"
            },
            RemovingProtection: {
                text: resources.StatusRemovingProtection,
                iconName: "spinner"
            },
            Default: {
                iconName: "spinner"
            }
        },
        savedWizardParams,
        wizard,
        vmRoleStatus = {
            Provisioned: "Provisioned",
            Provisioning: "Provisioning",
            Updating: "Updating",
            Failed: "Failed",
            Deprovisioned: "Deprovisioned"
        },
        vmStatus = {
            PoweringOff: "PoweringOff",
            Saving: "Saving",
            Restoring: "Restoring",
            Starting: "Starting",
            MergingDrives: "MergingDrives",
            Deleting: "Deleting",
            DiscardingDrives: "DiscardingDrives",
            Pausing: "Pausing",
            UnderCreation: "UnderCreation",
            UnderTemplateCreation: "UnderTemplateCreation",
            UnderUpdate: "UnderUpdate",
            UnderReplacement: "UnderReplacement",
            UnderMigration: "UnderMigration",
            CreatingCheckpoint: "CreatingCheckpoint",
            DeletingCheckpoint: "DeletingCheckpoint",
            RecoveringCheckpoint: "RecoveringCheckpoint",
            InitializingCheckpointOperation: "InitializingCheckpointOperation",
            FinishingCheckpointOperation: "FinishingCheckpointOperation",
            Stopping: "Stopping",

            Saved: "Saved",
            Stored: "Stored",
            DiscardSavedState: "DiscardSavedState",

            CreationFailed: "CreationFailed",
            TemplateCreationFailed: "TemplateCreationFailed",
            CustomizationFailed: "CustomizationFailed",
            UpdateFailed: "UpdateFailed",
            ReplacementFailed: "ReplacementFailed",
            MigrationFailed: "MigrationFailed",
            CheckpointFailed: "CheckpointFailed",
            P2VCreationFailed: "P2VCreationFailed",
            V2VCreationFailed: "V2VCreationFailed",

            Missing: "Missing",
            HostNotResponding: "HostNotResponding",
            Unsupported: "Unsupported",
            UnsupportedSharedFiles: "UnsupportedSharedFiles",
            UnsupportedCluster: "UnsupportedCluster",
            IncompleteVMConfig: "IncompleteVMConfig",

            Paused: "Paused",
            PowerOff: "PowerOff",

            Running: "Running",
            Stopped: "Stopped",
            NotReady: "NotReady",
            Error: "Error"
        }

    function dateFormatter(value) {
        try {
            if (value) {
                return $.datepicker.formatDate("m/d/yy", value);
            }
        }
        catch (err) { }  // Display "-" if the date is in an unrecoginzed format.

        return "-";
    }

    function actionStatusFormatter(value) {
        if (value === null || value === undefined) {
            return value;
        } else {
            switch (value) {
                case "None":
                    return "";
                case "Protecting":
                    return resources.StatusProtecting;
                case "ProtectComplete":
                    return resources.StatusProtectComplete;
                case "ProtectionFailed":
                    return resources.StatusProtectionFailed;
                case "BackingUp":
                    return resources.StatusBackingUp;
                case "BackupComplete":
                    return resources.StatusBackupComplete;
                case "BackupFailed":
                    return resources.StatusBackupFailed;
                case "Restoring":
                    return resources.StatusRestoring;
                case "RestoreComplete":
                    return resources.StatusRestoreComplete;
                case "RestoreFailed":
                    return resources.StatusRestoreFailed;
                case "RemovingProtection":
                    return resources.StatusRemovingProtection;
                case "RemoveProtectionFailed":
                    return resources.StatusRemoveProtectionFailed;
                default:
                    return value.ActionStatus;
            }
        }
    }

    function vmStatusFormatter(value) {
        return {
            text: getVmDisplayStatus(value.VmStatus),
            iconName: getVmDisplayStatusIcon(value.VmStatus)
        };
    }

    function getVmDisplayStatus(status) {
        switch (status) {
            case vmRoleStatus.Provisioned:
                return vmResources.statusProvisioned;
            case vmRoleStatus.Provisioning:
                return vmResources.statusProvisioning;
            case vmRoleStatus.Updating:
            case vmStatus.UnderUpdate:
                return vmResources.statusUpdating;
            case vmRoleStatus.Failed:
                return vmResources.statusFailed;
            case vmRoleStatus.Deprovisioned:
                return vmResources.statusDeprovisioned;
            case vmStatus.PoweringOff:
                return vmResources.statusPoweringOff;
            case vmStatus.Restoring:
                return vmResources.statusRestoring;
            case vmStatus.Starting:
                return vmResources.statusStarting;
            case vmStatus.Deleting:
                return vmResources.statusDeleting;
            case vmStatus.Pausing:
                return vmResources.statusPausing;
            case vmStatus.CreationFailed:
                return vmResources.statusCreationFailed;
            case vmStatus.Missing:
                return vmResources.statusMissing;
            case vmStatus.HostNotResponding:
                return vmResources.statusHostNotResponding;
            case vmStatus.Paused:
                return vmResources.statusPaused;
            case vmStatus.PowerOff:
                return vmResources.statusStopped;
            case vmStatus.Running:
                return vmResources.statusRunning;
            case vmStatus.Stopped:
                return vmResources.statusStopped;
            case vmStatus.Stopping:
                return vmResources.statusStopping;
            case vmStatus.NotReady:
                return vmResources.statusNotReady;
            case vmStatus.Error:
                return vmResources.statusError;
            default:
                return "";
        }
    }

    function getVmDisplayStatusIcon(status) {
        switch (status) {
            case vmStatus.PoweringOff:
            case vmStatus.Saving:
            case vmStatus.Restoring:
            case vmStatus.Starting:
            case vmStatus.MergingDrives:
            case vmStatus.Deleting:
            case vmStatus.DiscardingDrives:
            case vmStatus.Pausing:
            case vmStatus.UnderCreation:
            case vmStatus.UnderTemplateCreation:
            case vmStatus.UnderUpdate:
            case vmStatus.UnderReplacement:
            case vmStatus.UnderMigration:
            case vmStatus.CreatingCheckpoint:
            case vmStatus.DeletingCheckpoint:
            case vmStatus.RecoveringCheckpoint:
            case vmStatus.InitializingCheckpointOperation:
            case vmStatus.FinishingCheckpointOperation:
            case vmStatus.NotReady:
            case vmStatus.Stopping:
                // VMRole
            case vmRoleStatus.Provisioning:
            case vmRoleStatus.Updating:
                return "spinner";

            case vmStatus.Saved:
            case vmStatus.Stored:
            case vmStatus.DiscardSavedState:
                return "complete";

            case vmStatus.CreationFailed:
            case vmStatus.TemplateCreationFailed:
            case vmStatus.CustomizationFailed:
            case vmStatus.UpdateFailed:
            case vmStatus.ReplacementFailed:
            case vmStatus.MigrationFailed:
            case vmStatus.CheckpointFailed:
            case vmStatus.P2VCreationFailed:
            case vmStatus.V2VCreationFailed:
            case vmStatus.Error:
                // VMRole
            case vmRoleStatus.Failed:
                return "bang";

            case vmStatus.Missing:
            case vmStatus.HostNotResponding:
            case vmStatus.UnsupportedSharedFiles:
            case vmStatus.UnsupportedCluster:
            case vmStatus.Unsupported:
            case vmStatus.IncompleteVMConfig:
                // VMRole
            case vmRoleStatus.Deprovisioned:
                return "warning";

            case vmStatus.Paused:
                return "paused";

            case vmStatus.PowerOff:
            case vmStatus.Stopped:
                return "stopped";

            default:
                return "";
        }
    }

    function onRowSelected(row) {
        if (row) {
            selectedRow = row;
            updateContextualCommands(row);
        }
    }

    function updateContextualCommands(row) {
        var isProtected = false,
            canProtect = true,
            canBackup = false,
            canRestore = false,
            canRemoveProtection = false;

        if (row.Status == "Protected") {
            isProtected = true;
            canProtect = false;
            canRemoveProtection = true;
        }

        if (isProtected & row.RecoveryPoints > 0) {
            canBackup = true;
            canRestore = true;
        }

        switch (row.ActionStatus) {
            case "Protecting":
                canProtect = false;
            case "BackingUp":
            case "Restoring":
            case "RemovingProtection":
                canRemoveProtection = false;
                canBackup = false;
                canRestore = false;
        }

        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.Contextual.set([
            new Exp.UI.Command("protectVM", resources.CommandProtectVM, Exp.UI.CommandIconDescriptor.getWellKnown("link"), canProtect),
            new Exp.UI.Command("backupVM", resources.CommandBackupVM, Exp.UI.CommandIconDescriptor.getWellKnown("downloadpackage"), canBackup),
            new Exp.UI.Command("restoreVM", resources.CommandRestoreVM, Exp.UI.CommandIconDescriptor.getWellKnown("uploadpackage"), canRestore),
            new Exp.UI.Command("removeProtection", resources.CommandRemoveProtection, Exp.UI.CommandIconDescriptor.getWellKnown("delete"), canRemoveProtection)]);
        Exp.UI.Commands.update();
    }

    // Command handlers
    function executeCommand(commandId, subCommandId, appData, context) {
        switch (commandId) {
            case "protectVM":
                protectVirtualMachine();
                return true;
            case "backupVM":
                backupVirtualMachine();
                return true;
            case "restoreVM":
                restoreVirtualMachine();
                return true;
            case "removeProtection":
                removeProtection();
                return true;
            default:
                return false;
        }
    }

    function protectVirtualMachine() {
        var cachedRow = selectedRow,
            promise;

        waz.interaction.confirm(resources.AreYouSureYouWantToProtect.format(cachedRow.Name))
        .done(function () {
            promise = global.VmBackupTenantExtension.Controller.protectVirtualMachine(cachedRow.SubscriptionId, cachedRow.VmId, cachedRow.Id, cachedRow.Name);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.ProtectingVirtualMachine.format(cachedRow.Name),
                    successText: resources.SuccessfullySubmittedProtectingVirtualMachine.format(cachedRow.Name),
                    failureText: resources.FailedToSubmitProtectingVirtualMachine.format(cachedRow.Name)
                });
            promise.always(function () {
                global.VmBackupTenantExtension.Controller.forceRefreshAllVirtualMachinesDataSet();
            });
        });
    }

    function backupVirtualMachine() {
        var cachedRow = selectedRow,
            promise;

        waz.interaction.confirm(resources.AreYouSureYouWantToBackupVirtualMachine.format(cachedRow.Name))
        .done(function () {
            promise = global.VmBackupTenantExtension.Controller.backupVirtualMachine(cachedRow);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.BackingUpVirtualMachine.format(cachedRow.Name),
                    successText: resources.SuccessfullySubmittedBackingUpVirtualMachine.format(cachedRow.Name),
                    failureText: resources.FailedToSubmitBackupVirtualMachine.format(cachedRow.Name)
                });
            promise.always(function () {
                global.VmBackupTenantExtension.Controller.forceRefreshAllVirtualMachinesDataSet();
            });
        });
    }

    function removeProtection() {
        var cachedRow = selectedRow,
            promise;

        waz.interaction.confirm(resources.AreYouSureYouWantToRemoveProtection.format(cachedRow.Name))
        .done(function () {
            promise = global.VmBackupTenantExtension.Controller.removeProtectionFromVirtualMachine(cachedRow);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.RemovingProtectionFromVirtualMachine.format(cachedRow.Name),
                    successText: resources.SuccessfullySubmittedRemovingProtectionFromVirtualMachine.format(cachedRow.Name),
                    failureText: resources.FailedToSubmitRemoveProtectionFromVirtualMachine.format(cachedRow.Name)
                });
            promise.always(function () {
                global.VmBackupTenantExtension.Controller.forceRefreshAllVirtualMachinesDataSet();
            });
        });
    }

    function restoreVirtualMachine() {
        var cachedRow = selectedRow,
            promise;

        waz.interaction.confirm(resources.AreYouSureYouWantToRestoreVirtualMachine.format(cachedRow.Name))
        .done(function () {
            Shell.UI.Spinner.show();
            promise = Shell.Net.ajaxPost({
                url: "VmBackupTenant/ListRecoveryPoints",
                data: {
                    subscriptionId: cachedRow.SubscriptionId,
                    vmId: cachedRow.VmId
                }

            });
            waz.interaction.showProgress(
            promise,
            {
                initialText: resources.LoadingRestorePoints.format(cachedRow.Name),
                successText: resources.SuccessfullyLoadedRestorePoints.format(cachedRow.Name),
                failureText: resources.FailureLoadingRestorePoints.format(cachedRow.Name)
            });

            promise.done(function (data, textStatus, jqXHR) {
                    var commandParameter;
                    Shell.UI.Spinner.hide();

                    commandParameter = {
                        header: resources.SelectRestorePointHeader,
                        title: resources.LetsSelectResourcePointTitle.format(cachedRow.Name),
                        subtitle: resources.SelectResourcePointSubtitle,
                        virtualMachine: cachedRow,
                        restorePoints: data
                    };
                    showSelectRestorePointWizard(commandParameter);
                });
        });
    }

    function onSelectRestorePointActivated() {
        Shell.UI.Validation.setValidationContainer("#selectRestorePointWizard");
    }

    function onSelectRestorePointComplete() {
        var isValid, promise, recoverySourceId;
        isValid = validateSelectRestorePointWizStep();

        //if (isValid) {
            recoverySourceId = $("#recoverySourceId").val();
            promise = global.VmBackupTenantExtension.Controller.restoreVirtualMachine(selectedRow, recoverySourceId);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.RestoringVirtualMachine.format(selectedRow.Name),
                    successText: resources.SuccessfullySubmittedRestoringVirtualMachine.format(selectedRow.Name),
                    failureText: resources.FailedToSubmitRestoringVirtualMachine.format(selectedRow.Name)
                });
            promise.always(function () {
                global.VmBackupTenantExtension.Controller.forceRefreshAllVirtualMachinesDataSet();
            });
        //}
    }

    function validateSelectRestorePointWizStep() {
        return Shell.UI.Validation.validateContainer("#selectRestorePointWizard");
    }


    function showSelectRestorePointWizard(wizardParams) {
        var steps;

        savedWizardParams = wizardParams;

        steps = [
                {
                    template: "selectRestorePointTemplate",
                    onStepActivate: onSelectRestorePointActivated,
                    onNextStep: onSelectRestorePointComplete,
                    data: savedWizardParams
                }
        ];

        wizard = cdm.stepWizard({
            extension: VmBackupTenantExtension.name,
            steps: steps
        },
        {   //Options
            size: "medium"
        });

    }

    function getSubscriptionIds(subsFromService) {
        var subs = [];
        for (var i = 0; i < subsFromService.length; i++) {
            subs[i] = subsFromService[i].id;
        }

        return subs;
    }


    // Public
    function loadTab(extension, renderArea, initData) {
        var subs = Exp.Rdfe.getSubscriptionList(),
           subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("vmbackup"),
        localDataSet = {            
            dataSetName: global.VmBackupTenantExtension.Controller.listVirtualMachinesUrl,
            ajaxData: { 
                subscriptionIds: getSubscriptionIds(subscriptionRegisteredToService),
                },
            url: global.VmBackupTenantExtension.Controller.listVirtualMachinesUrl
        },
        options = $.extend({ forceCacheRefresh: true }, localDataSet);  // Start with cached data but initiate an async refresh.
        $("#tabcontainer .itemtitle").text(resources.TabContainerTitleOverride);
        Shell.UI.Spinner.show();
        Exp.Data.getData(localDataSet, options)
            .then(
            function (dataSetName, value) {
                renderGrid(renderArea, value.data);
                Shell.UI.Spinner.hide();
            },
            function (url, jqXHR, error) {
                Shell.UI.Spinner.hide();
                global.waz.interaction.notifyError("Error", jqXHR, error);
            });

    }
    
    function renderGrid(container, data) {
        var columns = [
                { name: resources.ColumnVMName, field: "Name", sortable: true },
                { name: resources.ColumnProtectionStatus, field: "Status", type: "status", displayStatus: global.waz.interaction.statusIconHelper(statusIcons), filterable: false, sortable: true },
                { name: "Latest Action", field: "ActionStatus", formatter: actionStatusFormatter, filterable: false, sortable: true },
                { name: resources.ColumnRecoveryPoints, field: "RecoveryPoints", filterable: false, sortable: true },
                { name: resources.ColumnBackupPolicy, field: "BackupPolicy", filterable: false, sortable: true },
                { name: resources.ColumnLastRecoveryPoint, field: "LastRecoveryPoint", filterable: false, sortable: true },
                { name: resources.ColumnSubscription, field: "SubscriptionId", type: "subscription" }

        ];

        grid = container.find(".gridContainer")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: data,
                keyField: "Id",
                columns: columns,
                gridOptions: {
                    rowSelect: onRowSelected
                },
                emptyListOptions: {
                    extensionName: "VmBackupTenantExtension",
                    templateName: "VmBackupTabEmpty"
                }
            });
        global.VmBackupTenantExtension.Controller.setVirtualMachineDataFastPolling(true);
    }

    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    window.VmBackupTenantExtension = window.VmBackupTenantExtension || {};
    window.VmBackupTenantExtension.VmBackupTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        executeCommand: executeCommand,
        statusIcons: statusIcons
    };
})(jQuery, this);
