using BepInEx.Logging;
using System.Runtime.CompilerServices;

public class LogCore
{
    public static ManualLogSource logger = null;

    public LogCore(ManualLogSource logger_) {
        logger = logger_;
    }

    public static void LogDebug(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
    {
        logger.LogDebug(logString(data, i, member));
    }
    public static void LogError(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
    {
        logger.LogError(logString(data, i, member));
    }
    public static void LogFatal(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
    {
        logger.LogFatal(logString(data, i, member));
    }
    public static void LogInfo(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
    {
        logger.LogInfo(logString(data, i, member));
    }
    public static void LogMessage(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
    {
        logger.LogMessage(logString(data, i, member));
    }
    public static void LogWarning(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "")
    {
        logger.LogWarning(logString(data, i, member));
    }

    private static string logString(object data, [CallerLineNumber] int i = 0, [CallerMemberName] string member = "") {
        return string.Format("{0} [Line: {1} | Method {2}]", data, i, member);
    }
}
