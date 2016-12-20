(function (global, undefined) {
    "use strict";

    var extensions = [{
        name: "VmBackupTenantExtension",
        displayName: "VM Backup",
        iconUri: "/Content/VmBackupTenant/VmBackup.png",
        iconShowCount: true,
        iconTextOffset: 11,
        iconInvertTextColor: true,
        displayOrderHint: 10 // Display it right after Networks extension 
    }];

    global.Shell.Internal.ExtensionProviders.addLocal(extensions);
})(this);