using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Application.Comments.DTOs;
using Tasky.Domain.Entities;

namespace Tasky.Application.Comments.Queries;

public sealed record GetCommentsForTaskQuery(Guid TaskId) : IRequest<IReadOnlyList<CommentDto>>;

public sealed class GetCommentsForTaskHandler(IRepository<Comment> repo)
    : IRequestHandler<GetCommentsForTaskQuery, IReadOnlyList<CommentDto>>
{
    public async Task<IReadOnlyList<CommentDto>> Handle(GetCommentsForTaskQuery r, CancellationToken ct)
    {
        var list = await repo.ListAsync(c => c.TaskId == r.TaskId, ct);
        return [.. list.OrderByDescending(c => c.CreatedAt).Select(CommentDto.FromEntity)];
    }
}
