using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using ASP_.Net_Core_Class_Home_Work.Middleware;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
using Microsoft.AspNetCore.Mvc;
[Route("api/location")]
[ApiController]
public class LocationController: BackendController
{
    private readonly DataAccessor _dataAccessor;
    private readonly ILogger<CategoryController> _logger;
    public LocationController(DataAccessor dataAccessor,  ILogger<CategoryController> _logger)
    {
        _dataAccessor = dataAccessor;
        this._logger = _logger;
    }

    [HttpGet("{id}")]
    public List<Location> DoGet(string id)
    {
        String? userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        bool isAdmin = "Admin".Equals(userRole);
        return _dataAccessor._ContentDao.GetLocations(id, isAdmin);
    }

    [HttpPut]
    public string DoPut([FromForm] LocationPostModel model)
    {
        if (GetAdminAuthMessage() is String mess)
        {
            return mess;
        }
        if (model.CategoryId==default(Guid))
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return "Missing required parameter: 'location-id' ";
        }
        // перевіряємо чи є така location
        Location? location = model.LocationID==null ? null : _dataAccessor._ContentDao.GetLocationById(model.LocationID.Value);
        if (location == null)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return $"Parameter 'location-id' ({model.CategoryId}) belongs to no entity. ";
        }

        if (!string.IsNullOrEmpty(model.Name))
        {
            location.Name = model.Name;
        }
        if (!string.IsNullOrEmpty(model.Description))
        {
            location.Description = model.Description;
        }
        if (!string.IsNullOrEmpty(model.Slug))
        {
            location.Slug = model.Slug;
        }

        if (model.Stars > 0)
        {
            location.Stars = model.Stars;
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

                if (!String.IsNullOrEmpty((location.PhotoUrl)))
                {
                    try
                    {
                        System.IO.File.Delete(path + location.PhotoUrl);
                    }
                    catch
                    {
                        _logger.LogWarning(location.PhotoUrl+"not deleted");
                    }
                     
                } 

                location.PhotoUrl = fileName;

            }
            catch (Exception ex)

            {
                _logger.LogWarning(ex.Message);
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return "Error uploading file";

            }

        }
        _dataAccessor._ContentDao.UpdateLocation(location);
        Response.StatusCode = StatusCodes.Status200OK;
        return "Update";
    }


    [HttpPost]
    public string DoPost([FromForm] LocationPostModel model)
    {
        if (GetAdminAuthMessage() is String mes)
        {
            return mes;
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
            _dataAccessor._ContentDao.AddLocation(
                name:model.Name,
                description:model.Description,
                CategoryId:model.CategoryId,
                Stars:model.Stars,
                PhotoUrl:fileName, 
                slug:model.Slug);
            Response.StatusCode = StatusCodes.Status201Created;
            return "Ok";
        }
        catch (Exception e)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Error";
        }
    }
    
    [HttpDelete("{id}")]
    public string DoDelete(Guid id)
    {
        if (GetAdminAuthMessage() is String msg)
        {
            return msg;
        }
        _dataAccessor._ContentDao.DeleteLocation(id);
        Response.StatusCode = StatusCodes.Status202Accepted;
        return "Ok";
    }
    
    [HttpPatch]
    public Location? DoPatch(string slug)
    {
        return _dataAccessor._ContentDao.GetLocationBySlug(slug);
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
            _dataAccessor._ContentDao.RestoreLocation(Guid.Parse(id!));
        }
        catch
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Empty or invalid id";
        }

        Response.StatusCode = StatusCodes.Status202Accepted;
        return "RESTORE works with id = " + id;
    }
    public class  LocationPostModel
    {
        [FromForm(Name="location-name")]
        public string Name { set; get; }
        
        [FromForm(Name="location-description")]
        public string Description { set; get; }
        
        [FromForm(Name="category-id")]
        public Guid CategoryId { set; get; }
        
        [FromForm(Name="location-slug")]
        public string Slug { set; get; }
        [FromForm(Name="location-stars")]
        public int Stars { set; get; }
        [FromForm(Name="location-photo")]
        public IFormFile Photo { set; get; }
        
        [FromForm(Name="location-id")]
        public Guid? LocationID { set; get; }
        

    }
}