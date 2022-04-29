using System;
using UnityEngine;

namespace LEGO.Logger
{
    /// <summary>
    /// Listens for Exceptions trown by the program and make sure they are logged through the Logger framework.
    /// </summary>
    internal sealed class UnityLogListener
    {
        private readonly ILog logger = LogManager.GetLogger<UnityLogListener>();

        public UnityLogListener()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void HandleLog(string message, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    logger.Error(message);
                    break;
                
                case LogType.Assert:
                    break;
                
                case LogType.Warning:
                    logger.Warn(message);
                    break;
                
                case LogType.Log:
                    logger.Verbose(message);
                    break;
                
                case LogType.Exception:
                    logger.Fatal(message + "\n" + stackTrace);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        ~UnityLogListener()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

}

