using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoDbContext _context;

    public TodoRepository(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Todo>> GetAllAsync()
    {
        return await _context.Todos
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await _context.Todos.FindAsync(id);
    }

    public async Task<IEnumerable<Todo>> GetByCreatorAsync(string creator)
    {
        return await _context.Todos
            .Where(t => t.Creator == creator)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Todo>> GetByStateAsync(TodoState state)
    {
        return await _context.Todos
            .Where(t => t.State == state)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Todo> CreateAsync(Todo todo)
    {
        todo.Id = Guid.NewGuid();
        todo.CreatedAt = DateTime.UtcNow;
        todo.UpdatedAt = null;

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        return todo;
    }

    public async Task<Todo?> UpdateAsync(Todo todo)
    {
        var existingTodo = await _context.Todos.FindAsync(todo.Id);
        if (existingTodo == null)
        {
            return null;
        }

        todo.UpdatedAt = DateTime.UtcNow;
        todo.CreatedAt = existingTodo.CreatedAt; // Preserve original creation time

        _context.Entry(existingTodo).CurrentValues.SetValues(todo);
        await _context.SaveChangesAsync();

        return existingTodo;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            return false;
        }

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Todos.AnyAsync(t => t.Id == id);
    }
}
