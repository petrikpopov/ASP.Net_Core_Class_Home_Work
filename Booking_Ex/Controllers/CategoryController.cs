using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using Microsoft.Extensions.Primitives;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly DataAccessor _DataAccessor;

    public CategoryController(DataAccessor dataAccessor)
    {
        _DataAccessor = dataAccessor;
    }

    [HttpGet]
    public List<Category> DoGet()
    {
        String? useRole = HttpContext.User.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.Role)?.Value;
        bool iaAdmin = "Admin".Equals(useRole);
       return _DataAccessor._ContentDao.GetCategories(includeDelete: iaAdmin);
    }

    [HttpPost]
    public string DoPost([FromForm]CategoryPostModel model)
    {
        // Токени передаються за стандартною схемою - заголовком
        // Authorization: Bearer 1233242
        // де 1233242 - токен
        var authHeader = Request.Headers["Authorization"];
        if (authHeader == StringValues.Empty)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return "Authentication required";
        }

        string authValue = authHeader.First()!;
        if (!authValue.StartsWith("Bearer"))
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return "Bearer scheme required!";
        }

        string token = authValue[7..];
        Guid tokenId;
        try
        {
            tokenId = Guid.Parse(token);
        }
        catch
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return "Token invalid GUID required!";
        }

        User? user = _DataAccessor.UserDao.GetUserByToken(tokenId);
        if (user == null)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return "Token invalid or expired!";
        }

        if (user.Role != "Admin")
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return "Access to API forbidden!";
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

    [HttpDelete("{id}")]
    public string DoDelete(Guid id)
    {
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
    }
}