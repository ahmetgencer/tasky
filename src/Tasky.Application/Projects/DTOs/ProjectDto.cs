using Tasky.Domain.Entities;

namespace Tasky.Application.Projects.DTOs;

public sealed record ProjectDto(Guid Id, string Name, Guid OwnerId, DateTime CreatedAt)
{
    public static ProjectDto FromEntity(Project e) => new(e.Id, e.Name, e.OwnerId, e.CreatedAt);
}
