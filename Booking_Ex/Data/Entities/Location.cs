namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class Location
{
    public Guid Id { set; get; }
    public Guid? CategoryId { set; get; }
    public Guid? CountryId { set; get; }
    public Guid? CityId { set; get; }
    public string? Address { set; get; } = null!;
    public int? Stars { set; get; }
    public string Name { set; get; } = null!;
    public string Description { set; get; } = null!;
    public DateTime? DeleteDt { get; set; }
    public string? PhotoUrl { set; get; }
    
    public string? Slug { get;set; }
    
}