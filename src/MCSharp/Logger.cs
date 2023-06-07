namespace MCSharp.Core;

public class Logger {
    public static int Verbosity { get; set; } = 0;

    private static StreamWriter? stream;
    private static string loggerName = "";

    private Logger() { }

    public static bool StartLoggingSession(string name, string? loggingPath) {
        if(loggerName != "")
            throw new InvalidOperationException("Logging session has already started!");
        if(name == "")
            throw new ArgumentException("Logger name cannot be null!");
        
        loggerName = name;
        
        if(loggingPath != null)
            stream = new StreamWriter(new FileStream(loggingPath, FileMode.Create, FileAccess.Write));
        
        return true;
    }

    public static void log(string message, LogLevel level) {
        if((int)level <= Verbosity) {
            SetConsoleColor(level);
            Console.WriteLine(message);
            stream?.WriteLine(message);
            Console.ResetColor();
        }
    }

    private static void SetConsoleColor(LogLevel level) {
        switch(level) {
            case LogLevel.DEBUG:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case LogLevel.WARN:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;
            case LogLevel.ERROR:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogLevel.HINT:
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case LogLevel.INFO:
            default:
                Console.ResetColor();
                break;
        }
    }

    public enum LogLevel {
        DEBUG = int.MaxValue,
        HINT = 1,
        INFO = -10,
        WARN = -100,
        ERROR = int.MinValue
    }
}