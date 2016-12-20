using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.WindowsAzurePack.VirtualMachineBackup.Service
{
    public class VmBackupLog 
    {
        public static VmBackupLog Current { get; private set; }

        private static EventLog log;
        static VmBackupLog()
        {
            Current = new VmBackupLog();
        }

        private VmBackupLog()
        {
            log = new EventLog("MgmtSvc-VmBackup", ".", "MgmtSvc-VmBackup");
        }

        public void WriteErrorMessage(string action, VmBackupEventId eventId, Exception e, params object[] args)
        {
            string message = VmBackupEventLogMessages.GetMessage(eventId, args);
            StringBuilder builder = new StringBuilder();
            builder.Append("Action: ");
            builder.AppendLine(action);
            builder.AppendLine();
            builder.Append("Message: ");
            builder.AppendLine(message);
            while (e != null)
            {
                builder.AppendLine();
                builder.Append("Exception Type: ");
                builder.AppendLine(e.GetType().ToString());
                builder.AppendLine();
                builder.Append("Exception Message: ");
                builder.AppendLine(e.Message);
                builder.AppendLine();
                builder.Append("Stack Trace: ");
                builder.AppendLine(e.StackTrace);

                e = e.InnerException;
            }
            log.WriteEntry(builder.ToString(), EventLogEntryType.Error, (int)eventId);
        }

        public void WriteWarningMessage(string action, string message, VmBackupEventId eventId)
        {
            log.WriteEntry(message, EventLogEntryType.Warning, (int)eventId);
        }

        public void WriteInformationMessage(string action, string message, VmBackupEventId eventId)
        {
            log.WriteEntry(message, EventLogEntryType.Information, (int)eventId);
        }

    }
}