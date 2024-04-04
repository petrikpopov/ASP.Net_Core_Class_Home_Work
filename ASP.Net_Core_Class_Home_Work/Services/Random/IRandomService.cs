namespace ASP_.Net_Core_Class_Home_Work.Models;

public interface IRandomService
{
     string RandomOTP(int lenth);
     string RandomNameFile(int lenth);
     string RandomSalt(int lenth);
     string GenerateRandom(string chars, int lenth);
}