using Tasky.Domain.Entities;
using Tasky.Domain.Enums;

namespace Tasky.Application.Tasks.DTOs;

public sealed record TaskDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskyStatus Status,
    Priority Priority,
    Guid? AssigneeId,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static TaskDto FromEntity(TaskItem e) => new(
        e.Id, e.ProjectId, e.Title, e.Description, e.Status, e.Priority, e.AssigneeId, e.DueDate, e.CreatedAt, e.UpdatedAt
    );
}
