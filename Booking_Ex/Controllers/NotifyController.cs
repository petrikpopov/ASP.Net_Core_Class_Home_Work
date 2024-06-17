using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Services.Email;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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

