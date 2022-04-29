using System;
using System.Collections.Generic;

namespace LEGO.Logger
{
	public enum LogLevel
	{
		UNDEFINED = -1,
		
		VERBOSE = 0, 
		DEBUG, 
		INFO, 
		WARN, 
		ERROR, 
		FATAL, 
		OFF
	}

    public interface ILog
    {
        string Name { get; }

        HashSet<ILogAppender> Appenders { get; }
        void AddAppender(ILogAppender appender);
        void RemoveAppender(ILogAppender appender);

	    //TODO: Consider removing these checks since the same can be determined from the loglevel.
        bool IsVerboseEnabled { get; }
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }

		void LogMessage(object message, LogLevel logLevel);
		void LogMessage(object message, Exception t, LogLevel logLevel);

		void Verbose(object message);
		void Debug(object message);
        void Info(object message);
        void Warn(object message);
        void Error(object message);
        void Fatal(object message);

        void Debug(object message, Exception t);
        void Info(object message, Exception t);
        void Warn(object message, Exception t);
        void Error(object message, Exception t);
        void Fatal(object message, Exception t);

    }
}