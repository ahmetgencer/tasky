namespace Tasky.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private User() { } // EF
    public User(string email, string passwordHash)
    {
        Email = email;
        PasswordHash = passwordHash;
    }
}
