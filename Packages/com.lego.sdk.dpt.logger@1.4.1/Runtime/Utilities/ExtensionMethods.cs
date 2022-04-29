namespace LEGO.Logger.Utilities
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Contains method ignoring case sensitivity. Does not allocate memory.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool ContainsIgnoringCase(this string source, string content)
        {
            return source.IndexOf(content, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}