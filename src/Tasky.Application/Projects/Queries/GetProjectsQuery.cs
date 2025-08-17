using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Application.Projects.DTOs;
using Tasky.Domain.Entities;

namespace Tasky.Application.Projects.Queries;

public sealed record GetProjectsQuery(Guid? OwnerId = null) : IRequest<IReadOnlyList<ProjectDto>>;

public sealed class GetProjectsHandler(
    IRepository<Project> repo,
    ICurrentUser current
) : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery r, CancellationToken ct)
    {
        var owner = r.OwnerId ?? current.UserId;
        var list = await repo.ListAsync(owner is null ? null : p => p.OwnerId == owner, ct);
        return [.. list.Select(ProjectDto.FromEntity)];
    }
}
