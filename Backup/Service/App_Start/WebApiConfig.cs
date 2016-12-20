// ---------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Globalization;
using System.Web.Http;

using Microsoft.WindowsAzure.Server.Common;

using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Controllers;
using Microsoft.WindowsAzurePack.VirtualMachineBackup.Service.Clients;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, string connection)
        {
            ConfigStorageClient configStorageClient = new ConfigStorageClient(connection);
            DpmServerClient dpmServerClient = new DpmServerClient();
            VmmServerClient vmmServerClient = new VmmServerClient();

            //NEW Routings
            config.SetControllerMapping(
                new ControllerMapping(
                    "subscriptions/{subscriptionId}/virtualmachines",
                    () => new VirtualMachinesController(configStorageClient, dpmServerClient)),
                new ControllerMapping(
                    "subscriptions/{subscriptionId}/virtualmachines/{id}",
                    () => new VirtualMachinesController(configStorageClient, dpmServerClient)),
                new ControllerMapping(
                    "subscriptions/{subscriptionId}/virtualmachines/{id}/restore/{recoverySourceId}",
                    () => new VirtualMachinesController(configStorageClient, dpmServerClient)),
                new ControllerMapping(
                    "servergroups",
                    () => new ServerGroupsController(configStorageClient)),
                new ControllerMapping(
                    "servergroups/{groupId}",
                    () => new ServerGroupsController(configStorageClient)),
                new ControllerMapping(
                    "backupservers",
                    () => new BackupServersController(configStorageClient, dpmServerClient)),
                new ControllerMapping(
                    "backupservers/{serverId}",
                    () => new BackupServersController(configStorageClient, dpmServerClient)),
                new ControllerMapping(
                    "vmmservers",
                    () => new VmmServersController(configStorageClient, vmmServerClient)),
                new ControllerMapping(
                    "vmmservers/{stampId}",
                    () => new VmmServersController(configStorageClient, vmmServerClient)),
                new ControllerMapping(
                    "spfserver",
                    () => new SpfServerController(configStorageClient, vmmServerClient)),

                new ControllerMapping(
                    "admin/defaultquota",
                    () => new QuotaController()),
                new ControllerMapping(
                    "admin/quota",
                    () => new QuotaController()),
                new ControllerMapping(
                    "admin/subscriptions",
                    () => new SubscriptionsController()),
                new ControllerMapping(
                    "admin/subscriptions/{subscriptionId}/usagesummary",
                    () => new UsageSummaryController()),
               new ControllerMapping(
                    "usage",
                    () => new UsageController())

                );


            //// Get usage collection interval 
            //var collectionIntervalHours = GetCollectionInterval();

            //// Get first run span
            //var firstRunSpan = GetFirstRunSpan();
             
            //ActionRunner runner = new ActionRunner();
            //UsageCollector collector = new UsageCollector();
            //collector.Initialize(runner, firstRunSpan, TimeSpan.FromHours(collectionIntervalHours), configStorageClient); 

            //OLD Routings
            config.Routes.MapHttpRoute(
               name: "AdminSettings",
               routeTemplate: "admin/settings",
               defaults: new { controller = "AdminSettings" });

            config.Routes.MapHttpRoute(
                name: "AdminProducts",
                routeTemplate: "admin/products",
                defaults: new { controller = "Products" });

            config.Routes.MapHttpRoute(
                name: "AdminFileServers",
                routeTemplate: "admin/fileservers",
                defaults: new { controller = "FileServers" });

            config.Routes.MapHttpRoute(
               name: "VmBackupQuota",
               routeTemplate: "admin/quota",
               defaults: new { controller = "Quota" });

            config.Routes.MapHttpRoute(
               name: "VmBackupDefaultQuota",
               routeTemplate: "admin/defaultquota",
               defaults: new { controller = "Quota" });

            config.Routes.MapHttpRoute(
               name: "Subscription",
               routeTemplate: "admin/subscriptions",
               defaults: new { controller = "Subscriptions" });

            config.Routes.MapHttpRoute(
               name: "FileShares",
               routeTemplate: "subscriptions/{subscriptionId}/fileshares",
               defaults: new { controller = "FileShare" });

            config.Routes.MapHttpRoute(
               name: "Usage",
               routeTemplate: "usage",
               defaults: new { controller = "Usage" });
        }

        private static double GetCollectionInterval()
        {
            double interval;

            if (!double.TryParse(ConfigSettingReader.Instance.GetSetting("UsageDataCollectionIntervalHours"), NumberStyles.Any, CultureInfo.InvariantCulture, out interval) || interval <= 0)
            {
                return 24;
            }

            return interval;
        }

        private static TimeSpan GetFirstRunSpan()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            string collectionTargetTime;

            if (!string.IsNullOrWhiteSpace(ConfigSettingReader.Instance.GetSetting("UsageDataCollectionTargetTime")))
            {
                collectionTargetTime = ConfigSettingReader.Instance.GetSetting("UsageDataCollectionTargetTime");
            }
            else
            {
                collectionTargetTime = "12:00:00 AM"; // If no collection time is specified, we default to 12 am
            }

            DateTime currentTime = DateTime.Now;
            DateTime timeOfCollection = DateTime.Parse(collectionTargetTime, CultureInfo.InvariantCulture);

            // If the time of collection is earlier than the current time, then add a day so we will collect data on the next day
            if (DateTime.Compare(timeOfCollection, currentTime) < 0)
            {
                timeOfCollection = timeOfCollection.AddDays(1);
            }

            // Add a random number of minutes so that all instances of this provider do not wake up at the exact same time and try to collect.
            timeOfCollection.AddMinutes(rnd.Next(59));

            return timeOfCollection - currentTime;
        }
    }
}
