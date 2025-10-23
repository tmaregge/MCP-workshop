using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _repository;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoRepository repository, ILogger<TodoController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all todos
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetAll()
    {
        var todos = await _repository.GetAllAsync();
        return Ok(todos);
    }

    /// <summary>
    /// Get a specific todo by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetById(Guid id)
    {
        var todo = await _repository.GetByIdAsync(id);
        if (todo == null)
        {
            return NotFound(new { message = $"Todo with ID {id} not found" });
        }

        return Ok(todo);
    }

    /// <summary>
    /// Get todos by creator
    /// </summary>
    [HttpGet("creator/{creator}")]
    public async Task<ActionResult<IEnumerable<Todo>>> GetByCreator(string creator)
    {
        var todos = await _repository.GetByCreatorAsync(creator);
        return Ok(todos);
    }

    /// <summary>
    /// Get todos by state
    /// </summary>
    [HttpGet("state/{state}")]
    public async Task<ActionResult<IEnumerable<Todo>>> GetByState(TodoState state)
    {
        var todos = await _repository.GetByStateAsync(state);
        return Ok(todos);
    }

    /// <summary>
    /// Create a new todo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Todo>> Create([FromBody] Todo todo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdTodo = await _repository.CreateAsync(todo);
        return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
    }

    /// <summary>
    /// Update an existing todo
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<Todo>> Update(Guid id, [FromBody] Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest(new { message = "ID in URL does not match ID in body" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedTodo = await _repository.UpdateAsync(todo);
        if (updatedTodo == null)
        {
            return NotFound(new { message = $"Todo with ID {id} not found" });
        }

        return Ok(updatedTodo);
    }

    /// <summary>
    /// Delete a todo
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = $"Todo with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Check if a todo exists
    /// </summary>
    [HttpHead("{id}")]
    public async Task<ActionResult> Exists(Guid id)
    {
        var exists = await _repository.ExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        return Ok();
    }
}
