using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Data;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using ASP_.Net_Core_Class_Home_Work.Models;
using ASP_.Net_Core_Class_Home_Work.Models.Home.FrontendForm;
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
    public HomeController(ILogger<HomeController> logger, IHashService hashService, IRandomService randomService,  DataAccessor dataAccessor, IKdfService kdfService)
    {
        _logger = logger; // збереження переданніх залежностей , що іх
        _hashService = hashService;// передасть контейнер прі створенні контроллера
        _iRandomService = randomService;
        _dataAccessor = dataAccessor;
        _kdfService = kdfService;
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
                String salt = _iRandomService.RandomSalt(5);
                _dataAccessor.UserDao.SignUp(new ()
                {
                    Name = formModel.UserName,
                    Email = formModel.UserEmail,
                    Birthdate = formModel.UserBirthdate,
                    AvaratUrl = formModel.SavedAvaterFileName,
                    Salt = salt,
                    DerivedKey = _kdfService.DerivedKey(salt,formModel.Password)
                });
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