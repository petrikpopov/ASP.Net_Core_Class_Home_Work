using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
using Microsoft.AspNetCore.Mvc;
[Route("api/location")]
[ApiController]
public class LocationController: ControllerBase
{
    private readonly DataAccessor _dataAccessor;

    public LocationController(DataAccessor dataAccessor)
    {
        _dataAccessor = dataAccessor;
    }

    [HttpGet("{id}")]
    public List<Location> DoGet(string id)
    {
        return _dataAccessor._ContentDao.GetLocations(id);
    }

    [HttpPost]
    public string DoPost([FromForm] LocationPostModel model)
    {
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
            _dataAccessor._ContentDao.AddLocation(name:model.Name,description:model.Description,CategoryId:model.CategoryId,Stars:model.Stars, PhotoUrl:fileName);
            Response.StatusCode = StatusCodes.Status201Created;
            return "Ok";
        }
        catch (Exception e)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Error";
        }
    }

    public class  LocationPostModel
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public Guid CategoryId { set; get; }
        public int Stars { set; get; }
        public IFormFile Photo { set; get; }

    }
}