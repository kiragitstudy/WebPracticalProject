using WebPracticalProject.Domain.Emails;
using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.DAL.Interfaces;

public interface IEmailConfirmationRepository
{
    Task<EmailConfirmationToken?> GetByTokenAsync(string token);
    Task AddAsync(EmailConfirmationToken token);
    Task SaveChangesAsync();
}