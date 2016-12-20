/// <reference path="VmBackupadmin.controller.js" />
(function ($, global, fx, Exp, undefined) {
    "use strict";

    var holder,
        resources = global.Resources.getResources("Microsoft.WindowsAzurePack.VirtualMachineBackup.LocalizableResources.AdminExtension.Resource"),
        steps,
        savedWizardParams,
        wizard,
        webSystemId;

    function cleanup() {
    }

    function addAction(holder, name, callback) {
        holder.quickstartItems(
            {
                actions: [
                    {
                        name: name,
                        callback: function (event) {
                            event.preventDefault();
                            callback(webSystemId);
                            return false;
                        }
                    }
                ]
            });
    }

    // TODO: this is a copy/paste, not very clean
    function getSkipQuickStart() {
        var skipQuickStart = false //SqlAdminExtension.settings.skipQuickStart;

        if (skipQuickStart === null || skipQuickStart === undefined) {
            setSkipQuickStart(true);
            return true;
        }

        return skipQuickStart;
    }

    // TODO: this is a copy/paste, not very clean
    function setSkipQuickStart(value) {
        //SqlAdminExtension.settings.skipQuickStart = value;
        /// <disable>JS2076.IdentifierIsMiscased</disable>
        global.Exp.UserSettings.updateGlobalUserSetting("SqlAdmin-skipQuickStart", JSON.stringify(value));
        /// <enable>JS2076.IdentifierIsMiscased</enable>
    }

    function openRegisterSpfEndpointWizard() {
        showRegisterSpfEndpointWizard(null);
    }

    function showRegisterSpfEndpointWizard(wizardParams) {
        var wizSteps;

        savedWizardParams = wizardParams;

        wizSteps = [
                {
                    template: "registerSpfEndpointDialog",
                    onStepActivate: onRegisterSpfEndpointWizPageActivated,
                    onNextStep: onRegisterSpfEndpointComplete,
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
    }

    function onRegisterSpfEndpointWizPageActivated() {
        initializeDialog();
        Shell.UI.Validation.setValidationContainer("#aux-editEndpointForm");
    }

    function onRegisterSpfEndpointComplete() {
        var isValid, promise, endpoint, username, password;
        isValid = validateRegisterSpfEndpointWizStep();

        if (isValid) {
            endpoint = $("#endpointUrl").val();
            username = $("#userName").val();
            password = $("#password").val();
            promise = global.VmBackupAdminExtension.Controller.setSpfServer(endpoint, username, password);
            waz.interaction.showProgress(
                promise,
                {
                    initialText: resources.SettingSpfServer,
                    successText: resources.SuccessfullySetSpfServer,
                    failureText: resources.FailedToSetSpfServer
                });
            //promise.always(function () {
            //    global.VmBackupTenantExtension.Controller.forceRefreshAllVirtualMachinesDataSet();
            //});
        }
    }

    function validateRegisterSpfEndpointWizStep() {
        return Shell.UI.Validation.validateContainer("#aux-editEndpointForm");
    }

    function loadTab(renderData, container) {
        var tabResources =
        {
            /// <disable>JS2076.IdentifierIsMiscased</disable>
            SkipQuickStartCheckedState: true,
            Title: resources.QuickStartTabTitle,
            SubTitle: resources.ManageYourVmBackupSubTitle,
            ChoiceText: resources.SkipQuickStartChoice,
            QuickStartImage: fx.resources.getContentUrl("Content/VmBackupAdmin/Images/quickstart.png"),

            RegisterSpfEndpointTitle: resources.RegisterSpfEndpointTitle,
            RegisterSpfEndpointAction: resources.RegisterSpfEndpointDescription,
            RegisterVmmServersTitle: resources.RegisterVmmServersTitle,
            RegisterVmmServersAction: resources.RegisterVmmServersDescription
            /// <enable>JS2076.IdentifierIsMiscased</enable>
        },
        holder = container.find(".adminQuickStart")
                .html(global.VmBackupAdminExtension.templates.quickStartTabContent.render(tabResources));

        $(".quickstartCheckbox").fxCheckBox({
            value: getSkipQuickStart(),
            trackedit: false,
            change: function (event, data) {
                setSkipQuickStart(data.value);
            }
        });

        if (!global.VmBackupAdminExtension.Controller.getIsResourceProviderRegistered()) {
            $(".quick-start-view").hide();
        } else {
            $(".hs-empty").hide();

            addAction(
                    holder.find(".quickstart-registerSpfEndpoint .detail .detailDescription"),
                    tabResources.RegisterSpfEndpointAction,
                    openRegisterSpfEndpointWizard
                );

            addAction(
                holder.find(".quickstart-registerVmmServers .detail .detailDescription"),
                tabResources.RegisterVmmServersAction,
                global.VmBackupAdminExtension.Controller.navigateToVmmServersTab
            );

        }
    }

    function executeCommand(commandId) {
        return false;
    }

    // Public
    global.VmBackupAdminExtension = global.VmBackupAdminExtension || {};
    global.VmBackupAdminExtension.QuickStartTab = {
        loadTab: loadTab,
        cleanup: cleanup,
        executeCommand: executeCommand
    };
})(jQuery, this, this.fx, this.Exp);