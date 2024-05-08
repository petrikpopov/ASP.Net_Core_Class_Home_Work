using System.Diagnostics;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Data;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using ASP_.Net_Core_Class_Home_Work.Models;
using ASP_.Net_Core_Class_Home_Work.Models.Home.FrontendForm;
using ASP_.Net_Core_Class_Home_Work.Services.Email;
using ASP_.Net_Core_Class_Home_Work.Services.Hash;
using ASP_.Net_Core_Class_Home_Work.Services.Kdf;
using ASP_.Net_Core_Class_Home_Work.Services.Random;

using ASP_.Net_Core_Class_Home_Work.Views.Home;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;

public class HomeController : Controller
{
    //Інжекція сервісів(залежностей) - запит у контейнера на передачу посилань на відповідь обьети. Найбільш рекомендований спосіб інжекціі - через конструктор. Це дозволяє , по-перше, оголосити поля як незмінні (redonly), та по-друге, унеможливити створення обьектів без передачі залежностей. У стартовому проєкті інжекція демонструється на сервіси логування (_logger).
    private readonly ILogger<HomeController> _logger;
    // Створюємо поле для посилання на сервіс
    private readonly IHashService _hashService;

    private readonly IRandomService _iRandomService;
    // Додаємо до конструктора параметр - залежність і зберігаємо іі у тілі
    private readonly DataContext _dataContext;
    private readonly DataAccessor _dataAccessor;
    private readonly IKdfService _kdfService;
    private readonly IEmailService _emailService;
    public HomeController(ILogger<HomeController> logger, IHashService hashService, IRandomService randomService,  DataAccessor dataAccessor, IKdfService kdfService,IEmailService _emailService)
    {
        _logger = logger; // збереження переданніх залежностей , що іх
        _hashService = hashService;// передасть контейнер прі створенні контроллера
        _iRandomService = randomService;
        _dataAccessor = dataAccessor;
        _kdfService = kdfService;
        this._emailService = _emailService;
    }

    public IActionResult ConfirmEmail(string id)
    {
        /*
         * Ідея Вasic-автентифікаціі -- розділення логіну та поролю через 
         dXNlckBpLnVhOnF3ZTEyMw==
         */
        string email, code;
        try
        {
            string data =
                System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(id)); // user@i.ua:qwe123
            string[] parts = data.Split(':', 2); // [ user@i.ua, qwe123]
            email = parts[0]; // user@i.ua
            code = parts[1]; // qwe123
            ViewData["result"] = _dataAccessor.UserDao.ConfirmEmail(email, code)
                ? "Пошта підтверджена"
                : "Помилка підтвердження пошти";
        }
        catch
        {
            ViewData["result"] = "Данні не розпізнані!";
        }
       
        return View();
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }
    public ViewResult Admin()
    {
        return View();
    }
    public IActionResult Singup(@ASP_.Net_Core_Class_Home_Work.Models.Home.Singup.SignupFormModel? formModel)
    {
        @ASP_.Net_Core_Class_Home_Work.Models.Home.Singup.SignupPageModel pageModel = new()
        {
            FormModel = formModel
        };
        if (formModel?.HasData ?? false)
        {
            pageModel.ValidationErrors = _ValidateSignUpModel(formModel);
            if (pageModel.ValidationErrors.Count == 0)
            {
                string code = Guid.NewGuid().ToString()[..6];
                MailMessage mailMessage = new()
                {
                    Subject = "Підтвердження пошти",
                    IsBodyHtml = true,
                    Body = " <p>Для підтверждення пошти введіть на сайті код</p>"+$"<h2 style= 'color: orange'>{code}</h2>"
                    
                };
                mailMessage.To.Add(formModel.UserEmail);
                try
                {
                    _emailService.Send(mailMessage); 
                    String salt = _iRandomService.RandomSalt(5);
                    _dataAccessor.UserDao.SignUp(new ()
                    {
                        Name = formModel.UserName,
                        Email = formModel.UserEmail,
                        EmailConfirmCode = code,
                        Birthdate = formModel.UserBirthdate,
                        AvaratUrl = formModel.SavedAvaterFileName,
                        Salt = salt,
                        DerivedKey = _kdfService.DerivedKey(salt,formModel.Password)
                    });
                }
                catch (Exception e)
                {
                    pageModel.ValidationErrors["email"] = "Не вдалося надіслати Email!";
                    _logger.LogInformation(e.Message);
                }
               
            }
        }
       // _logger.LogInformation(Directory.GetCurrentDirectory());
        return View(pageModel);
    }

    private Dictionary<string, string> _ValidateSignUpModel(
        @ASP_.Net_Core_Class_Home_Work.Models.Home.Singup.SignupFormModel? model)
    {
        Dictionary<string, string> result = new();
        if (model == null)
        {
            result["model"] = "Model is null";
        }
        else
        {
            if (string.IsNullOrEmpty((model.UserName)))
            {
                result[nameof(model.UserName)] = "User Name should not be empty";
            }

            if (string.IsNullOrEmpty(model.UserEmail))
            {
                result[nameof(model.UserEmail)] = "User Email should not be empty";
            }

            if (model.UserBirthdate == default(DateTime))
            {
                result[nameof(model.UserBirthdate)] = "User Birthday should not be empty";
            }

            if (string.IsNullOrEmpty(model.Password) || 
                model.Password.Length <= 3 || !model.Password.Any(char.IsLetter) || 
                !model.Password.Any(char.IsDigit))
            {
                result[nameof(model.Password)] = "Password is not correct!";
            }

            if (model.Password != model.Repeat)
            {
                result[nameof(model.Repeat)] = "Repeat password is not equal to password!";
            }

            if (model.Agreement == false)
            {
                result[nameof(model.Agreement)] = "We have to argee to the terms!";
            }

            if (model.UserAvatar != null)
            {
                // є файл аналізуємо його 
                // дізнажмось рожшірення фалу
                int dotPosition = model.UserAvatar.FileName.LastIndexOf('.');
                if (dotPosition == -1)
                {
                    result[nameof(model.UserAvatar)] = "File without extention not allowed";
                }

                String ext = model.UserAvatar.FileName.Substring(dotPosition);
                String path = Directory.GetCurrentDirectory() + "/wwwroot/img/avatars/";
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
                if (!allowedExtensions.Contains(ext.ToLower()))
                {
                    result[nameof(model.UserAvatar)] = "File extension is not allowed";
                }
                _logger.LogInformation(ext);
                String fileName;
                String pathName;
                do
                {
                    fileName = _iRandomService.RandomNameFile(6) + ext;
                    pathName = path + fileName;
                } while (System.IO.File.Exists(pathName));
                using var stream = System.IO.File.OpenWrite(pathName);
                model.UserAvatar.CopyTo(stream);
                model.SavedAvaterFileName = fileName;
            }
        }

        return result;
    }

    [HttpPost]  // атрибут дозволяє запит лише POST-методом.
    public JsonResult FrontendForm([FromBody] FrontendFormInput input)
    {
        FrontendFormOut frontendFormOut = new()
        {
            Code = 200,
            Message = $"{input.UserName}------{input.UserEmail}------{input.Birthday}----{input.english}-----{input.german}-----{input.armenian}----{input.french}\n"
        };
        _logger.LogInformation(frontendFormOut.Message);
        return Json(frontendFormOut);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}