using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace LEGO.Logger.Appenders
{
    /// <summary>
    /// Redirects LEGO logging calls to Unity's Debug.Log/LogWarning etc console.
    /// Do NOT utilize this along with the <see cref="UnityLogListener"/>, as this will result in recursive functionality.
    /// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class UnityEngineLogAppender : ILogAppender
	{
		public LogLevel LevelFilter { get; set; }

        public UnityEngineLogAppender ()
		{
            #if UNITY_EDITOR
            LevelFilter = LogLevel.WARN;
            #else
            //When running on a physical device we want debug log as well
            LevelFilter = LogLevel.DEBUG;
            #endif
		}

		public void Print (ILogMessage message)
		{
			if (message.Level < LevelFilter)
				return;

            var msg = string.Format("[{0}]\t{1:d/M/yy HH:mm:ss.fff}\t{2}", message.ClassName, DateTime.Now, message.Text);

			if (message.Level == LogLevel.DEBUG || message.Level == LogLevel.INFO) {
				Debug.Log (msg);
			}
			else if (message.Level == LogLevel.WARN) {
				Debug.LogWarning(msg);
			}
			else if (message.Level >= LogLevel.ERROR) {
				Debug.LogError(msg);
			}

            if (message.Exception != null)
                Debug.LogException (message.Exception);
		}
	}
}
