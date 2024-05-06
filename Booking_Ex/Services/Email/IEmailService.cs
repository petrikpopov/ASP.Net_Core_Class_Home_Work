using System.Net.Mail;

namespace ASP_.Net_Core_Class_Home_Work.Services.Email;

public interface IEmailService
{
    void Send(MailMessage message);
}