using System.Text.Json.Serialization;

namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { set; get; }
    public string Email { set; get; }
    public string? EmailConfirmCode { set; get; } 
    public string? AvaratUrl { get; set; }
    public DateTime? Birthdate { get; set; }
    [JsonIgnore]
    public string Salt { set; get; } 
    [JsonIgnore]
    public string DerivedKey { set; get; } 
    
    public DateTime? DeleteDt { set; get; }
    
    public string? Role { set; get; }
    [JsonIgnore]
    public List<Reservation> Reservations { set; get; }
}

