using CapstoneMaui.Core.Services;
using global::Android.Util;


using Log = global::Android.Util.Log;
using Toast = global::Android.Widget.Toast;
using Application = global::Android.App.Application;

namespace CapstoneMaui.Platforms.Android.Services
{

    public class AndroidLoggerService : ILoggerService
    {
        public void Log(string message, string level = "info")
        {
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
        }
    }

}
