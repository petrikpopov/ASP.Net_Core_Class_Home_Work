using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ASP_.Net_Core_Class_Home_Work.Models;
using ASP_.Net_Core_Class_Home_Work.Models.Home.DataBase;
using ASP_.Net_Core_Class_Home_Work.Models.Home.Ioc;
using ASP_.Net_Core_Class_Home_Work.Services.Hash;
using ASP_.Net_Core_Class_Home_Work.Models.Home.Intro;
using ASP_.Net_Core_Class_Home_Work.Models.Home.Razor;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;

public class HomeController : Controller
{
    //Інжекція сервісів(залежностей) - запит у контейнера на передачу посилань на відповідь обьети. Найбільш рекомендований спосіб інжекціі - через конструктор. Це дозволяє , по-перше, оголосити поля як незмінні (redonly), та по-друге, унеможливити створення обьектів без передачі залежностей. У стартовому проєкті інжекція демонструється на сервіси логування (_logger).
    private readonly ILogger<HomeController> _logger;
    // Створюємо поле для посилання на сервіс
    private readonly IHashService _hashService;

    private readonly IRandomService _iRandomService;
    // Додаємо до конструктора параметр - залежність і зберігаємо іі у тілі
    public HomeController(ILogger<HomeController> logger, IHashService hashService, IRandomService randomService)
    {
        _logger = logger; // збереження переданніх залежностей , що іх
        _hashService = hashService;// передасть контейнер прі створенні контроллера
        _iRandomService = randomService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Intro()
    {
        IntroPageModel introPageModel = new()
        {
            Tittle = "Вступ",
            HeaderPage = "Вступ до ASP",
            ContentAboutASP = " ASP — фреймворк .NЕТ для створення веб-застосунків різного типу. Найбільш сучасна модифікація ASP NET Core MVC.На початку слід звернути увагу на те, що багато речей виконуються через збіг імені. Тому слід уважно стежити за назвами класів методів файлів, змінних тощо. Використовується принцип шаблонізації (Layout), тобто всі сторінки формально є складовими частинами більш загального файлу <code>_Layout.cshtml</code>",
            TittleAboutFormUrl = " Щодо відносних форм URL",
            ContentAboutFormUrl = "Нехай сторінка знаходиться за адресою <code>https://localhost:5567/Home/Url</code>, тоді якщо у HTML-коді даної сторінки є відносні посилання, то їх повна адреса буде",
            Ul_li = new string[]{ @"<code>https://localhost:5567/Home/image.png</code>
            (посилання ""image.png"" додається після <u>останнього</u> слешу)",

            @"<code>https://localhost:5567/image.png</code>
            (посилання ""image.png"" додається після доменного імені - після
            першого одинарного слешу) !! при роботі з файлами HTML посилання
            буде формуватися від кореня диску (file://C:/image.png)",

            @"<code>https://image.png</code>
            (посилання ""image.png"" додається після протоколу (схеми) - після
            подвійного слешу). Такі форми вживають для збереження протоколу",

            @"<code>https://localhost:5567/Home/Url?image.png</code>
            (посилання ""image.png"" додається після всієї адреси). Вживається
            для доповнення посилання параматрами (імітація форми)",

            @"<code>https://localhost:5567/Home/Url#image.png</code>
            (посилання ""image.png"" додається після всієї адреси). Вживається
            для внутрішніх переходів - сторінка не перевантажується, а 
            прокручуютьсч",

            @"<code>https://localhost:5567/image.png</code>
            (посилання ""image.png"" додається до контекстного шляху 
            якщо він є). Вживається коли на одному сервері існує декілька
            сайтів і вони розділяються контекстним шляхом:
            https://site.com/site1/Home/URL
            https://site.com/site2Home/URL"
            }




        };
        return View(introPageModel);
    }

    public IActionResult AboutRazor()
    {
        RazorPageModel razorPageModel = new()
        {
            Tittle = "About Razor",
            HeaderPage = "Синтаксис Razor",
            AboutRazor = "Razor -технологія включення до складу НTМL розмітки засобів мови програмування С#.",
            H2Expressions = "Вирази",
            AboutExpressions = "Вирази (у програмуванні) — інструкції, які мають результат Коли йдеться про Razor мається на увазі виведення цього результату. &commat;(2 + 3) = @(2 + 3)<br/>\n    Для деяких випадків круглі дужки не абовіязкові (як правила для\n    виведення змінних), але можливі неправильні визначення між виразів, тому думки лишати бажано.",
            H2Instructions = "Інструкції",
            AboutInstruction = " Інструкції - узагальнені засоби мови програмування, які можуть бути як виразами (результати яких не потрібно виводити) так і безрезультатними літералами.\n    Razor - інструкції вміщують у фігурні дужки &commat;{...}. В середині дужок момуть бути довільні інструкції Mовo С#. Рекомендується вживати лише ті інструкції, які обробляють передані до представлення дані і не використовують сервіси та інші засоби проєкту."
        };
        return View(razorPageModel);
    }

    public IActionResult Ioc()
    {
        // користужмося інжектованним сервісом
        //ViewData - спеціальний обьект для передачі данних до представлення. Його ключі на типу ["hash"] можна створювати з довільними назвами.
        IocPageModel pageModel = new()
        {
            Tittle = "Декілько випадкових дайджестів:",
            HeaderPage = "Інверсія управління. Сервіси.",
            AboutIoc = "Ioc(Inversion of Control, Інверсія управління) - архітерктурна шаблон, згідно з яким задачі управління жіттєвим циклом обьектів перекладаються на спеціальний модуль (інжектор, контейнер залежностей, провайдер).Життєвий цикл обьекта: CRUD. практично це означає , що замість операторів <code> new / delete</code> будуть відповідні звернення до контейнеру.\n Робота у стилі Ioc полягає у наступних кроках.",
            LiStepWork_Ioc = new string[]{"Створення сервісу - класу, щт надає необхідну функціональність.","Реєстрація всіх сервісів у контейнері(інжекторі).","Інжекція сервісів у інших обьектів , яким вони потрібні"},
            CreateService = "Стровення сервісу слід виконувати з дотриманням принципу DIP (з Solid ) - принціпу інверсіі залежностей.\n Є три терміни:",
            TypeInversion = new string[]{"інверсія управління","інжекція залежносте","інверсія залежностей. \n Всі вони є різними хоч і стосуються близьких задач.\n  Принцип DIP \"не створювати залежності від реалізацій , створювати від \"абстракций\" практычно радить при сроворенні сервісу почати з інтерфейсу і ліше потім створювати клас. Це дозволить замінювати класи , але не змінювати інтерфейсів "},
            ExampleCreateService = "На прикладі створення сервісу гешування:",
            LiExampleCreateService = new string []{"(одноразово) Створюємо папку Services у корені проекту","Оскільки сервіси - це щонайменше два файли (клас та інтерфейс), для кожного сервісу також створюються папки (у данному разі - Hash)","Створюємо інтерфейс IHashService та класс Md5HashService","Рееструэм сервіс(див Program.cs)","Інжетуємо сервіс (див HomeController)","Імітуємо задачу: необхідно перейти на інший геш-фдгоритм SHA","OCP (з SOLID) \"доповлює але не змінює --- створює новий клас ShaHashService у папаці Servicec/Hash","Program.cs змінює зареєстрованний сервіс. !!!! перехід між різними сервісами(за іх наявності) відбувається одним рядком зміни реєстраціі. Ані контроллер, ані представлення змін не зазнають."},
            SingleHash = _hashService.Digest("Hello,World")
            
        };
        for (int i = 0; i<5;i++)
        {
            string str = (i+10050).ToString();
            pageModel.Hashes[str] = _hashService.Digest(str);
        }

        return View(pageModel);
    }

    public IActionResult DataBase()
    {
        DataBasePageModel dataBasePageModel = new()
        {
            Tittle = "Connect to DB",
            HeaderPage = "Connect to DB",
            H2ListNuGet = "Використовуємо Entity Framework відповідно ми повинні піключити такі пакети, переходімо NuGet:",
            UsingPackages = new string[]{"Microsoft.EntityFrameworkCore - ядро фреймворку, основні засоби","Microsoft.EntityFrameworkCore.Tools - інструменти управління міграціями","Драйвер БД: у залежності від СУБД - для MSSQL: Microsoft.EntityFrameworkCore.SqlServer\nMySQL: Pomelo.EntityFrameworkCore.MySql"},
            Structure = "Структура",
            LiStructure = new []{"Створюємо в корені проєкту папку <b>Data</b>, у ній - клас <b>DataContext</b>","Реалізуємо рядок підключення до БД. \n        MSSQL може створювати БД, відповідно можна створити рядок до поки що не існуючоі БД.\nMySQL  краще створити БД, але залишити порожньою. Рядки підключення вміщують до <b>appsettings.json</b> у спеціальну секцію ConnectionStrings ","Cтворюємо папку Entities в папке Data - у ній клас User","Клас DataContext наслідує клас DbContext. В класі DataContext ми повинні перевизначити методи <b>OnConfiguring()</b> та <b>OnModelCreating()</b>. В конструктор передати <b>(DbContextOptions options):base(options)</b>",
                "також нам треба додати: public DbSet<Data.Entities.User/> users { set; get; }","Переходімо в файл <b>Program.cs</b> рееструємо контекст данних:\n        1)string connectinString = builder.Configuration.GetConnectionString(\"LocalMySQL\");\n        2)MySqlConnection connection = new MySqlConnection(connectinString);\n        3)builder.Services.AddDbContext<DataContext/>(option => option.UseMySql(connection, ServerVersion.AutoDetect(connection))).","Використовую в терміналі команду <b>dotnet ef migrations add InitialCreate</b> для створення міграціі"}
        };
        return View(dataBasePageModel);
    }
    // Модель форми зазначаеться параметром методу, заповнення-автоматичне
    public IActionResult Model(Models.Home.Model.FormModel formModel)
    {
        //Модель представлення створюється і заповнюється самойтійно
        Models.Home.Model.PageModel pageModel = new()
        {
            TittlePage = "Model",
            TabHeader = "Моделі в ASP",
            AboutModel = "Моделі в ASP мають дещо видмінный змист вид класычного MVC. \n    Якщо в MVC Модель - постачальниик данних, архітектурний шаг программи, що відповідає за обіг данних , то в ASp модель- це засіб обігу, класи, що поєднують у собі набір даних, потрібний для того чи іншого \"спожівача\". Часто альтернативною назваю обьктів є DTO(Data Transfer Object) або структура.\n    Умовно розрізняють, моделі форм, моделі представлення(сторінок) та моделі даних(Entities).",
            H3ViewModel = "Моделі представлень",
            AboutViewMode = " Моделі представлень (Page Model, View Model) - структура даних, яка містить усі необхідні данні для формування (заповнення) певної сторінки ( представлення, view). В ідеальному випадку, представлення не містить нічого, окрім засобів розмітки/декорування даних з моделі(немає надписів, чисел, дат, тощо - все буруться з моделі).",
            H3ModelForm = "Моделі форм",
            AboutModelForm = " Моделі форм вікорістовуються для зворотних потоків даних- від користувача до бекенду. Оскільки дані частіше за все надсилаються за допомогою HTTP-форм, моделі носять іх назву.  Доволі часто форма проходить валідацію і, якщо дані не коректні, пропонується повторне заповнення форми. Але попередні дані підставляються до неї(окрім паролей).Відповідно, модель форм також входить до моделі представлення.",
            FormModel = formModel

        };
        // модель представлення передаеться аргументом View();
        return View(pageModel);
    }
    public IActionResult RandomService()
    {
        ViewData["OTP"] = _iRandomService.RandomOTP(6);
        
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


}