using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using ASP_.Net_Core_Class_Home_Work.Middleware;
using ASP_.Net_Core_Class_Home_Work.Migrations;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Location;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Room;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/room")]
[ApiController]
public class RoomController: BackendController
{
    private readonly DataAccessor _dataAccessor;
      private readonly ILogger _logger;
      private bool isAuthentication;
      private bool iaAdmin;

    public RoomController(DataAccessor _dataAccessor, ILogger<RoomController>logger)
    {
        this._dataAccessor = _dataAccessor;
        _logger = logger;
    }
    
    [HttpGet("{id}")]
    public List<Room> DoGet(string id)
    {
        String? userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        bool isAdmin = "Admin".Equals(userRole);
        return _dataAccessor._ContentDao.GetRooms(id, isAdmin);
    }
    
    [HttpGet("all/{id}")]
    public List<Room> GetRooms(string id)
    {
        List<Room> rooms;
        {
            rooms = _dataAccessor._ContentDao.GetRooms(id);
        }
        return rooms;
       
    }
    
    [HttpGet("{id}")]
    public Room GetRoom([FromRoute] string id)
    {
        var room = _dataAccessor._ContentDao.GetRoomBySlug(id);
        if (room == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        room.Reservations = room.Reservations.Where(r => r.Date >= DateTime.Today).ToList();
       
        return room;
    }
    [HttpPost]
    public string DoPost( [FromForm] RoomFormModel model)
    {
        if (GetAdminAuthMessage() is String msg)
        {
            return msg;
            
        }
        try
        {
            String? fileName = null;
            if (model.Photo != null)
            {
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
            }
            if (fileName == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return "File Image required";
            }
            _dataAccessor._ContentDao.AddRoom(
                name: model.Name,
                description: model.Description,
                photoUrl: fileName,
                slug: model.Slug,
                locationId: model.LocationyId,
                stars: model.Stars,
                Dailyprice: model.DailyPrice);
            Response.StatusCode = StatusCodes.Status201Created;
            return "Added";
        }
        catch (Exception ex)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return ex.Message;
        }
  
    }
    
    //
    [HttpPut]
    public string DoPut([FromForm] RoomPostModel model)
    {
        if (GetAdminAuthMessage() is String mess)
        {
            return mess;
        }
        if (model.LocationID==default(Guid))
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return "Missing required parameter: 'room-id' ";
        }

        Room room = model.RoomId == null ? null : _dataAccessor._ContentDao.GetRoomById(model.RoomId.Value);
        if (room == null)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return $"Parameter 'room-id' ({model.LocationID}) belongs to no entity. ";
        }
        if (!string.IsNullOrEmpty(model.Name))
        {
            room.Name = model.Name;
        }
        if (!string.IsNullOrEmpty(model.Description))
        {
            room.Description = model.Description;
        }
        if (!string.IsNullOrEmpty(model.Slug))
        {
            room.Slug = model.Slug;
        }
        if (!double.IsNaN(model.DailyPrice) && !double.IsInfinity(model.DailyPrice))
        {
            room.DailyPrice = model.DailyPrice;
        }

        if (model.Stars > 0)
        {
            room.Stars = model.Stars;
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
               
                if (!String.IsNullOrEmpty((room.PhotoUrl)))
                {
                    try
                    {
                        System.IO.File.Delete(path + room.PhotoUrl);
                    }
                    catch
                    {
                        _logger.LogWarning(room.PhotoUrl+"not deleted");
                    }
                     
                } 
               

                room.PhotoUrl = fileName;

            }
            catch (Exception ex)

            {
                _logger.LogWarning(ex.Message);
                Response.StatusCode = StatusCodes.Status400BadRequest;

                return "Error uploading file";

            }

        }
        _dataAccessor._ContentDao.UpdateRoom(room);
        Response.StatusCode = StatusCodes.Status200OK;
        return "Update";
    }

    [HttpPost("reserve")]
    public string ReserveRoom([FromBody]ReserveRoomFormModel model)
    {
        
        if (!base.isAuthentication)
        {
            
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return "Authorization failed!";
        }
       
         if ( base.claims?.First(c => c.Type == ClaimTypes.Sid)?.Value != model.UserId.ToString())
         {
             Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
             return "Ambiguous user identification!";
         }
        Reservation? reservation = _dataAccessor._ContentDao.GetReservation(model.RoomId, model.Date);
        
        if (reservation != null)
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return "Room is reserved for requested date!";
        }
        if (model.Date < DateTime.Today) 
        {
            Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return "Cannot reserve for a past date!";
        }
        try
        {
            _dataAccessor._ContentDao.ReserveRoom(model);
            Response.StatusCode = StatusCodes.Status201Created;
            return "Reserved";
        }
        catch (Exception e)
        {
           _logger.LogError(e.Message);
           Response.StatusCode = StatusCodes.Status400BadRequest;
           return e.Message;
        }
    }
    
    [HttpGet("reserve")]
    public List<Reservation> GetReservations(string id)
    {
        Room? room;
        lock (_logger)
        {
            room = _dataAccessor._ContentDao.GetRoomBySlug( id);
        }
        return room?.Reservations;
    }
    
    [HttpPatch]
    public Room? DoPatch(string slug)
    {
        return _dataAccessor._ContentDao.GetRoomBySlug(slug);
    }
    
    [HttpDelete("reserve")]
    public string DropReservation([FromQuery] Guid reservId)
    {
        if (reservId == default)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Guid parse error!";
        }

        try
        {
            _dataAccessor._ContentDao.DeleteReservation(reservId);
            Response.StatusCode = StatusCodes.Status202Accepted;
            return "";
        }
        catch (Exception e)
        {
           _logger.LogError(e.Message);
           Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
           return e.Message;
        }
        
    }
    
    [HttpDelete("{id}")]
    public string DoDelete(Guid id)
    {
        if (GetAdminAuthMessage() is String msg)
        {
            return msg;
        }
        _dataAccessor._ContentDao.DeleteRoom(id);
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
            _dataAccessor._ContentDao.RestoreRoom(Guid.Parse(id!));
        }
        catch
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            return "Empty or invalid id";
        }

        Response.StatusCode = StatusCodes.Status202Accepted;
        return "RESTORE works with id = " + id;
    }
    public class RoomPostModel
    {
        [FromForm(Name="room-name")]
        public string Name { set; get; }
        
        [FromForm(Name="room-description")]
        public string Description { set; get; }
        
        [FromForm(Name="room-id")]
        public Guid? RoomId { set; get; }
        
        [FromForm(Name="room-slug")]
        public string Slug { set; get; }
        
        [FromForm(Name="room-stars")]
        public int Stars { set; get; }
        
        [FromForm(Name="room-photo")]
        public IFormFile Photo { set; get; }
        
        [FromForm(Name="location-id")]
        public Guid? LocationID { set; get; }
        
        [FromForm(Name="room-price")]
        public Double DailyPrice { set; get; }
    }
   
}