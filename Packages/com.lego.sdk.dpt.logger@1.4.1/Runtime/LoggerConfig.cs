using LEGO.Logger;
using UnityEngine;

public class LoggerConfig : ScriptableObject
{
    [Tooltip("Sets the log level at startup of the project.\nLog level can still be edited through code at runtime")]
    [SerializeField] private LogLevel defaultLogLevel = LogLevel.ERROR;
    
    public LogLevel GetDefaultLogLevel
    {
        get { return defaultLogLevel; }
    }
}
