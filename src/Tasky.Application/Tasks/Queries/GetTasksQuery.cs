using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Application.Tasks.DTOs;
using Tasky.Domain.Entities;
using Tasky.Domain.Enums;

namespace Tasky.Application.Tasks.Queries;

public sealed record GetTasksQuery(Guid ProjectId, TaskyStatus? Status = null) : IRequest<IReadOnlyList<TaskDto>>;

public sealed class GetTasksHandler(IRepository<TaskItem> repo) : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery r, CancellationToken ct)
    {
        var list = await repo.ListAsync(t =>
            t.ProjectId == r.ProjectId && (!r.Status.HasValue || t.Status == r.Status), ct);

        return [.. list.Select(TaskDto.FromEntity)];
    }
}
