using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Dtos;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title must be 100 characters or fewer.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description must be 500 characters or fewer.")]
    public string? Description { get; set; }
}
