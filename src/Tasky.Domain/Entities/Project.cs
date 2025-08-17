namespace Tasky.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public Guid OwnerId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Project() { } // EF
    public Project(string name, Guid ownerId)
    {
        Name = name;
        OwnerId = ownerId;
    }
}
