namespace ASP_.Net_Core_Class_Home_Work.Models.Content.Room;

public class RoomPageModel
{
    public Data.Entities.Room Room { set; get; } = null;
    public int Year { get; set; }
    
    public int Month { get; set; }
    
    public int? Day { get; set; }
}