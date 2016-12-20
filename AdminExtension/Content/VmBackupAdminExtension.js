/*globals window,jQuery,Shell,Exp,waz*/

(function (global, $, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        vmBackupExtensionActivationInit,
        navigation;   

    function clearCommandBar() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.update();
    }

    function onApplicationStart() {
        Exp.UserSettings.getGlobalUserSetting("Admin-skipQuickStart").then(function (results) {
            var setting = results ? results[0] : null;
            if (setting && setting.Value) {
                global.VmBackupAdminExtension.settings.skipQuickStart = JSON.parse(setting.Value);
            }
        });
                
        global.VmBackupAdminExtension.settings.skipQuickStart = false;
    }

    function executeCommand(commandId, subCommandId, appData, context) {
        if (global.VmBackupAdminExtension.BackupServersTab.executeCommand(commandId, subCommandId, appData, context)) {
            return true;
        }
        else if (global.VmBackupAdminExtension.ServerGroupsTab.executeCommand(commandId, subCommandId, appData, context)) {
            return true;
        }
        else if (global.VmBackupAdminExtension.VmmServersTab.executeCommand(commandId, subCommandId, appData, context)) {
            return true;
        }
        else {
            return false;
        }
    }

    function loadQuickStart(extension, renderArea, renderData) {
        clearCommandBar();
        global.VmBackupAdminExtension.QuickStartTab.loadTab(renderData, renderArea);
    }

    function loadBackupServersTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.BackupServersTab.loadTab(renderData, renderArea);
    }

    function loadServerGroupsTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.ServerGroupsTab.loadTab(renderData, renderArea);
    }

    function loadVmmServersTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.VmmServersTab.loadTab(renderData, renderArea);
    }

    function loadFileServersTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.FileServersTab.loadTab(renderData, renderArea);
    }

    function loadProductsTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.ProductsTab.loadTab(renderData, renderArea);
    }

    function loadSettingsTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.SettingsTab.loadTab(renderData, renderArea);
    }

    function loadControlsTab(extension, renderArea, renderData) {
        global.VmBackupAdminExtension.ControlsTab.loadTab(renderData, renderArea);
    }

    global.vmBackupExtension = global.VmBackupAdminExtension || {};

    navigation = {
        tabs: [
                {
                    id: "quickStart",
                    displayName: "",
                    template: "quickStartTab",
                    activated: loadQuickStart,
                    icon: Exp.Navigation.quickStartImageDescriptor
                },
                {
                    id: "backupServers",
                    displayName: resources.BackupServersTabName,
                    template: "backupServersTab",
                    activated: loadBackupServersTab
                },
                 {
                     id: "serverGroups",
                     displayName: resources.ServerGroupsTabName,
                     template: "serverGroupsTab",
                     activated: loadServerGroupsTab
                 },
                {
                    id: "vmmServers",
                    displayName: resources.VmmServersTabName,
                    template: "vmmServersTab",
                    activated: loadVmmServersTab
                },
                //{
                //    id: "controls",
                //    displayName: "controls",
                //    template: "controlsTab",
                //    activated: loadControlsTab
                //}
        ],
        types: [
        ]
    };

    vmBackupExtensionActivationInit = function () {
        var vmBackupExtension = $.extend(this, global.VmBackupAdminExtension);

        $.extend(vmBackupExtension, {
            displayName: resources.VmBackupName,
            tooltip: resources.VmCBackupDisplay,
            displayStatus: global.waz.interaction.statusIconHelper(global.VmBackupAdminExtension.Controller.statusIcons, "Status"),
            getTypeDisplayName: function (typeName) {
                return resources.VmBackupServer;
            },
            navigationalViewModel: {
                uri: global.VmBackupAdminExtension.Controller.backupServerListUrl,
                subscriptionDetailLevel: global.Exp.Constants.subscriptionDetailLevel.none,
            },
            //virtualNavigationalViewModelDataSetInfo: global.VmBackupAdminExtension.Controller.getBackupServersDataSetInfo(),
            viewModels: [
                {
                    uri: global.VmBackupAdminExtension.Controller.groupListUrl,
                    subscriptionDetailLevel: global.Exp.Constants.subscriptionDetailLevel.none
                },
                {
                    uri: global.VmBackupAdminExtension.Controller.isResourceProviderRegisteredUrl,
                    subscriptionDetailLevel: global.Exp.Constants.subscriptionDetailLevel.none
                }
            ],
            viewModelUris: [
                global.VmBackupAdminExtension.Controller.adminSettingsUrl,
                global.VmBackupAdminExtension.Controller.adminProductsUrl,
            ],
            menuItems: [
                {
                    name: "VmBackupAdminMenuItem",
                    displayName: resources.VmBackupMenuName,
                    data: { resources: resources },
                    url: "#Workspaces/VmBackupAdminExtension",
                    description: resources.VmBackupMenuDescription,
                    subMenu: [
                        global.VmBackupAdminExtension.QuickCreate.getCreateBackupServerMenuItem(),
                        global.VmBackupAdminExtension.QuickCreate.getCreateGroupMenuItem()
                        //global.VmBackupAdminExtension.QuickCreate.getCreateVmmServerMenuItem()
                    ]
                }
            ],
            settings: {
                skipQuickStart: true
            },
            getResources: function () {
                return resources;
            }
        });

        vmBackupExtension.onApplicationStart = onApplicationStart;        
        vmBackupExtension.setCommands = clearCommandBar();
        vmBackupExtension.executeCommand = executeCommand;

        Shell.UI.Pivots.registerExtension(vmBackupExtension, function () {
            Exp.Navigation.initializePivots(this, navigation);
        });

        // Finally activate vmBackupExtension 
        $.extend(global.VmBackupAdminExtension, Shell.Extensions.activate(vmBackupExtension));
    };

    Shell.Namespace.define("VmBackupAdminExtension", {
        init: vmBackupExtensionActivationInit,
        loadVmmServersTab: loadVmmServersTab
    });

})(this, jQuery, Shell, Exp);