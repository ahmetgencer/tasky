using MediatR;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;

namespace Tasky.Application.Comments.Commands;

public sealed record DeleteCommentCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteCommentHandler(
    IRepository<Comment> repo,
    ICurrentUser current
) : IRequestHandler<DeleteCommentCommand, bool>
{
    public async Task<bool> Handle(DeleteCommentCommand r, CancellationToken ct)
    {
        var c = await repo.GetByIdAsync(r.Id, ct);
        if (c is null) return false;
        if (current.UserId is null || current.UserId != c.AuthorId) return false;

        await repo.DeleteAsync(c, ct);
        return true;
    }
}
