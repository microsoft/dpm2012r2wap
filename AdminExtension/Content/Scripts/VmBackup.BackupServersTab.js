/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        grid,
        selectedRow,
        wizard,
        savedWizardParams;

    function onRowSelected(row) {
        if (row) {
            selectedRow = row;
            updateContextualCommands(row);
        }
    }

    function showUpdateBackupServerWizard(wizardParams) {
        var wizSteps;

        savedWizardParams = wizardParams;

        wizSteps = [
                {
                    template: "updateBackupServerDialog",
                    onStepActivate: onUpdateBackupServerWizPageActivated,
                    onNextStep: onUpdateBackupServerComplete,
                    data: savedWizardParams
                }
        ];

        wizard = cdm.stepWizard({
            extension: global.VmBackupAdminExtension.name,
            steps: wizSteps
        },
        {   //Options
            size: "medium"
        });

    }

    function initializeDialog() {
        if (savedWizardParams) {
            $("#serverName").val(savedWizardParams.serverName);
            if (savedWizardParams.userName) {
                $("#userName").val(savedWizardParams.userName);
            }
        }
    }

    function onUpdateBackupServerWizPageActivated() {
        initializeDialog();
        Shell.UI.Validation.setValidationContainer("#aux-editBackupServerForm");
    }

    function onUpdateBackupServerComplete() {
        var isValid, promise, serverId, serverName, userName, password;
        isValid = validateUpdateBackupServerWizStep();

        if (isValid) {
            serverId = savedWizardParams.serverId;
            serverName = $("#serverName").val();
            userName = $("#userName").val();
            password = $("#password").val();

            promise = global.VmBackupAdminExtension.Controller.updateBackupServer(serverId, serverName, userName, password);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.UpdatingBackupServer.format(serverName),
                    successText: resources.SuccessfullyUpdatedBackupServer.format(serverName),
                    failureText: resources.FailedToUpdateBackupServer.format(serverName)
                });
            promise.always(function () {
                global.VmBackupAdminExtension.Controller.forceRefreshBackupServerDataSet();
            });
        }
    }

    function validateUpdateBackupServerWizStep() {
        return Shell.UI.Validation.validateContainer("#aux-editBackupServerForm");
    }

    function updateContextualCommands(row) {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.Contextual.set([
            new Exp.UI.Command("newBackupServer", resources.CommandAdd, Exp.UI.CommandIconDescriptor.getWellKnown("add"), true),
            new Exp.UI.Command("repairBackupServer", resources.CommandRepair, Exp.UI.CommandIconDescriptor.getWellKnown("repair"), true),
            new Exp.UI.Command("removeBackupServer", resources.CommandRemove, Exp.UI.CommandIconDescriptor.getWellKnown("delete"), true)]);
        Exp.UI.Commands.update();
    }

    // Command handlers
    function executeCommand(commandId, subCommandId, appData, context) {
        var wizardParams;
        switch (commandId) {
            case "newBackupServer":
                global.VmBackupAdminExtension.QuickCreate.openCreateBackupServer();
                return true;
            case "repairBackupServer":
                wizardParams = {
                    serverId: selectedRow.ServerId,
                    serverName: selectedRow.ServerName,
                    userName: selectedRow.UserName
                };
                showUpdateBackupServerWizard(wizardParams);
                return true;
            case "removeBackupServer":
                removeBackupServer();
                return true;
            default:
                return false;
        }
    }

    function removeBackupServer() {
        var cachedRow = selectedRow,
            promise;

        waz.interaction.confirm(resources.AreYouSureYouWantToDeleteBackupServer.format(cachedRow.ServerName))
        .done(function () {
            promise = global.VmBackupAdminExtension.Controller.deleteBackupServer(cachedRow.ServerId);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.DeletingBackupServer.format(cachedRow.ServerName),
                    successText: resources.SuccessfullyDeletedBackupServer.format(cachedRow.ServerName),
                    failureText: resources.FailedToDeleteBackupServer.format(cachedRow.ServerName)
                });
            promise.always(function () {
                global.VmBackupAdminExtension.Controller.forceRefreshBackupServerDataSet();
            });
        });
    }

    function loadTab(extension, renderArea, initData) {
        var dataSetInfo = global.VmBackupAdminExtension.Controller.getBackupServersDataSetInfo(),
            options = $.extend({ forceCacheRefresh: true }, dataSetInfo);  // Start with cached data but initiate an async refresh.

        $("#tabcontainer .itemtitle").text(resources.TabContainerTitleOverride);
        Shell.UI.Spinner.show();

        Exp.Data.getData(dataSetInfo.dataSetName, options)
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
                { name: resources.ColumnName, field: "ServerName", sortable: true },
                { name: resources.ColumnStatus, field: "Status", type: "status", displayStatus: global.waz.interaction.statusIconHelper(global.VmBackupAdminExtension.Controller.statusIcons), filterable: false, sortable: true },
                //{ name: "Latest Action", field: "ActionStatus", formatter: actionStatusFormatter, filterable: false, sortable: true },
                { name: resources.ColumnProtectionGroupCount, field: "ProtectionGroupCount", filterable: false, sortable: true },
                { name: resources.ColumnVirtualMachineCount, field: "VirtualMachineCount", filterable: false, sortable: true },
                { name: resources.ColumnGroupName, field: "GroupName", filterable: false, sortable: true }

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
                    extensionName: "VmBackupAdminExtension",
                    templateName: "backupServersTabEmpty",
                    arrowLinkSelector: ("{0} .NewBackupServerLink").format(container.selector),
                    arrowLinkAction: createBackupServer
                }
            });
       // global.VmBackupTenantExtension.Controller.setVirtualMachineDataFastPolling(true);
    }

    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    function createBackupServer() {
        global.VmBackupAdminExtension.QuickCreate.openCreateBackupServer();
    }

    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.BackupServersTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        executeCommand: executeCommand
    };
})(jQuery, this, this.Shell, this.Exp);
