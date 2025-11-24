using Microsoft.EntityFrameworkCore;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Emails;
using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.DAL.Repositories;

public class EmailConfirmationRepository(AppDbContext context) : IEmailConfirmationRepository
{
    public Task<EmailConfirmationToken?> GetByTokenAsync(string token)
    {
        return context.EmailConfirmationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token && !x.Used);
    }

    public async Task AddAsync(EmailConfirmationToken token)
    {
        await context.EmailConfirmationTokens.AddAsync(token);
    }

    public Task SaveChangesAsync() => context.SaveChangesAsync();
}