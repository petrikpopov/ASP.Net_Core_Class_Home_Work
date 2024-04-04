namespace ASP_.Net_Core_Class_Home_Work.Models.Home.Ioc;

public class IocPageModel
{
    public string Tittle { set; get; } = null!;
    public string HeaderPage { set; get; }
    public string AboutIoc { set; get; }
    public string[] LiStepWork_Ioc { set; get; }
    public string CreateService { set; get; }
    public string[] TypeInversion { set; get; }
    public string ExampleCreateService { set; get; }
    public string[] LiExampleCreateService { set; get; }
    public string SingleHash { set; get; } = null!;
    public Dictionary<string, string> Hashes { get; set; } = new();

}