namespace Tasky.Domain.Entities;

public sealed class Comment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TaskId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Content { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Comment() { } // EF
    public Comment(Guid taskId, Guid authorId, string content)
    {
        TaskId = taskId;
        AuthorId = authorId;
        Content = content;
    }
}
