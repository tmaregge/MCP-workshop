using System.Net.Http.Json;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService
{
    private readonly HttpClient _httpClient;

    public TodoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Todo>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Todo>>("api/todo") ?? new List<Todo>();
    }

    public async Task<Todo?> GetByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<Todo>($"api/todo/{id}");
    }

    public async Task<Todo?> CreateAsync(Todo todo)
    {
        var response = await _httpClient.PostAsJsonAsync("api/todo", todo);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Todo>();
    }

    public async Task<Todo?> UpdateAsync(Todo todo)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/todo/{todo.Id}", todo);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Todo>();
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/todo/{id}");
        response.EnsureSuccessStatusCode();
    }
}
