using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using ASP_.Net_Core_Class_Home_Work.Migrations;
using ASP_.Net_Core_Class_Home_Work.Models.Content.Room;
using Microsoft.EntityFrameworkCore;

namespace ASP_.Net_Core_Class_Home_Work.Data.DAL;

public class ContentDao
{
    private readonly DataContext _context;
    private readonly Object _dbLocker;
    public ContentDao(DataContext context, Object _dbLocker)
    {
        _context = context;
        this._dbLocker = _dbLocker;
    }

    public void AddCategory(string name, string description, string? photoUrl, string? slug=null)
    {
        if (slug == null)
        {
            slug = name;
        }
        lock (_dbLocker)
        {        
            _context.categories.Add(new Category()
            {
                     Id = Guid.NewGuid(),
                     Name=name,
                     Description = description,
                     DeletedDt = null,
                     PhotoUrl = photoUrl,
                     Slug = slug
            });
            _context.SaveChanges();
        }


    }

    public List<Category> GetCategories(bool includeDelete = false)
    {
        List<Category> list;
        lock (_dbLocker)
        {
            list = _context.categories.Where(c =>includeDelete || c.DeletedDt == null).ToList();
        }

        return list;
    }

    public Category? GetCategoryBySlug(string slug)
    {
        Category? ctg;
        lock (_dbLocker)
        {
            ctg = _context.categories.FirstOrDefault(c => c.Slug == slug);
        }

        return ctg;
    }

    public Category? GetCategoryById(Guid id)
    {
        Category? ctg;
        lock (_dbLocker)
        {
            ctg = _context.categories.Find(id);
        }

        return ctg;
    }
    public void DeleteCategory(Guid id)
    {
        var ctg = _context.categories.Find(id);
        if (ctg != null && ctg.DeletedDt == null)
        {
            ctg.DeletedDt = DateTime.Now;
            lock (_dbLocker)
            {
                _context.SaveChanges();
            }
            
        }
    }
    public void DeleteCategory(Category category)
    {
        DeleteCategory(category.Id);
    }
    public void UpdateCategory(Category category)
    {
        Category? ctg;
        lock (_dbLocker)
        {
            ctg =  _context.categories.Find(category.Id);
        }
        if (ctg != null && ctg!=category)
        {
            ctg.Name = category.Name;
            ctg.Description = category.Description;
            ctg.DeletedDt = category.DeletedDt;
            ctg.PhotoUrl = ctg.PhotoUrl;
        }
        lock (_dbLocker)
        {
            _context.SaveChanges();
        }
    }
    public void RestoreCategory(Guid id)
    {
        var ctg = _context.categories.Find(id);
        if (ctg != null && ctg.DeletedDt != null)
        {
            ctg.DeletedDt = null;
            lock (_dbLocker)
            {
                _context.SaveChanges();
            }
            
        }
    }
    
