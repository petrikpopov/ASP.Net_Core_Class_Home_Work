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
    // [NonAction]
    // public void OnActionExecuting(ActionExecutingContext context)
    // {
    //     var identity = User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthSessionMiddleware));
    //     identity ??=  User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthTokenMiddleware));
    //     this.isAuthentication = identity != null;
    //     String? useRole = identity?.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.Role)?.Value; 
    //     this.iaAdmin = "Admin".Equals(useRole);
    //    
    // }

    [HttpGet("all/{id}")]
    public List<Room> GetRooms(string id)
    {
        _logger.LogWarning($"auth={isAuthentication}, admin={iaAdmin}");
        //var location = _dataAccessor._ContentDao.GetLocationBySlug(id);
        List<Room> rooms;
        {
            rooms = _dataAccessor._ContentDao.GetRooms(id);
        }
        return rooms;
    }

    [HttpGet("{id}")]
    public Room GetRoom([FromRoute] string id /*[FromQuery] int? year, [FromQuery] int? moth*/)
    {
        var room = _dataAccessor._ContentDao.GetRoomBySlug(id);
        if (room == null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            return null;
        }

        room.Reservations = room.Reservations.Where(r => r.Date >= DateTime.Today).ToList();
        /*room.Reservations.ForEach(r =>
        {
            r.Room = null!;
            r.User = null!;
        });*/
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

    [HttpPost("reserve")]
    public string ReserveRoom([FromBody]ReserveRoomFormModel model)
    {
        // Todo: перевірити що кімната вільна на дату бронування
        // А також дата бронування не є у минулому
        // Первинна автентифікація - за сесією
        // Якщо вона є, то іде работа з робота Разор
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            // Якщо немає первинної авторизаціі - перевіряємо токен
            var identity= User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthTokenMiddleware));
            if (identity == null)
            {
                // якщо авторизація не пройдена то повідомлення а Items
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? "";
            }
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
    // [NonAction] // Якщо неможна зробити метод private то позначаємо атрибутом [NonAction]
    // public void OnActionExecuted(ActionExecutedContext context)
    // {
    //    
    // }
}