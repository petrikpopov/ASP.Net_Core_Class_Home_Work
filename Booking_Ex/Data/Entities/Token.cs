namespace ASP_.Net_Core_Class_Home_Work.Data.Entities;

public class Token
{
    public Guid Id { set; get; }
    public Guid UserID { set; get; }
    public DateTime SubmitDt { set; get; }
    public DateTime ExpireDt { set; get; }
    public User User { get; set; } = null!;
}