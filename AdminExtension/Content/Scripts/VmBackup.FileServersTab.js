/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var grid,
        statusIcons = {
            Registered: {
                text: "Registered",
                iconName: "complete"
            },
            Default: {
                iconName: "spinner"
            }
        };

    function onRowSelected(row) {
    }

    function loadTab(extension, renderArea, initData) {
        var localDataSet = {
            url: global.VmBackupAdminExtension.Controller.adminFileServersUrl,
            dataSetName: global.VmBackupAdminExtension.Controller.adminFileServersUrl
        },
            columns = [
                { name: "Name", field: "FileServerName", sortable: false },
                { name: "Total Space", field: "TotalSpace", filterable: false, sortable: false },
                { name: "Free Space", field: "FreeSpace", filterable: false, sortable: false },
                { name: "Default Share Size", field: "DefaultSize", filterable: false, sortable: false },
            ];

        grid = renderArea.find(".grid-container")
            .wazObservableGrid("destroy")
            .wazObservableGrid({
                lastSelectedRow: null,
                data: localDataSet,
                keyField: "name",
                columns: columns,
                gridOptions: {
                    rowSelect: onRowSelected
                },
                emptyListOptions: {
                    extensionName: "VmBackupAdminExtension",
                    templateName: "fileServersTabEmpty"
                }
            });
    }

    function cleanUp() {
        if (grid) {
            grid.wazObservableGrid("destroy");
            grid = null;
        }
    }

    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.FileServersTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };
})(jQuery, this, this.Shell, this.Exp);