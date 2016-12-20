/*globals window,jQuery,cdm, VmBackupAdminExtension*/
(function ($, global, undefined) {
    "use strict";

    var baseUrl = "/VmBackupAdmin",
        isResourceProviderRegisteredUrl = baseUrl + "/IsResourceProviderRegistered",
        groupListUrl = baseUrl + "/ListServerGroups",
        backupServerListUrl = baseUrl + "/ListBackupServers",
        vmmServerListUrl = baseUrl + "/ListVmmServers",

        adminSettingsUrl = baseUrl + "/AdminSettings",
        adminProductsUrl = baseUrl + "/Products",
        adminFileServersUrl = baseUrl + "/FileServers",
        resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        statusIcons = {
            Active: {
                text: resources.StatusActive,
                iconName: "complete"
            },
            Default: {
                iconName: "spinner"
            }
        };

    function makeAjaxCall(url, data) {
        return Shell.Net.ajaxPost({
            url: url,
            data: data
        });
    }

    function getIsResourceProviderRegistered() {
        return Exp.Data.getLocalDataSet(isResourceProviderRegisteredUrl).data;
    }

    function getGroupDataSetInfo() {
        return {
            url: groupListUrl,
            dataSetName: groupListUrl,
        };
    }

    function getGroupDataSet() {
        return Exp.Data.getLocalDataSet(groupListUrl);
    }

    function forceRefreshGroupDataSet() {
        Exp.Data.getData({ url: groupListUrl, forceCacheRefresh: true });
    }

    function createGroup(name, azureBackupEnabled) {
        return Shell.Net.ajaxPost({
            data: {
                groupName: name,
                azureBackupEnabled: azureBackupEnabled
            },
            url: baseUrl + "/CreateServerGroup"
        });
    }

    function deleteGroup(groupId, groupName) {
        return waz.dataWrapper.create(getGroupDataSet().data, "id")
            .remove(groupName,
             Shell.Net.ajaxPost({
                 data: {
                     groupId: groupId,
                 },
                 url: baseUrl + "/DeleteServerGroup"
             }));
    }

    function getBackupServersDataSetInfo() {
        return {
            url: backupServerListUrl,
            dataSetName: backupServerListUrl,
        };
    }

    function getBackupServerDataSet() {
        return Exp.Data.getLocalDataSet(backupServerListUrl);
    }

    function forceRefreshBackupServerDataSet() {
        Exp.Data.getData({ url: backupServerListUrl, forceCacheRefresh: true });
    }

    function createBackupServer(serverName, userName, password, groupId) {
        return Shell.Net.ajaxPost({
            data: {
                serverName: serverName,
                userName: userName,
                password: password,
                groupId: groupId
            },
            url: baseUrl + "/CreateBackupServer"
        });
    }

    function updateBackupServer(serverId, serverName, userName, password) {
        return waz.dataWrapper.create(getBackupServerDataSet().data, "ServerId", "Status")
            .update(
            {
                id: serverId,
                ServerId: serverId,
                Status: "Updating..."
            },
             Shell.Net.ajaxPost({
                 data: {
                     serverId: serverId,
                     serverName: serverName,
                     userName: userName,
                     password: password
                 },
                 url: baseUrl + "/UpdateBackupServer"
             }));
    }

    function deleteBackupServer(serverId) {
        return waz.dataWrapper.create(getBackupServerDataSet().data, "id")
            .remove(serverId,
             Shell.Net.ajaxPost({
                 data: {
                     serverId: serverId,
                 },
                 url: baseUrl + "/DeleteBackupServer"
             }));
    }

    function navigateToVmmServersTab() {
        Shell.UI.Navigation.navigate({
            extension: global.VmBackupAdminExtension,
            view: "vmmServers"
        });
    }

    function getVmmServersDataSetInfo() {
        return {
            url: vmmServerListUrl,
            dataSetName: vmmServerListUrl,
        };
    }

    function getVmmServerDataSet() {
        return Exp.Data.getLocalDataSet(vmmServerListUrl);
    }

    function forceRefreshVmmServerDataSet() {
        Exp.Data.getData({ url: vmmServerListUrl, forceCacheRefresh: true });
    }

    function createVmmServer(stampId, serverName, userName, password) {
        return waz.dataWrapper.create(getVmmServerDataSet().data, "StampId", "Status")
            .update(
            {
                id: stampId,
                StampId: stampId,
                Status: "Registering..."
            },
             Shell.Net.ajaxPost({
                data: {
                    stampId: stampId,
                    serverName: serverName,
                    userName: userName,
                    password: password
                },
                url: baseUrl + "/CreateVmmServer"
            }));
    }

    function updateVmmServer(stampId, serverName, userName, password) {
        return waz.dataWrapper.create(getVmmServerDataSet().data, "StampId", "Status")
            .update(
            {
                id: stampId,
                StampId: stampId,
                Status: "Updating..."
            },
             Shell.Net.ajaxPost({
                 data: {
                     stampId: stampId,
                     serverName: serverName,
                     userName: userName,
                     password: password
                 },
                 url: baseUrl + "/UpdateVmmServer"
             }));
    }

    function setSpfServer(adminUrl, userName, password) {
        return Shell.Net.ajaxPost({
            data: {
                adminUrl: adminUrl,
                userName: userName,
                password: password
            },
            url: baseUrl + "/SetSpfServer"
        });
    }

    function updateAdminSettings(newSettings) {
        return makeAjaxCall(baseUrl + "/UpdateAdminSettings", newSettings);
    }

    function invalidateAdminSettingsCache() {
        return global.Exp.Data.getData({
            url: global.VmBackupAdminExtension.Controller.adminSettingsUrl,
            dataSetName: VmBackupAdminExtension.Controller.adminSettingsUrl,
            forceCacheRefresh: true
        });
    }

    function getCurrentAdminSettings() {
        return makeAjaxCall(global.VmBackupAdminExtension.Controller.adminSettingsUrl);
    }

    function isResourceProviderRegistered() {
        global.Shell.UI.Spinner.show();
        global.VmBackupAdminExtension.Controller.getCurrentAdminSettings()
        .done(function (response) {
            if (response && response.data.EndpointAddress) {
                return true;
            }
            else {
                return false;
            }
        })
         .always(function () {
             global.Shell.UI.Spinner.hide();
         });
    }

    function trueFalseEnabledFormatter(value) {
        if (value === null || value === undefined) {
            return value;
        } else if (value) {
            return resources.EnabledStatus;
        }
        else {
            return resources.DisabledStatus;
        }
    }

    // Public
    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.Controller = {
        adminSettingsUrl: adminSettingsUrl,
        adminProductsUrl: adminProductsUrl,
        adminFileServersUrl: adminFileServersUrl,

        isResourceProviderRegisteredUrl: isResourceProviderRegisteredUrl,
        groupListUrl: groupListUrl,
        backupServerListUrl: backupServerListUrl,
        statusIcons: statusIcons,
        getIsResourceProviderRegistered: getIsResourceProviderRegistered,
        getGroupDataSetInfo: getGroupDataSetInfo,
        getGroupDataSet: getGroupDataSet,
        forceRefreshGroupDataSet: forceRefreshGroupDataSet,
        createGroup: createGroup,
        deleteGroup: deleteGroup,
        getBackupServersDataSetInfo: getBackupServersDataSetInfo,
        getBackupServerDataSet: getBackupServerDataSet,
        forceRefreshBackupServerDataSet: forceRefreshBackupServerDataSet,
        createBackupServer: createBackupServer,
        updateBackupServer: updateBackupServer,
        deleteBackupServer: deleteBackupServer,
        navigateToVmmServersTab: navigateToVmmServersTab, 
        getVmmServersDataSetInfo: getVmmServersDataSetInfo,
        getVmmServerDataSet: getVmmServerDataSet,
        forceRefreshVmmServerDataSet: forceRefreshVmmServerDataSet,
        createVmmServer: createVmmServer,
        updateVmmServer: updateVmmServer,
        setSpfServer: setSpfServer,
        trueFalseEnabledFormatter: trueFalseEnabledFormatter,

        updateAdminSettings: updateAdminSettings,
        getCurrentAdminSettings: getCurrentAdminSettings,
        invalidateAdminSettingsCache: invalidateAdminSettingsCache,
        isResourceProviderRegistered: isResourceProviderRegistered
    };
})(jQuery, this);
