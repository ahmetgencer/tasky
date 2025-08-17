using Tasky.Domain.Entities;

namespace Tasky.Application.Comments.DTOs;

public sealed record CommentDto(Guid Id, Guid TaskId, Guid AuthorId, string Content, DateTime CreatedAt)
{
    public static CommentDto FromEntity(Comment c) => new(c.Id, c.TaskId, c.AuthorId, c.Content, c.CreatedAt);
}
