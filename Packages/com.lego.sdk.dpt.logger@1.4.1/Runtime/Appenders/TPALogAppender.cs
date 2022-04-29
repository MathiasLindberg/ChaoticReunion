using System;
using UnityEngine;

namespace LEGO.Logger.Appenders
{
    public class TPALogAppender : ILogAppender
    {
        public LogLevel LevelFilter { get; set; }

        private bool isTPAEnabled = true;

        public void Print(ILogMessage message)
        {
            if (!isTPAEnabled)
                return;

            if (message.Level < LevelFilter)
                return;

            var msg = MessageTextSafeForTPA(message.Level.ToString().ToUpper() + "\t" + message.SourceDescription + "\t" + message.Text);
            try 
            {
#if TPA_ENABLED && !UNITY_ANDROID
            TPAUnitySDK.TPA.Log(msg);
#endif
            } 
            catch (FormatException e)
            {
                Debug.LogWarning(string.Format("Format exception for message {0}: {1}", msg, e));
            }
        }

        static string MessageTextSafeForTPA(string msg)
        {
            // TPA uses System.String.Format (with placeholders like {0}). Braces are escaped by doubling them, so {{0}} will end up as {0}
            msg = msg.Replace("{", "{{");
            msg = msg.Replace("}", "}}");
            return msg;
        }
    }
}