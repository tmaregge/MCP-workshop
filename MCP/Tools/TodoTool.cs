using System.ComponentModel;
using ModelContextProtocol.Server;

namespace TodoApi.MCP.Tools;

[McpServerToolType]
public class TodoTool()
{
    [McpServerTool, Description("Run this to test if the server is working")]
    public static string Echo(string message) => $"hello {message}";
}