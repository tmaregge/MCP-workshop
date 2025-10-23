namespace TodoApi.Models;

public class Todo
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required string Creator { get; set; }

    public TodoState State { get; set; } = TodoState.Todo;

    public Priority Priority { get; set; } = Priority.Medium;

    public DateTime? StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? EndDate { get; set; }

    public List<string> Tags { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
