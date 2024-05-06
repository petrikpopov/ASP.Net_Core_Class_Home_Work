namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { set; get; }
    public string Email { set; get; }
    public string? AvaratUrl { get; set; }
    public DateTime? Birthdate { get; set; }
    public string Salt { set; get; } // за RFC-2898 
    public string DerivedKey { set; get; } // за RFC-2898
    
    public DateTime? DeleteDt { set; get; }
    
    public string? Role { set; get; }
    public List<Reservation> Reservations { set; get; }
}
//Категорія - отель квартиры вилы курты
//Локация - отель1, отель2,отель3
//Комната - комната1, комната2 , комната3 ......

//
