using CapstoneMaui.Core.Services;

namespace CapstoneMaui.Platforms.Android.Services
{

    public class AndroidLoggerService : AbstractLoggerService
    {
        public override void Log(object? sender, string message, string level = "info")
        {
		    //add emojis to final string for level
            string emoji = level switch
            {
                "error" => "❌",
                "warning" => "⚠️",
                "info" => "ℹ️",
                "debug" => "🐞",
                "trace" => "🔍",
                _ => "ℹ️"
            };
            var senderName = sender.ToString().Split('.').Last();
            message = $"[{senderName}] {emoji} {message}";	
#if ANDROID
            switch (level.ToLower())
            {
                case "error":
                    global::Android.Util.Log.Error("AndroidLoggerService", message);
                    break;
                case "warning":
                    global::Android.Util.Log.Warn("AndroidLoggerService", message);
                    break;
                case "debug":
                    global::Android.Util.Log.Debug("AndroidLoggerService", message);
                    break;
                case "trace":
                    global::Android.Util.Log.Verbose("AndroidLoggerService", message);
                    break;
                case "info":
                default:
                    global::Android.Util.Log.Info("AndroidLoggerService", message);
                    break;
            }
            LogToSystem(message);
            OnLog(message);
        }

#endif
    }

}
