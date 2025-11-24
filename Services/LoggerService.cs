using System;
using Microsoft.Extensions.Logging;
using CapstoneBlazorApp.Services.Abstractions;
namespace CapstoneBlazorApp.Services
{
	public class LoggerService : AbstractLoggerService
	{
		public override void Log(object? sender, string message, string level = "info")
		{
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

			OnLog(message);
		}
	}
}
