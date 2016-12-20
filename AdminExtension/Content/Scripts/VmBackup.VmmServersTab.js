/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        statusIcons = {
            Registered: {
                text: resources.StatusReady,
                iconName: "complete"
            },
            NotRegistered: {
                text: resources.StatusNotRegistered,
                iconName: "bang"
            },
            Default: {
                iconName: "spinner"
            }
        }, grid,
        selectedRow,
        wizard,
        savedWizardParams;

    function onRowSelected(row) {
        if (row) {
            selectedRow = row;
            updateContextualCommands(row);
        }
    }

    function showRegisterVmmServerWizard(wizardParams) {
        var wizSteps;

        savedWizardParams = wizardParams;

        wizSteps = [
                {
                    template: "registerVmmServerDialog",
                    onStepActivate: onRegisterVmmServerWizPageActivated,
                    onNextStep: onRegisterVmmServerComplete,
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

    function onRegisterVmmServerWizPageActivated() {
        initializeDialog();
        Shell.UI.Validation.setValidationContainer("#aux-editVmmServerForm");
    }

    function onRegisterVmmServerComplete() {
        var isValid, promise, stampId, serverName, userName, password;
        isValid = validateRegisterVmmServerWizStep();

        if (isValid) {
            stampId = savedWizardParams.stampId;
            serverName = $("#serverName").val();
            userName = $("#userName").val();
            password = $("#password").val();
            if (savedWizardParams.userName) {
                promise = global.VmBackupAdminExtension.Controller.updateVmmServer(stampId, serverName, userName, password);
                waz.interaction.showProgress(
                    promise,
                    {
                        initialText: resources.UpdatingVmmServer.format(serverName),
                        successText: resources.SuccessfullyUpdatedVmmServer.format(serverName),
                        failureText: resources.FailedToUpdateVmmServer.format(serverName)
                    });
            }
            else {
                promise = global.VmBackupAdminExtension.Controller.createVmmServer(stampId, serverName, userName, password);
                waz.interaction.showProgress(
                    promise,
                    {
                        initialText: resources.RegisteringVmmServer.format(serverName),
                        successText: resources.SuccessfullyRegisteredVmmServer.format(serverName),
                        failureText: resources.FailedToRegisterVmmServer.format(serverName)
                    });
            }
            promise.always(function () {
                global.VmBackupAdminExtension.Controller.forceRefreshVmmServerDataSet();
            });
        }
    }

    function validateRegisterVmmServerWizStep() {
        return Shell.UI.Validation.validateContainer("#aux-editVmmServerForm");
    }

    function updateContextualCommands(row) {
        var canConnect, canRepair, canDelete;
        
        switch (row.Status) {
            case "Registered":
                canConnect = false;
                canRepair = true;
                canDelete = true;
                break;
            case "NotRegistered":
                canConnect = true;
                canRepair = false;
                canDelete = false;
                break;
            default:
                canConnect = false;
                canRepair = false;
                canDelete = false;
                break;
        }

        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.Contextual.set([
            new Exp.UI.Command("connectVmm", resources.CommandConnect, Exp.UI.CommandIconDescriptor.getWellKnown("connect"), canConnect),
            new Exp.UI.Command("repairVmm", resources.CommandRepair, Exp.UI.CommandIconDescriptor.getWellKnown("repair"), canRepair),
            new Exp.UI.Command("removeVmm", resources.CommandRemove, Exp.UI.CommandIconDescriptor.getWellKnown("delete"), canDelete)]);
        Exp.UI.Commands.update();
    }

    // Command handlers
    function executeCommand(commandId, subCommandId, appData, context) {
        var wizardParams;
        switch (commandId) {
            case "connectVmm":
                wizardParams = {
                    stampId: selectedRow.StampId,
                    serverName: selectedRow.ServerName,
                    userName: null
                };
                showRegisterVmmServerWizard(wizardParams);
                return true;
            case "repairVmm":
                wizardParams = {
                    stampId: selectedRow.StampId,
                    serverName: selectedRow.ServerName,
                    userName: selectedRow.UserName
                };
                showRegisterVmmServerWizard(wizardParams);
                return true;
            case "removeVmm":
                //restoreVirtualMachine();
                return true;
            default:
                return false;
        }
    }

    function loadTab(extension, renderArea, initData) {
        var dataSetInfo = global.VmBackupAdminExtension.Controller.getVmmServersDataSetInfo(),
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
                { name: resources.ColumnStatus, field: "Status", type: "status", displayStatus: global.waz.interaction.statusIconHelper(statusIcons), filterable: false, sortable: true }
                //{ name: "Latest Action", field: "ActionStatus", formatter: actionStatusFormatter, filterable: false, sortable: true },

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
                    templateName: "vmmServersTabEmpty",
                    arrowLinkSelector: ("{0} .NewVmmServerLink").format(container.selector),
                    arrowLinkAction: createVmmServer
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

    function createVmmServer() {
        global.VmBackupAdminExtension.QuickCreate.openCreateVmmServer();
    }

    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.VmmServersTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        executeCommand: executeCommand
    };
})(jQuery, this, this.Shell, this.Exp);
