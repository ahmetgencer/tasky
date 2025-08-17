using FluentValidation;
using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;
using Tasky.Domain.Enums;

namespace Tasky.Application.Tasks.Commands;

public sealed record UpdateTaskCommand(
    Guid Id, string Title, string? Description, TaskyStatus Status, Priority Priority, Guid? AssigneeId, DateTime? DueDate
) : IRequest<bool>;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateTaskHandler(IRepository<TaskItem> repo) : IRequestHandler<UpdateTaskCommand, bool>
{
    public async Task<bool> Handle(UpdateTaskCommand r, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(r.Id, ct);
        if (entity is null) return false;
        entity.Update(r.Title, r.Description, r.Status, r.Priority, r.AssigneeId, r.DueDate);
        await repo.UpdateAsync(entity, ct);
        return true;
    }
}
