namespace CapstoneMaui.Core.Services;

public interface ILoggerService
{
	//log with detail for errors, warnings, info, debug, trace
	public void Log(string message, string level = "info");
}