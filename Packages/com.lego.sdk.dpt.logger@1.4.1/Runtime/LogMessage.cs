using System;
using System.Linq;

namespace LEGO.Logger
{
    internal sealed class LogMessage : ILogMessage
    {
        public LogMessage(ILog logger, string text, LogLevel level, DateTime timeStamp, Exception exception = null)
        {
            Logger = logger;
            Text = text;
            Level = level;
            Exception = exception;
            TimeStamp = timeStamp;
            ClassName = logger.Name;
            
            //We cannot extract the stack trace in Release mode, so we only use this in the editor
            LineNumber = -1;

            #if UNITY_EDITOR
            //Shorten to class name, from full class name
            ClassName = ClassName.Split('.').Last();
            //Identify matching frame from the stacktrace 
            if (TryGetMatch(out var match, ClassName))
            {
                //Copy the line number
                LineNumber = match.GetFileLineNumber();
            }
            #endif
        }

        private static bool TryGetMatch(out System.Diagnostics.StackFrame match, string className)
        {
            var frames = new System.Diagnostics.StackTrace(true).GetFrames();
            foreach (var frame in frames)
            {
                if(frame.GetFileName() == null)
                    continue;
                
                if (!frame.GetFileName().Contains(className)) 
                    continue;
                
                match = frame;
                return true;
            }
            
            match = default;
            return false;
        }

        #region ILogMessage implementation

        public ILog Logger { get; private set; }

        public string Text { get; private set; }

        public LogLevel Level { get; private set; }

        public Exception Exception { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public int LineNumber { get; private set; }

        public string ClassName { get; private set; }

        public string SourceDescription {
            get 
            {
                if (LineNumber >= 0)
                    return ClassName + ":" + LineNumber;
                else
                    return ClassName;
            }
        }
        #endregion
    }
}
