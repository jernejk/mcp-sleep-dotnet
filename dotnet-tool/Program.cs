using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Information);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();

[McpServerToolType]
public static class SleepTools
{
    [McpServerTool]
    [Description("Sleep for the given number of milliseconds, then return a JSON result.")]
    public static async Task<string> Sleep(
        [Description("Milliseconds to pause (e.g., 1000 = 1 second)")] int ms)
    {
        await Task.Delay(Math.Max(0, ms));
        return $"{{\"slept\":{ms}}}";
    }
}
