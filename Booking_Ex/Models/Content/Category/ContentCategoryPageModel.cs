namespace ASP_.Net_Core_Class_Home_Work.Models.Content.Category;

public class ContentCategoryPageModel
{
    public Data.Entities.Category Category { set; get; } = null;
    public List<Data.Entities.Location> Locations { set; get; } = new ();

}