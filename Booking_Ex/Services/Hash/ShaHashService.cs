namespace ASP_.Net_Core_Class_Home_Work.Services.Hash;

public class ShaHashService :IHashService
{
    public string Digest(string input)
    {
        return Convert.ToHexString(System.Security.Cryptography.SHA1.HashData(System.Text.Encoding.UTF8.GetBytes((input))));
    }
}