namespace ASP_.Net_Core_Class_Home_Work.Models.Home.Model;

public class PageModel
{
    public string TittlePage { set; get; } = null!;
    public string TabHeader { set; get; } = null!;
    public string AboutModel { set; get; } = null!;
    public string H3ViewModel { set; get; }
    public string AboutViewMode { set; get; }
    public string H3ModelForm { set; get; }
    public string AboutModelForm { set; get; }


    public FormModel? FormModel { get; set; } = null!;
}