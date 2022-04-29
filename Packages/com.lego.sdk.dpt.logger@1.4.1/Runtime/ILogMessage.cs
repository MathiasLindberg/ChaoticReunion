using System;

namespace LEGO.Logger
{
    public interface ILogMessage
    {
        ILog Logger { get; }
        string Text { get; }
        LogLevel Level { get; }
        Exception Exception { get; }
        DateTime TimeStamp { get;  }
        int LineNumber { get; }
        string ClassName { get; }
        string SourceDescription { get; }
    }
}