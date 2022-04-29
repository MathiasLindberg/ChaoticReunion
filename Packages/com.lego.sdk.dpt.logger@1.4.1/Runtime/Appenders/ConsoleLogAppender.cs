using System;

namespace LEGO.Logger.Appenders
{
    /// <summary>
    /// Appender utilizing <see cref="System.Console.WriteLine()"/> directing all messages directly to the unity default log.
    /// </summary>
    public sealed class ConsoleLogAppender : ILogAppender
    {
        private static readonly object SyncObject = new object();
        
        public LogLevel LevelFilter { get; set; }
        public void Print(ILogMessage message)
        {
            if (message.Level < LevelFilter)
                return;
            
            //Unity internally writes to the editor log file. We don't want duplicates.
            if(message.Logger.Name == typeof(UnityLogListener).FullName)
                return;
            
            lock (SyncObject)
            {
                var msg = string.Format("[{0}]\t{1:d/M/yy HH:mm:ss.fff}\t{2}", message.ClassName, DateTime.Now, message.Text);
                Console.WriteLine(msg);
            }
        }
    }
}