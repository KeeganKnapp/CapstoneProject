using System;

namespace CapstoneMaui.Platforms.iOS.Services;

public class IOSLoggerService : CapstoneMaui.Core.Services.AbstractLoggerService<IOSLoggerService>
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


    }


}
