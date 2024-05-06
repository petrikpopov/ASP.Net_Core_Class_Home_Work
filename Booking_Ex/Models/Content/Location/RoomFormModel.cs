using Microsoft.AspNetCore.Mvc;

namespace ASP_.Net_Core_Class_Home_Work.Models.Content.Location;

public class RoomFormModel
{
    [FromForm(Name = "location-id")]
    public Guid LocationyId { set; get; }
    [FromForm(Name = "room-name")]
    public string Name { set; get; }
    [FromForm(Name = "room-description")]
    public string Description { set; get; }
    [FromForm(Name = "room-slug")]
    public string Slug { set; get; }
    [FromForm(Name = "room-stars")]
    public int Stars { set; get; }
    
    [FromForm(Name = "room-price")]
    public Double DailyPrice { set; get; }
    
    [FromForm(Name = "room-photo")]
    public IFormFile Photo { set; get; }
   
}