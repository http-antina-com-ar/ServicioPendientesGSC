using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServicioPendientes
{
    public class Logger
    {

        private static string logPath = SettingsPendings.Default.logPath;
        private static string logFile = "\\LogPendientesGCS_" + DateTime.Now.Year + DateTime.Now.Month.ToString("d2") + DateTime.Now.Day.ToString("d2") + ".log";
        private static string logDestiny = logPath + logFile;
        private static EventLog eventLog;
        string logName = "Application";
        string logSource = "Servicio Pendientes GSC";


        public enum LogType
        {
            LOG_FILE = 1,
            WIN_EVENT = 2,
            BOTH = 3
        }

        public enum LogLevel
        {
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            FATAL = 4
        }

        
        public Logger()
        {
            eventLog = new EventLog();

            if (!EventLog.SourceExists(logSource))
            {
                EventLog.CreateEventSource(logSource, logName);
            }
            eventLog.Source = logSource;
        }
        
        public void Log(string message, LogLevel logLevel)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logDestiny, true))
                {
                    writer.WriteLine("[" + DateTime.Now.ToString() + "] - [" + logLevel.ToString() + "] - " + message);
                }
            }
            catch (Exception ex)
            {
                string msgLog = "ERROR: No se pudo iniciar el archivo de Log en: [" + logDestiny + "]" + ex.Message;
                EventLog.WriteEntry(logSource, msgLog, EventLogEntryType.Error);
                //ERROR EN MAIL
            }
        }

        public void Log(LogType logType, string message, LogLevel logLevel)
        {
            EventLogEntryType logEvent = EventLogEntryType.Information;

            switch (logLevel)
            {
                case LogLevel.WARNING:
                    logEvent = EventLogEntryType.Warning;
                    break;
                case LogLevel.FATAL:
                    logEvent = EventLogEntryType.Error;
                    break;
                case LogLevel.ERROR:
                    logEvent = EventLogEntryType.Error;
                    break;
            }

            switch (logType)
            {
                case LogType.LOG_FILE:
                    Log(message, logLevel);
                    break;
                case LogType.WIN_EVENT:
                    EventLog.WriteEntry(logSource, message, logEvent);
                    break;
                case LogType.BOTH:
                    Log(message, logLevel);
                    EventLog.WriteEntry(logSource, message, logEvent);
                    break;
            }
        }
     }
}
