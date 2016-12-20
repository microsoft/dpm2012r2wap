(function (global, undefined) {
    "use strict";

    var extensions = [{
        name: "VmBackupAdminExtension",
        displayName: "VM Backup",
        iconUri: "/Content/VmBackupAdmin/VMBackup.png",
        iconShowCount: true,
        iconTextOffset: 11,
        iconInvertTextColor: true,
        displayOrderHint: 225
    }];

    global.Shell.Internal.ExtensionProviders.addLocal(extensions);
})(this);