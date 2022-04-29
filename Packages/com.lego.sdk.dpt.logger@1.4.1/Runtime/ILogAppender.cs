namespace LEGO.Logger
{
    public interface ILogAppender
    {
		// The appender will filter out anything below the LevelFilter
    	LogLevel LevelFilter { get; set; }

        void Print(ILogMessage message);
    }
}