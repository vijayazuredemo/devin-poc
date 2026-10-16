using System.Collections.Concurrent;
using TaskManager.Api.Models;

namespace TaskManager.Api.Repositories;

// Stores tasks in memory only; all data is lost when the app restarts.
public class InMemoryTaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<int, TaskItem> _tasks = new();
    private int _nextId;

    public IEnumerable<TaskItem> GetAll() =>
        _tasks.Values.OrderBy(t => t.Id).ToList();

    public TaskItem? GetById(int id) =>
        _tasks.TryGetValue(id, out var task) ? task : null;

    public TaskItem Add(string title, string? description)
    {
        var task = new TaskItem
        {
            Id = Interlocked.Increment(ref _nextId),
            Title = title.Trim(),
            Description = description?.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        _tasks[task.Id] = task;
        return task;
    }

    public TaskItem? Update(int id, string title, string? description, bool isCompleted)
    {
        if (!_tasks.TryGetValue(id, out var task))
        {
            return null;
        }

        task.Title = title.Trim();
        task.Description = description?.Trim();
        task.IsCompleted = isCompleted;
        return task;
    }

    public bool Delete(int id) => _tasks.TryRemove(id, out _);
}
