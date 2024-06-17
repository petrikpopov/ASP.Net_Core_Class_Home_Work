using System.Text.Json.Serialization;

namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }    
    
    public Double Price { get; set; }
   
    public DateTime OrderDateTime { set; get; }
    public DateTime? DeleteDt { set; get; } 

    // navigation props
    [JsonIgnore] public User User { set; get; }
    [JsonIgnore] public Room Room { set; get; }
}