#if UNITY_ANDROID
using System;
using UnityEngine;
#endif

namespace LEGO.Logger.Appenders
{
    /// <summary>
    /// Android log appender utilizing 'AndroidJavaObject("android.util.log")' to direct all logs to the default
    /// android console which can be picked up by ADB. 
    /// </summary>
    public sealed class AndroidAdbLogAppender : ILogAppender
    {
        public LogLevel LevelFilter { get; set; }

        #if UNITY_ANDROID
        private readonly AndroidJavaObject log;
        #endif

        public AndroidAdbLogAppender()
        {
            //When running on a physical device we want debug log as well
            LevelFilter = LogLevel.DEBUG;

            #if UNITY_ANDROID
            log = new AndroidJavaClass("android.util.Log");
            #endif
        }

        public void Print(ILogMessage message)
        {

            #if UNITY_ANDROID
            if (message.Level < LevelFilter)
                return;

            var msg = string.Format("[{0}]\t{1:d/M/yy HH:mm:ss.fff}\t{2}", message.ClassName, DateTime.Now, message.Text);

            string logMethod;
            switch (message.Level) 
            {
                case LogLevel.FATAL:
                case LogLevel.ERROR:
                    logMethod = "e"; break;
                case LogLevel.WARN:
                    logMethod = "w"; break;
                case LogLevel.INFO:
                    logMethod = "i"; break;
                case LogLevel.DEBUG:
                    logMethod = "i"; break;
                //logMethod = "d"; break;
                case LogLevel.VERBOSE:
                default:
                    logMethod = "i"; break;
                //logMethod = "v"; break;
            }
            var tag = message.ClassName;
            if (tag.Length > 8) 
                tag = tag.Substring(0,8);
           
            log.CallStatic<int>(logMethod, tag, msg);
#endif
        }
    }
}