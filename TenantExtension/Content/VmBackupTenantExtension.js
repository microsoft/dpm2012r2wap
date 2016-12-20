/// <reference path="scripts/VmBackupTenant.VmBackupTab.js" />
/// <reference path="scripts/VmBackupTenant.controller.js" />
/*globals Shell,azureTabs,SqlAdminExtension,Exp, waz*/

(function (global, $, Shell, Exp, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.TenantExtension.Resource"),
        VmBackupTenantExtensionActivationInit,
        navigation,
        serviceName = "vmBackup";

    function onNavigateAway() {
        Exp.UI.Commands.Contextual.clear();
        Exp.UI.Commands.Global.clear();
        Exp.UI.Commands.update();
    }

    function executeCommand(commandId, subCommandId, appData, context) {
        return VmBackupTenantExtension.VmBackupTab.executeCommand(commandId, subCommandId, appData, context);
    }

    function loadSettingsTab(extension, renderArea, renderData) {
        global.VmBackupTenantExtension.SettingsTab.loadTab(renderData, renderArea);
    }

    function vmBackupTab(extension, renderArea, renderData) {
        global.VmBackupTenantExtension.VmBackupTab.loadTab(renderData, renderArea);
    }

    function getSubscriptionIds(subsFromService) {
        var subs = [];
        for (var i = 0; i < subsFromService.length; i++) {
            subs[i] = subsFromService[i].id;
        }

        return subs;
    }

    global.VmBackupTenantExtension = global.VmBackupTenantExtension || {};

    navigation = {
        tabs: [
            {
                id: "vmBackup",
                displayName: resources.VmBackupTabName,
                template: "VmBackupTab",
                activated: vmBackupTab
            }            
        ],
        types: [
        ]
    };

    VmBackupTenantExtensionActivationInit = function () {
        var subs = Exp.Rdfe.getSubscriptionList(),
            subscriptionRegisteredToService = global.Exp.Rdfe.getSubscriptionsRegisteredToService("vmbackup"),
            vmBackupExtension = $.extend(this, global.VmBackupTenantExtension),
            subIds = getSubscriptionIds(subscriptionRegisteredToService);

        // Don't activate the extension if user doesn't have a plan that includes the service.
        if (subscriptionRegisteredToService.length === 0) {
            return false; // Don't want to activate? Just bail
        }

        $.extend(vmBackupExtension, {
            displayStatus: global.waz.interaction.statusIconHelper(global.VmBackupTenantExtension.VmBackupTab.statusIcons, "Status"),
            getTypeDisplayName: function (type) {
                if (type === "VmBackup") {
                    return resources.VmBackupType;
                } else {
                    throw "Error: unknown type: " + type; // Internal error message
                }
            },
            viewModelUris: [],
            displayName: resources.VmBackupName,
            subscriptions: subIds,
            navigationalViewModel: {
                uri: vmBackupExtension.Controller.listProtectedMachinesUrl,
                subscriptionDetailLevel: global.Exp.Constants.subscriptionDetailLevel.filtered,
                ajaxData: subIds
            },
            //menuItems: [
            //    {
            //        name: "VmBackup",
            //        displayName: "VM Backup",
            //        url: "#Workspaces/VmBackupTenantExtension",
            //        preview: "createPreview",
            //        subMenu: [
            //            {
            //                name: "Create",
            //                displayName: "Create File Share",
            //                description: "Quickly Create File Share on a File Server",
            //                template: "CreateFileShare",
            //                label: "Create",
            //                subMenu: [
            //                    {
            //                        name: "QuickCreate",
            //                        displayName: "FileFile",
            //                        template: "CreateFileShare"                                    
            //                    }
            //                ]
            //            }
            //        ]
            //    }
            //],
            getResources: function () {
                return resources;
            }
        });

        vmBackupExtension.onNavigateAway = onNavigateAway;
        vmBackupExtension.navigation = navigation;
        vmBackupExtension.executeCommand = executeCommand;

        Shell.UI.Pivots.registerExtension(vmBackupExtension, function () {
            Exp.Navigation.initializePivots(this, this.navigation);
        });

        // Finally activate and give "the" vmBackupExtension the activated extension since a good bit of code depends on it
        $.extend(global.VmBackupTenantExtension, Shell.Extensions.activate(vmBackupExtension));
    };

    function getQuickCreateFileShareMenuItem() {
        //return {
        //    name: "QuickCreate",
        //    displayName: "Create File Share",
        //    description: "Create new file share",
        //    template: "quickCreateWithRdfe",
        //    label: resources.CreateMenuItem,

        //    opening: function () {
        //        AccountsAdminExtension.AccountsTab.renderListOfHostingOffers(offersListSelector);
        //    },

        //    open: function () {
        //        // Enables As-You-Type validation experience on a container specified
        //        Shell.UI.Validation.setValidationContainer(valContainerSelector);
        //        // Enables password complexity feedback experience on a container specified
        //        Shell.UI.PasswordComplexity.parse(valContainerSelector);
        //    },

        //    ok: function (object) {
        //        var dialogFields = object.fields,
        //            isValid = validateAccount();

        //        if (isValid) {
        //            createAccountWithRdfeCore(dialogFields);
        //        }
        //        return false;
        //    },

        //    cancel: function (dialogFields) {
        //        // you can return false to cancel the closing
        //    }
        //};
    }

    Shell.Namespace.define("VmBackupTenantExtension", {
        serviceName: serviceName,
        init: VmBackupTenantExtensionActivationInit,
        getQuickCreateFileShareMenuItem: getQuickCreateFileShareMenuItem
    });
})(this, jQuery, Shell, Exp);
