using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace LEGO.Logger.Appenders
{
	/// <summary>
	/// File IO log appender. Appends all logs into a file on disk, within <see cref="UnityEngine.Application.persistenDataPath"/>.
	/// A new file is created for each session, with one backup being maintained.
	/// The backup will be appended with "-previous.log"
	/// </summary>
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class FileLogAppender : ILogAppender
	{
		private readonly StreamWriter outputStream;

		public FileLogAppender(string filenamePrefix)
		{
			var logFileName = Path.Combine(Application.persistentDataPath, filenamePrefix + ".log");
			var logFileNamePrevious = Path.Combine(Application.persistentDataPath, filenamePrefix + "-previous.log");
			
			if (File.Exists(logFileName))
				File.Copy(logFileName, logFileNamePrevious, true);
			    
			outputStream = new StreamWriter(logFileName, false /*append*/);
		}

        public LogLevel LevelFilter { get; set; }

		public void Print(ILogMessage message)
		{
			if (message.Level < LevelFilter)
				return;

			var msg = string.Format("[{0}]\t{1:d/M/yy HH:mm:ss.fff}\t{2}", message.ClassName, DateTime.Now, message.Text);

			if (message.Exception != null)
				outputStream.WriteLine(message.Exception + "\n" + message.Exception.StackTrace);
			else
				outputStream.WriteLine(message.Level + ": " + msg);

			outputStream.Flush();
		}
	}
}