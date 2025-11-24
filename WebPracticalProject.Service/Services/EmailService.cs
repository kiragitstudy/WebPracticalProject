using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Service.Services;

public class EmailService(IOptions<SmtpOptions> options, IConfiguration configuration) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendConfirmationEmailAsync(string email, string token)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Каталог музыкальных инструментов", _options.From));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Подтверждение регистрации";

        var confirmationLink =
            $"{configuration["AppSettings:BaseUrl"]}/Account/ConfirmEmail?token={Uri.EscapeDataString(token)}";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <h2>Добро пожаловать в Каталог музыкальных инструментов!</h2>
                <p>Пожалуйста, подтвердите ваш email, перейдя по ссылке ниже:</p>
                <a href='{confirmationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                    Подтвердить email
                </a>
                <p>Если вы не регистрировались на нашем сайте, просто проигнорируйте это письмо.</p>"
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_options.Username, _options.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
