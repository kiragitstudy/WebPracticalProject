using Microsoft.EntityFrameworkCore;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.DAL.Repositories;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<Guid> CreateAsync(CreateUserArgs a, CancellationToken ct)
    {
        var e = new User { 
            Email=a.Email, 
            PasswordHash=a.PasswordHash, 
            DisplayName=a.DisplayName, 
            Role=a.Role, 
            EmailConfirmed=a.IsEmailConfirmed 
        };
        await db.Users.AddAsync(e, ct);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateUserArgs a, CancellationToken ct)
    {
        var e = await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("user");
        if (a.DisplayName is not null) e.DisplayName = a.DisplayName;
        if (a.Role.HasValue) e.Role = a.Role.Value;
        if (a.EmailConfirmed.HasValue) e.EmailConfirmed = a.EmailConfirmed.Value;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var stub = new User { Id = id };
        db.Attach(stub);
        db.Remove(stub);
        await db.SaveChangesAsync(ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        db.Users.AsNoTracking().AnyAsync(x => x.Email == email, ct);

    public Task SetLastLoginAsync(Guid id, DateTimeOffset at, CancellationToken ct) =>
        db.Users.Where(x => x.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.LastLoginAt, at), ct);
    
    
    public Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, ct);

    public async Task SetGoogleIdAsync(Guid userId, string googleId, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return;
        user.GoogleId = googleId;
        await db.SaveChangesAsync(ct);
    }
    
    public async Task<(IReadOnlyList<User>, int)> GetPagedAsync(int page,int size,CancellationToken ct)
    {
        var q = db.Users.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page-1)*size).Take(size).ToListAsync(ct);
        return (items,total);
    }
}