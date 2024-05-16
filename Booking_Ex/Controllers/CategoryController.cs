using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using ASP_.Net_Core_Class_Home_Work.Middleware;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/category")]
[ApiController]
// IActionFilter - засоби для дій що виконуються до вільного Action-iв
public class CategoryController : BackendController
{
    private readonly DataAccessor _DataAccessor;
    private readonly ILogger<CategoryController> _logger;
    // private bool isAuthentication;
    // private bool iaAdmin;
    public CategoryController(DataAccessor dataAccessor, ILogger<CategoryController> _logger)
    {
        _DataAccessor = dataAccessor;
        this._logger = _logger;
    }
    // метод IActionFilter, що виконується ДО дій контролера (DoGet, DoPost,...)
    // [NonAction]
    // public void OnActionExecuting(ActionExecutingContext context)
    // {
    //    
    //     // У проекті є жві авторизаціі - через сесіі та через токени
    //     // Первинна авторизація за сесією 
    //     // Дані авторизаціі за токеном шукаємо за типом авторизації яку ми встановили як назва класу AuthTokenMiddleware
    //     var identity = User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthSessionMiddleware));
    //     identity ??=  User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthTokenMiddleware));
    //     this.isAuthentication = identity != null;
    //     String? useRole = identity?.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.Role)?.Value; 
    //     this.iaAdmin = "Admin".Equals(useRole);
    //    
    // }
    
    [HttpGet]
    public List<Category> DoGet()
    {
       return _DataAccessor._ContentDao.GetCategories(includeDelete: iaAdmin);
    }

    [HttpPost]
    public string DoPost([FromForm]CategoryPostModel model)
    {
       
        // var identity = User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthSessionMiddleware));
        // identity ??=  User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthTokenMiddleware));
        if (GetAdminAuthMessage() is String msg)
        {
           return msg;
        }
        try
        {
            String fileName = null;
            if (model.Photo != null)
            {
                String ext = Path.GetExtension(model.Photo.FileName);
                String path = Directory.GetCurrentDirectory() + "/wwwroot/img/Content/";
                    
                String pathName;
                do
                {
                    fileName = Guid.NewGuid() + ext;
                    pathName = path + fileName;
                } while (System.IO.File.Exists(pathName));
                using var stream = System.IO.File.OpenWrite(pathName);
                model.Photo.CopyTo(stream);
                                    
                

               
            }
            _DataAccessor._ContentDao.AddCategory(model.Name, model.Description, fileName,model.Slug);
            Response.StatusCode = StatusCodes.Status201Created;
            return "Ok";
        }
        catch(Exception exception)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Error";
        }
    }

    [HttpPut] //Update
    public string DoPut([FromForm] CategoryPostModel model)
    {
        if (GetAdminAuthMessage() is String msg)
        {
            return msg;
        }
        // перевіряємо CategoryId на наявність
        if (model.CategoryId==null || model.CategoryId==default(Guid))
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return "Missing required parameter: 'category-id' ";
        }
        // перевіряємо чи є така ктегорія
        Category? category = _DataAccessor._ContentDao.GetCategoryById(model.CategoryId.Value);
        if (category == null)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return $"Parameter 'category-id' ({model.CategoryId.Value}) belongs to no entity. ";
        }
        // оновлення даних - якщо немає данних то залішається попереднє значення
        if (!string.IsNullOrEmpty(model.Name))
        {
            category.Name = model.Name;
        }
        if (!string.IsNullOrEmpty(model.Description))
        {
            category.Description = model.Description;
        }
        if (!string.IsNullOrEmpty(model.Slug))
        {
            category.Slug = model.Slug;
        }
        if (model.Photo != null)   // передається новий файл - зберігаємо новий, видаляємо старий
        {

            try

            {

                String? fileName = null;

                String ext = Path.GetExtension(model.Photo.FileName);

                String path = Directory.GetCurrentDirectory() + "/wwwroot/img/content/";

                String pathName;

                do

                {

                    fileName = Guid.NewGuid() + ext;

                    pathName = path + fileName;

                }

                while (System.IO.File.Exists(pathName));

                using var stream = System.IO.File.OpenWrite(pathName);

                model.Photo.CopyTo(stream);
                // новий файл успішно завантажений - видаляємо старий

                if (!String.IsNullOrEmpty((category.PhotoUrl)))
                {
                    try
                    {
                        System.IO.File.Delete(path + category.PhotoUrl);
                    }
                    catch
                    {
                        _logger.LogWarning(category.PhotoUrl+"not deleted");
                    }
                     
                } 
                // зберігаємо нове ім'я 

                category.PhotoUrl = fileName;

            }
            catch (Exception ex)

            {
                _logger.LogWarning(ex.Message);
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return "Error uploading file";

            }

        }
        _DataAccessor._ContentDao.UpdateCategory(category);
        Response.StatusCode = StatusCodes.Status200OK;
        return "Update";
    }

    [HttpDelete("{id}")]
    public string DoDelete(Guid id)
    {
        if (GetAdminAuthMessage() is String msg)
        {
            return msg;
        }
        _DataAccessor._ContentDao.DeleteCategory(id);
        Response.StatusCode = StatusCodes.Status202Accepted;
        return "Ok";
    }
    
    // метод не позначений атрибутом буде викликано , якщо не знайдеться
    // необхідний з позначених. Це доволяє прийняти нестандартні запити

    public Object DoOther()
    {
        if (Request.Method == "RESTORE")
        {
            return DoRestore();
        }

        Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        return "Method Not Allowed";
    }
   // Другій НЕ позначений метод private щоб не було конфлікту
    private string DoRestore()
    {
        if (GetAdminAuthMessage() is String msg)
        {
            return msg;
        }
        string? id = Request.Query["id"].FirstOrDefault();
        try
        {
            _DataAccessor._ContentDao.RestoreCategory(Guid.Parse(id!));
        }
        catch
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Empty or invalid id";
        }

        Response.StatusCode = StatusCodes.Status202Accepted;
        return "RESTORE works with id = " + id;
    }
 
    // метод IActionFilter, що виконується ПІСЛЯ дій контролера (DoGet, DoPost,...)
    // [NonAction] // Якщо неможна зробити метод private то позначаємо атрибутом [NonAction]
    // public void OnActionExecuted(ActionExecutedContext context)
    // {
    //    
    // }
    public class CategoryPostModel
    {
        [FromForm(Name="category-name")]
        public string Name { set; get; }
        
        [FromForm(Name="category-description")]
        public string Description { set; get; }
        
        [FromForm(Name="category-slug")]
        public string Slug { set; get; }
        
        [FromForm(Name="category-photo")]
        public IFormFile? Photo { set; get; }
        
        [FromForm(Name="category-id")]
        public Guid? CategoryId { set; get; }

    }
}