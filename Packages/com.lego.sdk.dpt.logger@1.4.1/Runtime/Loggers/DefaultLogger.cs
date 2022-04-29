using System;

namespace LEGO.Logger.Loggers
{
    internal sealed class DefaultLogger : AbstractLogger
    {
        public DefaultLogger(Type type) : base(type) { }

        public override void LogMessage(object message, LogLevel logLevel)
        {
            if (!IsLogLevelEnabled(logLevel)) 
                return;
            
            var logMessage = (message != null) ? message.ToString() : "(null)";
            int messageLengthLimit = 2048;
            
            while(logMessage.Length > 0)
            {
                var substringLength = Math.Min(logMessage.Length, messageLengthLimit);
                var limitedMessage = logMessage.Substring(0, substringLength);
                logMessage = logMessage.Remove(0, substringLength);
                
                foreach (var appender in Appenders)
                    appender.Print(new LogMessage(this, limitedMessage, logLevel, DateTime.Now));
            }
        }

        public override void LogMessage(object message, Exception t, LogLevel logLevel)
        {
            if (!IsLogLevelEnabled(logLevel)) 
                return;
            
            var logTxt = (message != null) ? message.ToString() : "(null)";
            var logMessage = new LogMessage(this, logTxt, logLevel, DateTime.Now, t);
            
            foreach (var appender in Appenders)
                appender.Print(logMessage);
        }                   


    }
}