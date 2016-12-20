/*globals window,jQuery,Exp,waz*/
(function ($, global, Shell, Exp, undefined) {
    "use strict";

    var resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        selectors =
        {
            AzureBackupEnabled: "#vmBackup_AzureBackupEnabled"
        };

    // Server Group Start
    function validateGroupInfo(value, element) {
        return true;
    }

    function getCreateGroupMenuItem() {
        var azureBackupEnabled = 0;
        global.VmBackupAdminExtension.Controller.forceRefreshGroupDataSet();

        return {
            name: "Create",
            displayName: resources.CreateGroupSubMenu,
            description: resources.AGroupRepresentAPoolOfServersOf,

            isEnabled: function () {
                if (!global.VmBackupAdminExtension.Controller.getIsResourceProviderRegistered()) {
                    return {
                        enabled: false,
                        description: resources.RegisterVmBackupProviderEndpoint
                    };
                } else {
                    return {
                        enabled: true,
                        description: resources.AGroupRepresentAPoolOfServersOf
                    };
                }
            },

            template: "createGroupMenu",
            label: resources.CreateAGroupLabel,

            open: function (context) {
                Shell.UI.Validation.setValidationContainer("#groupInfoForm", validateGroupInfo);

                $(selectors.AzureBackupEnabled).change(function () {
                    if (this.checked) {
                        azureBackupEnabled = 1;
                    } else {
                        azureBackupEnabled = 0;
                    }
                });
                azureBackupEnabled = 0;
            },

            ok: function (context) {
                var fields = context.fields,
                    dataWrapper;
                if (Shell.UI.Validation.validateContainer("#groupInfoForm")) {
                    dataWrapper = new waz.dataWrapper(global.VmBackupAdminExtension.Controller.getGroupDataSet());
                    dataWrapper.navigationColumnField = "GroupName";
                    waz.interaction.showProgress(
                            dataWrapper
                                .add(
                                    // Place holder
                                    {
                                        id: fields.vmBackup_GroupName,
                                        GroupName: fields.vmBackup_GroupName,
                                        Status: "Adding",
                                        Type: "VmBackup_ServerGroup"
                                    },
                                global.VmBackupAdminExtension.Controller.createGroup(fields.vmBackup_GroupName, azureBackupEnabled)),
                        {
                            initialText: resources.CreatingServerGroup.format(fields.vmBackup_GroupName),
                            successText: resources.ServerGroupIsCreated.format(fields.vmBackup_GroupName),
                            failureText: resources.ThereWasAnErrorCreatingYourGroup.format(fields.vmBackup_GroupName)
                        });
                    return true;
                }
                return false;
            }
        };
    }

    function openCreateGroup() {
        Exp.Drawer.openMenu("VmBackupAdminMenuItem/Create");
    }

    // Server Group End

    // Backup Server Start
    function validateBackupServerInfo(value, element) {
        return true;
    }

    function getCreateBackupServerMenuItem() {
        global.VmBackupAdminExtension.Controller.forceRefreshBackupServerDataSet();

        return {
            name: "ConnectTo",
            displayName: resources.CreateBackupServerSubMenu,
            description: resources.AddYourExistingDpmServer,

            data: {
                groups: null
            },

            isEnabled: function () {
                if (!global.VmBackupAdminExtension.Controller.getIsResourceProviderRegistered()) {
                    return {
                        enabled: false,
                        description: resources.RegisterVmBackupProviderEndpoint
                    };
                } else {
                    return {
                        enabled: true,
                        description: resources.AddYourExistingDpmServer
                    };
                }
            },

            template: "createBackupServerMenu",
            label: resources.CreateABackupServerLabel,

            opening: function (object) {
                var groupDataSet = global.VmBackupAdminExtension.Controller.getGroupDataSet();

                if (!groupDataSet || groupDataSet.data.length === 0) {
                    // TODO: Looks like quick create might want to support canceling from the opening function in the future
                    // currently this return does nothing and we will continue rendering quick add menu
                    Shell.UI.Notifications.add(resources.BackupServerAdditionErrorMessage, Shell.UI.InteractionSeverity.error, Shell.UI.InteractionBehavior.ok);
                    return;
                }

                object.data.groups = groupDataSet.data;
            },

            open: function (context) {
                Shell.UI.Validation.setValidationContainer("#backupServerInfoForm", validateBackupServerInfo);
            },

            ok: function (context) {
                var fields = context.fields,
                    selectedGroup,
                    dataWrapper;
                if (Shell.UI.Validation.validateContainer("#backupServerInfoForm")) {
                    $.each(context.data.groups, function (i, group) {
                        if (group.GroupId === fields.vmBackup_GroupId) {
                            selectedGroup = group;
                        }
                    });
                    dataWrapper = new waz.dataWrapper(global.VmBackupAdminExtension.Controller.getBackupServerDataSet());
                    dataWrapper.navigationColumnField = "ServerName";
                    waz.interaction.showProgress(
                            dataWrapper
                                .add(
                                    // Place holder
                                    {
                                        id: fields.vmBackup_BackupServerName,
                                        ServerName: fields.vmBackup_BackupServerName,
                                        GroupName: selectedGroup.GroupName,
                                        Status: "Adding",
                                        Type: "VmBackup_BackupServer"
                                    },
                                global.VmBackupAdminExtension.Controller.createBackupServer(fields.vmBackup_BackupServerName, fields.vmBackup_BackupServerUserName, fields.vmBackup_BackupServerPassword, fields.vmBackup_GroupId)),
                        {
                            initialText: resources.CreatingBackupServer.format(fields.vmBackup_BackupServerName),
                            successText: resources.BackupServerIsCreated.format(fields.vmBackup_BackupServerName),
                            failureText: resources.ThereWasAnErrorCreatingYourBackupServer.format(fields.vmBackup_BackupServerName)
                        });
                    return true;
                }
                return false;
            }
        };
    }

    function openCreateBackupServer() {
        Exp.Drawer.openMenu("VmBackupAdminMenuItem/ConnectTo");
    }

    // Backup Sever End

    // VMM Server Start

    function getCreateVmmServerMenuItem() {
        global.VmBackupAdminExtension.Controller.forceRefreshVmmServerDataSet();

        return {
            name: "ConnectToVmm",
            displayName: resources.CreateVmmServerSubMenu,
            description: resources.AddYourExistingVmmServer,

            data: {
                groups: null
            },

            isEnabled: function () {
                if (!global.VmBackupAdminExtension.Controller.getIsResourceProviderRegistered()) {
                    return {
                        enabled: false,
                        description: resources.RegisterVmBackupProviderEndpoint
                    };
                } else {
                    return {
                        enabled: true,
                        description: resources.AddYourExistingVmmServer
                    };
                }
            },

            template: "createVmmServerMenu",
            label: resources.CreateAVmmServerLabel,

            open: function (context) {
                Shell.UI.Validation.setValidationContainer("#vmmServerInfoForm");
            },

            ok: function (context) {
                var fields = context.fields,
                    selectedGroup,
                    dataWrapper;
                if (Shell.UI.Validation.validateContainer("#vmmServerInfoForm")) {
                    dataWrapper = new waz.dataWrapper(global.VmBackupAdminExtension.Controller.getVmmServerDataSet());
                    dataWrapper.navigationColumnField = "ServerName";
                    waz.interaction.showProgress(
                            dataWrapper
                                .add(
                                    // Place holder
                                    {
                                        id: fields.vmBackup_VmmServerName,
                                        Status: "Adding",
                                        Type: "VmBackup_VmmServer"
                                    },
                                global.VmBackupAdminExtension.Controller.createVmmServer(fields.vmBackup_VmmServerName, fields.vmBackup_VmmServerUserName, fields.vmBackup_VmmServerPassword, fields.vmBackup_GroupId)),
                        {
                            initialText: resources.CreatingBackupServer.format(fields.vmBackup_BackupServerName),
                            successText: resources.BackupServerIsCreated.format(fields.vmBackup_BackupServerName),
                            failureText: resources.ThereWasAnErrorCreatingYourBackupServer.format(fields.vmBackup_BackupServerName)
                        });
                    return true;
                }
                return false;
            }
        };
    }
    // VMM Server End
    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.QuickCreate = {
        getCreateGroupMenuItem: getCreateGroupMenuItem,
        openCreateGroup: openCreateGroup,
        getCreateBackupServerMenuItem: getCreateBackupServerMenuItem,
        openCreateBackupServer: openCreateBackupServer,
        getCreateVmmServerMenuItem: getCreateVmmServerMenuItem
    };
})(jQuery, this, this.Shell, this.Exp);
