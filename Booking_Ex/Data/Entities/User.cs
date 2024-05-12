using System.Text.Json.Serialization;

namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { set; get; }
    public string Email { set; get; }
    public string? EmailConfirmCode { set; get; } // code Or Null: null - ознака підтвердження
    public string? AvaratUrl { get; set; }
    public DateTime? Birthdate { get; set; }
    [JsonIgnore]
    public string Salt { set; get; } // за RFC-2898 
    [JsonIgnore]
    public string DerivedKey { set; get; } // за RFC-2898
    
    public DateTime? DeleteDt { set; get; }
    
    public string? Role { set; get; }
    [JsonIgnore]
    public List<Reservation> Reservations { set; get; }
}
//Категорія - отель квартиры вилы курты
//Локация - отель1, отель2,отель3
//Комната - комната1, комната2 , комната3 ......

//
