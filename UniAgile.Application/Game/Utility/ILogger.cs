namespace UniAgile.Game
{
    public enum LogType
    {
        Log,
        Warning,
        Error
    }

    public interface ILogger
    {
        void Log(string  category,
                 string  message,
                 LogType logType);

        string CreateCategory(string featureName,
                              string specifier);
    }

    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger,
                               string       featureName,
                               string       specifier,
                               string       message,
                               LogType      logType = LogType.Log)
        {
            logger.Log(logger.CreateCategory(featureName, specifier), message, logType);
        }
    }
}