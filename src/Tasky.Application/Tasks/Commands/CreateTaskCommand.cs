using FluentValidation;
using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;
using Tasky.Domain.Enums;

namespace Tasky.Application.Tasks.Commands;

public sealed record CreateTaskCommand(Guid ProjectId, string Title, string? Description = null,
                                       TaskyStatus Status = TaskyStatus.Todo,
                                       Priority Priority = Priority.Medium,
                                       Guid? AssigneeId = null,
                                       DateTime? DueDate = null) : IRequest<Guid>;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateTaskHandler(IRepository<TaskItem> repo) : IRequestHandler<CreateTaskCommand, Guid>
{
    public async Task<Guid> Handle(CreateTaskCommand r, CancellationToken ct)
    {
        var entity = new TaskItem(r.ProjectId, r.Title);
        entity.Update(r.Title, r.Description, r.Status, r.Priority, r.AssigneeId, r.DueDate);
        await repo.AddAsync(entity, ct);
        return entity.Id;
    }
}
