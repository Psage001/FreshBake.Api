using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService ( IConfiguration config )
    {
        _config = config;
    }

    public async Task SendPasswordResetEmail ( string toEmail, string resetLink )
    {
        var emailSettings = _config.GetSection("Email");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            emailSettings["FromName"],
            emailSettings["FromAddress"]
        ));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Reset your FreshBake password";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <p>You requested a password reset.</p>
                <p><a href='{resetLink}'>Click here to reset your password</a></p>
                <p>This link expires in 30 minutes. If you didn't request this, you can ignore this email.</p>
            "
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            emailSettings["SmtpHost"],
            int.Parse(emailSettings["SmtpPort"]!),
            MailKit.Security.SecureSocketOptions.StartTls
        );
        await client.AuthenticateAsync(emailSettings["SmtpUser"], emailSettings["SmtpPass"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}