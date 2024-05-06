namespace ASP_.Net_Core_Class_Home_Work.Services.Random;

public class RandomService:IRandomService
{
    public System.Random random = new();

    public string RandomOTP(int lenth)
    {
        const string str1 = "0123456789";
        return GenerateRandom(str1, lenth);
    }

    public string RandomNameFile(int lenth)
    {
        string str = "bcdefghijklmnopqrstuvwxyz!@#$";
        return GenerateRandom(str,lenth);
    }

    public string RandomSalt(int lenth)
    {
        string str = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
        return GenerateRandom(str, lenth);
    }

    public string GenerateRandom(string chars,int lenth)
    {
        char[] buffer = new char [lenth];
        for (int i = 0; i<lenth;i++)
        {
            buffer[i] = chars[random.Next(chars.Length)];
        }

        return new string(buffer);

    }
}