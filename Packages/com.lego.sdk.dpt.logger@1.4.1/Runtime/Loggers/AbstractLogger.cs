using System;
using System.Collections.Generic;

namespace LEGO.Logger.Loggers 
{
    public abstract class AbstractLogger : ILog
    {
        protected Type LoggingType { get; set; }

        public string Name { get; private set; }

        public HashSet<ILogAppender> Appenders { get; private set; }

        protected AbstractLogger(Type type)
        {
            if (type != null)
            {
                LoggingType = type;
                Name = type.FullName;
            }
            else
            {
                Name = "Anonymous";
            }
            Appenders = new HashSet<ILogAppender>();
        }

        public void AddAppender(ILogAppender appender)
        {
            Appenders.Add(appender);
        }

        public void RemoveAppender(ILogAppender appender)
        {
            Appenders.Remove(appender);
        }

		/* Test if a level is enabled for logging */
		public bool IsVerboseEnabled
		{
			get { return IsLogLevelEnabled(LogLevel.VERBOSE); }
		}


		public bool IsDebugEnabled
        {
            get { return IsLogLevelEnabled(LogLevel.DEBUG); }
        }

        public bool IsInfoEnabled
        {
            get { return IsLogLevelEnabled(LogLevel.INFO); }
        }

        public bool IsWarnEnabled
        {
            get { return IsLogLevelEnabled(LogLevel.WARN); }
        }

        public bool IsErrorEnabled
        {
            get { return IsLogLevelEnabled(LogLevel.ERROR); }
        }

        public bool IsFatalEnabled
        {
            get { return IsLogLevelEnabled(LogLevel.FATAL); }
        }

        public bool IsLogLevelEnabled(LogLevel level)
        {
            return LogManager.RootLevel <= level;
        }

		/* Log a message object */
		public void Verbose(object message)
		{
			LogMessage(message, LogLevel.VERBOSE);
		}

		public void Debug(object message)
        {
            LogMessage(message, LogLevel.DEBUG);
        }

        public void Info(object message)
        {
            LogMessage(message, LogLevel.INFO);
        }

        public void Warn(object message)
        {
            LogMessage(message, LogLevel.WARN);
        }

        public void Error(object message)
        {
            LogMessage(message, LogLevel.ERROR);
        }

        public void Fatal(object message)
        {
            LogMessage(message, LogLevel.FATAL);
        }

        /* Log a message object and exception */
        public void Debug(object message, Exception t)
        {
            LogMessage(message, t, LogLevel.DEBUG);
        }

        public void Info(object message, Exception t)
        {
            LogMessage(message, t, LogLevel.INFO);
        }

        public void Warn(object message, Exception t)
        {
            LogMessage(message, t, LogLevel.WARN);
        }

        public void Error(object message, Exception t)
        {
            LogMessage(message, t, LogLevel.ERROR);
        }

        public void Fatal(object message, Exception t)
        {
            LogMessage(message, t, LogLevel.FATAL);
        }

        public abstract void LogMessage(object message, LogLevel logLevel);

        public abstract void LogMessage(object message, Exception t, LogLevel logLevel);

    }
}

