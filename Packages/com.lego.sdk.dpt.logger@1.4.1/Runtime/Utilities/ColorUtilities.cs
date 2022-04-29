using UnityEngine;

namespace LEGO.Logger.Utilities
{
	public static class ColorUtility 
	{
		public static Color GetLogLevelColor(LogLevel logLevel, Color defaultColor)
		{
			switch (logLevel)
			{
				case LogLevel.VERBOSE:
					return Color.white;
				
				case LogLevel.DEBUG:
					return From255(42,116,153);
				
				case LogLevel.INFO:
					return From255(128,152,3);
				
				case LogLevel.WARN:
					return From255(168,132,14);
				
				case LogLevel.ERROR:
					return From255(217,104,13);
				
				case LogLevel.FATAL:
					return From255(206,48,47);
				
				case LogLevel.OFF:
					return Color.white;
				
				default:
					return defaultColor;
			}
		}

		/// <summary>
		/// values below 0, and above 255 will result in undefined behaviour.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static Color From255(int r, int g, int b)
		{
			var red = r == 0 ? r : r / 255f;
			var green = g == 0 ? g : g / 255f;
			var blue = b == 0 ? b : b / 255f;
			return new Color(red, green, blue);
		}
	}
}
