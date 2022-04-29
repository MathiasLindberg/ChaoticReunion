namespace LEGO.Logger.Appenders
{
    public sealed class Filter
    {
        public LogLevel LogLevel = LogLevel.VERBOSE;
        public string Context = string.Empty;
        public string Content = string.Empty;
        public bool ShowSource = false;
    }
}