using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManager.Api.Models;

namespace TaskManager.Api.Tests;

// Integration tests: each test spins up the real API in memory and calls it over HTTP.
public class TasksApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TasksApiTests()
    {
        // A new factory per test gives each test its own empty in-memory store.
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task<TaskItem> CreateTaskAsync(string title = "Test task", string? description = "Details")
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { title, description });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskItem>())!;
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsOkWithEmptyList()
    {
        var response = await _client.GetAsync("/api/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Empty(tasks!);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedWithLocation()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { title = "Buy milk", description = "2 liters" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(task);
        Assert.True(task!.Id > 0);
        Assert.Equal("Buy milk", task.Title);
        Assert.Equal("2 liters", task.Description);
        Assert.False(task.IsCompleted);
        Assert.NotEqual(default, task.CreatedAt);
        Assert.EndsWith($"/api/tasks/{task.Id}", response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_WithMissingTitle_ReturnsBadRequest(string title)
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { title });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithTooLongTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/tasks", new { title = new string('a', 101) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsTask()
    {
        var created = await CreateTaskAsync("Read a book");

        var response = await _client.GetAsync($"/api/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal("Read a book", task!.Title);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/tasks/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsUpdatedTask()
    {
        var created = await CreateTaskAsync();

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}",
            new { title = "Updated", description = "New details", isCompleted = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal("Updated", task!.Title);
        Assert.Equal("New details", task.Description);
        Assert.True(task.IsCompleted);
        Assert.Equal(created.CreatedAt, task.CreatedAt);
    }

    [Fact]
    public async Task Update_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/tasks/999", new { title = "Nope", isCompleted = false });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WithEmptyTitle_ReturnsBadRequest()
    {
        var created = await CreateTaskAsync();

        var response = await _client.PutAsJsonAsync($"/api/tasks/{created.Id}", new { title = "", isCompleted = false });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContentAndRemovesTask()
    {
        var created = await CreateTaskAsync();

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{created.Id}");
        var getResponse = await _client.GetAsync($"/api/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/tasks/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
