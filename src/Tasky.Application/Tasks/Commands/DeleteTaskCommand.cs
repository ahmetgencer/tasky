using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;

namespace Tasky.Application.Tasks.Commands;

public sealed record DeleteTaskCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteTaskHandler(IRepository<TaskItem> repo) : IRequestHandler<DeleteTaskCommand, bool>
{
    public async Task<bool> Handle(DeleteTaskCommand r, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(r.Id, ct);
        if (entity is null) return false;
        await repo.DeleteAsync(entity, ct);
        return true;
    }
}
