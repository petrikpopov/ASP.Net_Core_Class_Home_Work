namespace ASP_.Net_Core_Class_Home_Work.Services.Hash;

public class Md5HashService : IHashService
{
    public string Digest(string input)
    {
        return Convert.ToHexString(System.Security.Cryptography.MD5.HashData(System.Text.Encoding.UTF8.GetBytes((input))));
    }
}