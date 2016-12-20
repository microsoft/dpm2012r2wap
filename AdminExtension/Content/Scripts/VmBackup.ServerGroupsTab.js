/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        statusIcons = {
            Ready: {
                text: resources.StatusReady,
                iconName: "complete"
            },
            Active: {
                text: resources.StatusActive,
                iconName: "complete"
            },
            Default: {
                iconName: "spinner"
            }
        },
        grid,
        selectedRow;

    function onRowSelected(row) {
        if (row) {
            selectedRow = row;
            updateContextualCommands(row);
        }
    }

    function updateContextualCommands(row) {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.Contextual.set([
            new Exp.UI.Command("addNewGroup", resources.CommandAdd, Exp.UI.CommandIconDescriptor.getWellKnown("add"), true),
            new Exp.UI.Command("removeGroup", resources.CommandRemove, Exp.UI.CommandIconDescriptor.getWellKnown("delete"), true)]);
        Exp.UI.Commands.update();
    }

    // Command handlers
    function executeCommand(commandId, subCommandId, appData, context) {
        switch (commandId) {
            case "addNewGroup":
                global.VmBackupAdminExtension.QuickCreate.openCreateGroup();
                return true;
            case "removeGroup":
                deleteGroup();
                return true;
            default:
                return false;
        }
    }

    function deleteGroup() {
        var cachedRow = selectedRow,
            promise;

        waz.interaction.confirm(resources.AreYouSureYouWantToDeleteServerGroup.format(cachedRow.GroupName))
        .done(function () {
            promise = global.VmBackupAdminExtension.Controller.deleteGroup(cachedRow.GroupId, cachedRow.GroupName);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.DeletingServerGroup.format(cachedRow.GroupName),
                    successText: resources.SuccessfullyDeletedServerGroup.format(cachedRow.GroupName),
                    failureText: resources.FailedToDeleteServerGroup.format(cachedRow.GroupName)
                });
            promise.always(function () {
                global.VmBackupAdminExtension.Controller.forceRefreshGroupDataSet();
            });
        });
    }

    function loadTab(extension, renderArea, initData) {
        var dataSetInfo = global.VmBackupAdminExtension.Controller.getGroupDataSetInfo(),
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
                { name: resources.ColumnName, field: "GroupName", sortable: true },
                { name: resources.ColumnStatus, field: "Status", type: "status", displayStatus: global.waz.interaction.statusIconHelper(statusIcons), filterable: false, sortable: true },
                { name: resources.ColumnAzureBackupEnabled, field: "AzureBackupEnabled", formatter: global.VmBackupAdminExtension.Controller.trueFalseEnabledFormatter, filterable: false, sortable: true },
                { name: resources.ColumnBackupServerCount, field: "BackupServerCount", filterable: false, sortable: true },
                { name: resources.ColumnProtectionGroupCount, field: "ProtectionGroupCount", filterable: false, sortable: true },
                { name: resources.ColumnVirtualMachineCount, field: "VirtualMachineCount", filterable: false, sortable: true }
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
                    templateName: "serverGroupsTabEmpty",
                    arrowLinkSelector: ("{0} .NewServerGroupLink").format(container.selector),
                    arrowLinkAction: createGroup
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

    function createGroup() {
        global.VmBackupAdminExtension.QuickCreate.openCreateGroup();
    }

    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.ServerGroupsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp,
        executeCommand: executeCommand
    };
})(jQuery, this, this.Shell, this.Exp);
