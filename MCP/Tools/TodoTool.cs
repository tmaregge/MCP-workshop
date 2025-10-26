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

    [McpServerTool, Description("Tool to create a new todo item")]
    public async Task<Todo> CreateTodo(
        string title,
        string creator,
        string? description = null,
        TodoState state = TodoState.Todo,
        Priority priority = Priority.Medium,
        DateTime? startDate = null,
        DateTime? dueDate = null,
        DateTime? endDate = null,
        List<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new McpException("Missing title parameter");
        if (string.IsNullOrWhiteSpace(creator)) throw new McpException("Missing creator parameter");
        
        var todo = new Todo
        {
            Title = title,
            Description = description,
            Creator = creator,
            State = state,
            Priority = priority,
            StartDate = startDate ?? DateTime.Now,
            DueDate = dueDate,
            EndDate = endDate,
            Tags = tags ?? []
        };

        return await todoRepo.CreateAsync(todo);
    }
}