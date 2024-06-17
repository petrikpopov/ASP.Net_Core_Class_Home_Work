using System.Net;
using System.Net.Mail;

namespace ASP_.Net_Core_Class_Home_Work.Services.Email;

public class GmailServise : IEmailService
{
    public readonly IConfiguration _configuration;
    public GmailServise(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public void Send(MailMessage mailMessage)
    {
        var smtp = _configuration.GetSection("smtp");
        String? host = smtp.GetSection("host").Value;
        String? email = smtp.GetSection("email").Value;
        String? password = smtp.GetSection("password").Value;
        Boolean ssl = smtp.GetValue<Boolean>("ssl");
        Int32 port = smtp.GetValue<Int32>("port");
        if (host == null || email == null || password == null)
        {
            throw new Exception("Config error");
        }
       
        using SmtpClient smtpClient = new()
        {
            Host = host,
            Port = port,
            EnableSsl = ssl,
            Credentials = new NetworkCredential(email, password)
        };
        mailMessage.From = new MailAddress(email);
        if (string.IsNullOrEmpty(mailMessage.Subject))
        {
            mailMessage.Subject = "ASP-212 site notification";
        }
        smtpClient.Send(mailMessage);
    }
}