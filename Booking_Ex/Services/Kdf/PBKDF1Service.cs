using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Services.Hash;

namespace ASP_.Net_Core_Class_Home_Work.Services.Kdf;

public class PBKDF1Service : IKdfService
{
    private readonly IHashService _hashService;

    public PBKDF1Service(IHashService hashService)
    {
        _hashService = hashService;
    }

    public string DerivedKey(string salt, string password)
    {
        String t1 = _hashService.Digest(password + salt);
        String t2 = _hashService.Digest(t1);
        String t3 = _hashService.Digest(t2);
        return t3;
    }
}
