using System;
using Microsoft.Extensions.Logging;


namespace CapstoneBlazorApp.Services.Abstractions
{

public abstract class AbstractLoggerService
{
	public required EventHandler<string> OnLogMessage;


	//log with detail for errors, warnings, info, debug, trace
	public abstract void Log(object? sender, string message, string level = "info");

	public void LogToSystem(string message)
	{
		//_logger.LogInformation(message);
	}

	public void OnLog(string message)
	{
		OnLogMessage?.Invoke(this, message);
	}
}
}
