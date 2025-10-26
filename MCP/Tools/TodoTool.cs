using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.MCP.Tools;

[McpServerToolType]
public class TodoTool(ITodoRepository todoRepo)
{
    [McpServerTool, Description("Run this to test if the server is working")]
    public static string Echo(string message) => $"hello {message}";

    [McpServerTool, Description("Tool to list all todos created by a specific user")]
    public async Task<IEnumerable<Todo>> ListTodos(string creator)
    {
        if (string.IsNullOrWhiteSpace(creator))
        {
            throw new McpException("Missing creator parameter");
        }
        
        return await todoRepo.GetByCreatorAsync(creator);
    }
}