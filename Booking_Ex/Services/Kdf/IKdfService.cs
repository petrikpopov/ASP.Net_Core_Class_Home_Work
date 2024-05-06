namespace ASP_.Net_Core_Class_Home_Work.Services.Kdf;

public interface IKdfService
{
    String DerivedKey(String salt, String password);
}
// Key Derivation service by RFC 2898 