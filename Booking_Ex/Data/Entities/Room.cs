namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class Room
{
    
    public Guid Id { set; get; }
    public Guid LocationId { set; get; }
    public int? Stars { set; get; }
    public string Name { set; get; } = null!;
    public string Description { set; get; } = null!;
    public DateTime? DeleteDt { get; set; }
    public Double DailyPrice { get; set; }
    public string? Slug { get;set; }
    public string? PhotoUrl { get; set; }
    public List<Reservation> Reservations { set; get; }
}