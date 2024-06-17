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

public class CategoryController : BackendController
{
    private readonly DataAccessor _DataAccessor;
    private readonly ILogger<CategoryController> _logger;
    public CategoryController(DataAccessor dataAccessor, ILogger<CategoryController> _logger)
    {
        _DataAccessor = dataAccessor;
        this._logger = _logger;
    }
    
    [HttpGet]
    public List<Category> DoGet()
    {
       return _DataAccessor._ContentDao.GetCategories(includeDelete: iaAdmin);
    }

    [HttpPost]
    public string DoPost([FromForm]CategoryPostModel model)
    {
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
        if (model.CategoryId==null || model.CategoryId==default(Guid))
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return "Missing required parameter: 'category-id' ";
        }
        Category? category = _DataAccessor._ContentDao.GetCategoryById(model.CategoryId.Value);
        if (category == null)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return $"Parameter 'category-id' ({model.CategoryId.Value}) belongs to no entity. ";
        }
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
        if (model.Photo != null) 
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
    public Object DoOther()
    {
        if (Request.Method == "RESTORE")
        {
            return DoRestore();
        }

        Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        return "Method Not Allowed";
    }
  
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