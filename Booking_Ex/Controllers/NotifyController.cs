using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Services.Email;
using Microsoft.AspNetCore.Mvc;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
[Route("api/notify")]
[ApiController]
public class NotifyController : ControllerBase
{
    private readonly IEmailService _emailService;

    public NotifyController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Object DoGet()
    {
        // надсилаємо листа
        try
        {
           
            MailMessage mailMessage = new()
            {
                IsBodyHtml = true,
                Body = "<h1>Шановний користувач!</h1>" +
                       "<p style='color:green'>Вітаємо на сайті <a href='http://localhost:5002/Content'>Booking</a></p>"
            };
            mailMessage.To.Add(new MailAddress("proviryalovich@gmail.com"));
            _emailService.Send(mailMessage);
            return new { Sent = "OK" };
        }
        catch (Exception e)
        {
            return new { Error = e.Message };
        }
       
    }
}
// Google Account
// In your Google Account, you can see and manage your info, activity, security options, and privacy preferences to make Google work better for you.

/* PobOTa 3 E-mail
1. SMTP - Simple Mail Transfer Protocol
протокол надсилання E-mail
[Email-server] ---- >[Email-server]
SMTP/send   IMAP\receive
[Backend]   [User-mailbox]
Для роботи з E-mail необхідно створити поштову скриню (mailbox)
на поштовому сервісі, який підтримує SMTP (у бажаному тарифному плані)
Далі на прикладі Gmail

2. Організація налаштувань
!!! Паролі від пошти (та інших ресурсів) не можна розміщувати на
відкритих репозиторіях і не бажано навіть на закритих.
Задля цього створюються два файли конфігурації - один реальний, який
вилучається з репозиторію, інший - демонстраційний, який є в репозиторії
та містить приклад заповнення конфігурації.
- У файл gitignore додаємо запис emailconfig. json
- створюємо у проєкті файл emailconfig. json
- створюємо у проєкті файл emailconfig.sample.json
= бажано перевірити - зробити коміт і переконатись, що лише один файл
є у репозитopii - emailconfig.sample.json

3. Налаштовуємо або дізнаємось налаштування SMTP (на прикладі Gmail)
- переходимо до обікового запису (https://myaccount.google.com/)
- (зліва) Безпека - (по центу) Вхід - двоєтапна аутентифікація
-- Пароли пріложеній - створити новий - копіюєм до emailconfig. json
-- Після заповнення emailconfig. json робимо його копію до  emailconfig.sample.json і видаляюмо в ній усі персональні дані
  (як правило замість них залишають примітки на кшталт **CHANGE**)
  -- Додаємо нову конфігурацію до загальгних налаштувань у Program.cs 
  
*/
