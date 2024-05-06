namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class Category
{
    public Guid Id { set; get; }
    public string Name { set; get; } = null!;
    public string Description { set; get; } = null!;
    public DateTime? DeletedDt { set; get; } // ознака видалення
    public string? PhotoUrl { set; get; }
    
    public string? Slug { get;set; } // slug - ідентифікатор ресурсу
}
//{09B2381F-452A-4621-8317-CCE6E3EE1A20}