    // // // // // location
    public void AddLocation(String name, String description,Guid CategoryId,
        int? Stars = null, Guid? CountryId = null, 
        Guid? CityId = null, string? Address = null, string? PhotoUrl = null, string? slug = null)
    {
        if (slug == null)
        {
            slug = name;
        }
        lock (_dbLocker)
        {
            _context.locations.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CategoryId = CategoryId,
                Stars = Stars,
                CountryId = CountryId,
                CityId = CityId,
                Address = Address,
                DeleteDt = null,
                PhotoUrl = PhotoUrl,
                Slug = slug
            });
            _context.SaveChanges();
        }
       
    }
    public Location? GetLocationById(Guid id)
    {
        Location? lct;
        lock (_dbLocker)
        {
            lct = _context.locations.Find(id);
        }

        return lct;
    }
    public Location? GetLocationBySlug(string slug)
    {
        Location? loc;
        lock (_dbLocker)
        {
            loc = _context.locations.FirstOrDefault(l => l.Slug == slug);
        }

        return loc;
    }
    public List<Location> GetLocations(String categorySlug, bool includeDelete = false)
    {
        var ctg = GetCategoryBySlug(categorySlug);
        if(ctg == null)
        {
            return new List<Location>();
        }
        var query = _context
            .locations
            .Where(loc => 
                (includeDelete || loc.DeleteDt == null) && 
                loc.CategoryId == ctg.Id);  
        return query.ToList();
    }
    public void UpdateLocation(Location location)
    {
        Location? lct;
        lock (_dbLocker)
        {
            lct = _context.locations.Find(location.Id);
        }

        if (lct != null && lct != location)
        {
            lct.Name = location.Name;
            lct.Description = location.Description;
            lct.DeleteDt = location.DeleteDt;
            lct.PhotoUrl = location.PhotoUrl;
            lct.Stars = location.Stars;
        }

        lock (_dbLocker)
        {
            _context.SaveChanges();
        }
    }
    
     public void DeleteLocation(Guid id)
    {
        var loc = _context.locations.Find(id);
        if (loc != null && loc.DeleteDt==null)
        {
            loc.DeleteDt = DateTime.Now;
            lock (_dbLocker)
            {
                _context.SaveChanges();
            }
            
        }
    }
    public void DeleteLocation(Location location)
    {
        DeleteLocation(location.Id);
    }
    public void RestoreLocation(Guid id)
    {
        var loc = _context.locations.Find(id);
        if (loc != null && loc.DeleteDt!= null)
        {
            loc.DeleteDt = null;
            lock (_dbLocker)
            {
                _context.SaveChanges();
            }
            
        }
    }
    //////// Room
    public void AddRoom(String name, String description, 
                 
         String photoUrl, String slug, Guid locationId, int stars, double  Dailyprice)
     {

         lock (_dbLocker)

         {
             _context.rooms.Add(new()

             {

                 Id = Guid.NewGuid(),

                 Name = name,

                 Description = description,

                 DeleteDt = null,

                 PhotoUrl = photoUrl,

                 Slug = slug,

                 LocationId = locationId,

                 Stars = stars,
                 DailyPrice = Dailyprice

             });

             _context.SaveChanges();

         }

     }
    public Room? GetRoomById(Guid id)
    {
        Room? room;
        lock (_dbLocker)
        {
            room = _context.rooms.Find(id);
        }

        return room;
    }
    public void UpdateRoom(Room room)
    {
        Room? rms;
        lock (_dbLocker)
        {
            rms = _context.rooms.Find(room.Id);
        }

        if (rms != null && rms != room)
        {
            rms.Name = room.Name;
            rms.Description = room.Description;
            rms.DeleteDt = room.DeleteDt;
            rms.DailyPrice = room.DailyPrice;
            rms.PhotoUrl = room.PhotoUrl;
            rms.Stars = room.Stars;
        }

        lock (_dbLocker)
        {
            _context.SaveChanges();
        }
    }
    public void DeleteRoom(Guid id)
    {
        var rm = _context.rooms.Find(id);
        if (rm != null && rm.DeleteDt==null)
        {
            rm.DeleteDt = DateTime.Now;
            lock (_dbLocker)
            {
                _context.SaveChanges();
            }
            
        }
    }
    public void DeleteRoom(Room room)
    {
        DeleteRoom(room.Id);
    }
    public void RestoreRoom(Guid id)
    {
        var rm = _context.rooms.Find(id);
        if (rm != null && rm.DeleteDt!= null)
        {
            rm.DeleteDt = null;
            lock (_dbLocker)
            {
                _context.SaveChanges();
            }
            
        }
    }
    public List<Room> GetRooms(string locationSlug, bool includeDelete = false)
    {
        var loc = GetLocationBySlug(locationSlug);
        if (loc == null)
        {
            return new List<Room>();
        }

        var query = _context.rooms.Where(rom => (includeDelete || rom.DeleteDt == null) && rom.LocationId == loc.Id);
        return query.ToList();
    }
    public List<Room> GetRooms(Guid locationId,bool includeDelete = false)
    {

        List<Room> res;
        lock (_dbLocker)
        {
            if (includeDelete)
            {
                res = _context.rooms.Where(r => r.LocationId == locationId).ToList();
            }
            else
            {
                res = _context.rooms.Where(r => r.LocationId == locationId && r.DeleteDt == null).ToList();
            }
        }

        return res;
    }

    public Room? GetRoomBySlug(string slug)
    {
        Guid? id;
        try
        {
            id = Guid.Parse(slug);
        }
        catch
        {
            id = null;
        }

        var slugSelector = (Room c) => c.Slug == slug;
        var idSelector = (Room c) => c.Id == id;

        Room? rooms;
        lock (_dbLocker)
        {
            rooms = _context.rooms.Include(r => r.Reservations).FirstOrDefault(id == null ? slugSelector : idSelector);
        }

        if (rooms != null)
        {
            rooms.Reservations = rooms.Reservations.Where(r => r.DeleteDt == null).ToList();
        }

        return rooms;
    }

    public void ReserveRoom(ReserveRoomFormModel model)
    {
        ArgumentNullException.ThrowIfNull(model,nameof(model));
        if (model.Date < DateTime.Today)
        {
            throw new ArgumentException("Date must not be inpast");
        }

        Room? room;
        lock (_dbLocker)
        {
            room = _context.rooms.Find((model.RoomId));
        }

        if (room==null)
        {
            throw new ArgumentException("Room not Found for id = " + model.RoomId);
        }
        
        _context.Reservations.Add(new()
        {
            Id = Guid.NewGuid(),
            Date = model.Date,
            RoomId = model.RoomId,
            UserId = model.UserId,
            Price = room.DailyPrice,
            OrderDateTime = DateTime.Now

        });
        _context.SaveChanges();
    }

    public Reservation? GetReservation(Guid roomId, DateTime date)
    {
        // Заопбігання подвійного бронування - пошук нового
        Reservation? reservation;
        lock (_dbLocker)
        {
            reservation =
                _context.Reservations.FirstOrDefault(r => 
                    r.RoomId == roomId 
                    && r.Date.Date == date.Date 
                    && r.DeleteDt == null);
        }

        return reservation;
    }
    public void DeleteReservation(Guid id)
    {
        Reservation? reservations;
        lock (_dbLocker)
        {
            reservations = _context.Reservations.Find(id);
        }

        if (reservations == null)
        {
            throw new ArgumentException("Passed id not found!");
        }

        if (reservations.DeleteDt != null)
        {
            throw new ArgumentException("Passed id already delete");
        }
        reservations.DeleteDt = DateTime.Now;
        lock (_dbLocker)
        {
            _context.SaveChanges();
        }
        
    }
}