using ASP_.Net_Core_Class_Home_Work.Data.Entities;

namespace ASP_.Net_Core_Class_Home_Work.Models.Content.Index;

public class ContentIndexPageModel
{
    public List<ASP_.Net_Core_Class_Home_Work.Data.Entities.Category> categories { set; get; } = new ();
}