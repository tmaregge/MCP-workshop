using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace TodoApi.MCP.Prompts;

[McpServerPromptType]
public static class TodoPrompts
{
    [McpServerPrompt, Description("Creates a prompt to list todos in a compact format")]
    public static ChatMessage CompactList([Description("The user who created the todo")] string creator) =>
        new(ChatRole.User, $"Please list all todos created by {creator} using the ListTodos tool. Format the response as a bullet list containing only the title of each todo. Only include todos that are not done.");
}