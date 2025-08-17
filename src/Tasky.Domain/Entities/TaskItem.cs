using Tasky.Domain.Enums;

namespace Tasky.Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProjectId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public TaskyStatus Status { get; private set; } = TaskyStatus.Todo;
    public Priority Priority { get; private set; } = Priority.Medium;
    public Guid? AssigneeId { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private TaskItem() { } // EF
    public TaskItem(Guid projectId, string title)
    {
        ProjectId = projectId;
        Title = title;
    }

    public void Update(string title, string? description, TaskyStatus status, Priority priority, Guid? assigneeId, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        AssigneeId = assigneeId;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
