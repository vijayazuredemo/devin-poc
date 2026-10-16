using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Dtos;
using TaskManager.Api.Models;
using TaskManager.Api.Repositories;

namespace TaskManager.Api.Controllers;

// [ApiController] automatically returns 400 Bad Request when validation attributes on the request DTOs fail.
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _repository;

    public TasksController(ITaskRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
    {
        return Ok(_repository.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = _repository.GetById(id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public ActionResult<TaskItem> Create(CreateTaskRequest request)
    {
        var task = _repository.Add(request.Title, request.Description);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TaskItem> Update(int id, UpdateTaskRequest request)
    {
        var task = _repository.Update(id, request.Title, request.Description, request.IsCompleted);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _repository.Delete(id) ? NoContent() : NotFound();
    }
}
