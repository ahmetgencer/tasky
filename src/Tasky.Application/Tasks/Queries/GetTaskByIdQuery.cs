using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Application.Tasks.DTOs;
using Tasky.Domain.Entities;

namespace Tasky.Application.Tasks.Queries;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskDto?>;

public sealed class GetTaskByIdHandler(IRepository<TaskItem> repo) : IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    public async Task<TaskDto?> Handle(GetTaskByIdQuery r, CancellationToken ct)
    {
        var e = await repo.GetByIdAsync(r.Id, ct);
        return e is null ? null : TaskDto.FromEntity(e);
    }
}
