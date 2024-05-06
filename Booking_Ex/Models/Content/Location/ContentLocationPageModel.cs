namespace ASP_.Net_Core_Class_Home_Work.Models.Content.Location;

public class ContentLocationPageModel
{
    public Data.Entities.Location Location { set; get; } = null!;
    public List<Data.Entities.Room> Rooms { set; get; } = new();
}