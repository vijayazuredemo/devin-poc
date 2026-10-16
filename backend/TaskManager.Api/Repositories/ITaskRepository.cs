using TaskManager.Api.Models;

namespace TaskManager.Api.Repositories;

public interface ITaskRepository
{
    IEnumerable<TaskItem> GetAll();
    TaskItem? GetById(int id);
    TaskItem Add(string title, string? description);
    TaskItem? Update(int id, string title, string? description, bool isCompleted);
    bool Delete(int id);
}
