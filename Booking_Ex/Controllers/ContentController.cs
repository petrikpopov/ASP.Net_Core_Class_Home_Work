using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Category;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Index;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Location;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Room;
using Microsoft.AspNetCore.Mvc;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;

public class ContentController : Controller
{
    public DataAccessor dataAccessor;
    public ContentController(DataAccessor dataAccessor)
    {
        this.dataAccessor = dataAccessor;
    }

    // GET
    public IActionResult Index()
    {
        ContentIndexPageModel model = new()
        {
            categories = dataAccessor._ContentDao.GetCategories()
        };
        return View(model);
    }
    
    public IActionResult Category([FromRoute]String id)
    {
        var ctg = dataAccessor._ContentDao.GetCategoryBySlug(id);
        
        return ctg==null? 
            View("NotFound"): View(new ContentCategoryPageModel()
                {
                    Category = ctg,
                    Locations = dataAccessor._ContentDao.GetLocations(ctg.Slug!)
                }
            );
    }
    public IActionResult Location([FromRoute]String id)
    {
        var loc = dataAccessor._ContentDao.GetLocationBySlug(id);
        
        return loc==null? 
            View("NotFound"):
            View(new ContentLocationPageModel()
            {
                Location = loc,
                Rooms = dataAccessor._ContentDao.GetRooms(loc.Id)
            });
    }

    public IActionResult Room([FromRoute] string id, [FromQuery]int? year, [FromQuery]int? moth)
    {
        var room = dataAccessor._ContentDao.GetRoomBySlug(id);
        
        return room==null? 
            View("NotFound"):
            View(new RoomPageModel()
            {
               Room = room,
               Year = year ?? DateTime.Today.Year,
               Month = moth ?? DateTime.Today.Month
            });
    }
}