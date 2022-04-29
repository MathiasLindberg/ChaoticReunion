using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LEGO.Logger.Appenders;
using LEGO.Logger.Loggers;
using UnityEngine;

namespace LEGO.Logger
{
    /// <summary>
    /// Main entry point for the LEGO logging system.
    /// </summary>
    public class LogManager : IDisposable
    {
        private static readonly Dictionary<string, ILog> Loggers = new Dictionary<string, ILog>();
        private static readonly HashSet<ILogAppender> Appenders = new HashSet<ILogAppender>();
        
        private static UnityLogListener unityLogListener;

        private static LogLevel rootLevel = LogLevel.UNDEFINED;
        public static LogManager Instance {get; private set;}
        static LogManager() {
            Instance = new LogManager();
        }
        public static LogLevel RootLevel
        {
            get
            {
                if (rootLevel == LogLevel.UNDEFINED)
                {
                    LoggerConfig config = Resources.Load<LoggerConfig>("Logger Config");
                    if (config != null)
                    {
                        rootLevel = config.GetDefaultLogLevel;
                    }
                }
                
                return rootLevel;
            }
            set
            {
                if (rootLevel == value)
                {
                    return;
                }
                
                rootLevel = value;
                OnLogLevelChanged?.Invoke(rootLevel);
            }
        }

        public static Action<LogLevel> OnLogLevelChanged = null;

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static void ListenOnUnityExceptions()
        {
            if(unityLogListener == null)
                unityLogListener = new UnityLogListener();
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static void UseRuntimeLogView()
        {
            if(Application.isBatchMode)
                return;
            
            if(Application.isEditor && !Application.isPlaying)
                return;
            
            var appender = RuntimeLogViewAppender.Factory.Create();
            AddAppender(appender);
        }
        
        public static ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILog GetLogger(Type type)
        {
            ILog currentLogger;
            if(Loggers.TryGetValue(type.Name, out currentLogger))
                return currentLogger;

            var logger = new DefaultLogger(type);

            foreach (var appender in Appenders)
                logger.AddAppender(appender);

            Loggers.Add(type.Name, logger);

            return logger;
        }

        public static void AddAppender(ILogAppender appender)
        {
            if (appender == null)
                return;

            Appenders.Add(appender);
            
            foreach (var entry in Loggers)
                entry.Value.AddAppender(appender);
        }

        public static void RemoveAppender(ILogAppender appender)
        {
            if(appender == null)
                return;
            
            Appenders.Remove(appender);
            
            foreach (var entry in Loggers)
                entry.Value.RemoveAppender(appender);
        }
        
        public static void RemoveAllAppenders()
        {
            var toRemove = new List<ILogAppender>(Appenders);
            foreach (var appender in toRemove) RemoveAppender(appender);
        }

        public static Version GetVersion()
        {
            var textAsset = Resources.Load<TextAsset>("Logging/version");
            var version = new Version(textAsset.text);
            return version;
        }

        public void Dispose()
        {
            RemoveAllAppenders();
        }
        
        public static void DisposeInstance() {
            if (Instance != null) {
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}