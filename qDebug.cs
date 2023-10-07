namespace qASIC
{
    public static partial class qDebug
    {
        public const string DEFAULT_COLOR_TAG = "default";
        public const string ERROR_COLOR_TAG = "error";
        public const string WARNING_COLOR_TAG = "warning";
        
        public static event Action<string, Color>? OnLogWithColor;
        public static event Action<string, string>? OnLogWithColorTag;

        public static void Log(object? message) =>
            Log(message, DEFAULT_COLOR_TAG);

        public static void LogWarning(object? message) =>
            Log(message, WARNING_COLOR_TAG);

        public static void LogError(object? message) =>
            Log(message, ERROR_COLOR_TAG);

        public static void Log(object? message, string colorTag) =>
            OnLogWithColorTag?.Invoke(message?.ToString() ?? "NULL", colorTag);

        public static void Log(object? message, Color color) =>
            OnLogWithColor?.Invoke(message?.ToString() ?? "NULL", color);
    }
}