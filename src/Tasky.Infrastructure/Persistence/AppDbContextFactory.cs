using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tasky.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("TASKY_CS")
            ?? "Server=localhost,1433;Database=Tasky;User ID=sa;Password=P@ssw0rd!;Encrypt=True;TrustServerCertificate=True";
        var opt = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(cs).Options;
        return new AppDbContext(opt);
    }
}
