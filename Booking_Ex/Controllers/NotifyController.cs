using Microsoft.AspNetCore.Mvc;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
[Route("api/notify")]
[ApiController]
public class NotifyController : Controller
{

}
// Робота з Емаїл. 1) SMTP - протокол надсилання Емаїл. [SMTP - server]
                                                // SMTP / send    IMAP \receive
                                            // [Backend]                [User-mailbox]                
// Для роботи з E-Mail необхідно створити поштову скриню (mailBox) на почтовому сервісіс, який підтримує SMTP у бажаному торифному плані.
// Далі на прикладі Gmail 
// 2) Организация налаштувань !!!! Пароли від пошти та інших ресурсів не можна розміщувати на відкритих репозиторіях і не бажано на закритих.
// Для цього створюються 2 файли конфігураціі - один реальний , який вилучається з репозиторію, інший - демостраційний , який є в репозиторіі та містить приклад заповнення конфігураціі.
// У файл  .gitignore додаємо запис emailconfig.json
// Створюємо у проекті файл  emailconfig.json
// Створюємо у проекті файл emailconfig.sample.json
// Бажано перевірити - зробити коміт і переконатись, що 
