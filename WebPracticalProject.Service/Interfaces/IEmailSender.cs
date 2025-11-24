namespace WebPracticalProject.Service.Interfaces;

public interface IEmailSender
{
    Task SendConfirmationEmailAsync(string email, string token);
}