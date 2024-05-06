using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using ASP_.Net_Core_Class_Home_Work.Services.Kdf;

namespace ASP_.Net_Core_Class_Home_Work.Data.DAL;

public class UserDao
{
    private readonly Object _dbLocker;
    public readonly DataContext _DataContext;
    private readonly IKdfService _kdfService;
    public UserDao(DataContext dataContext, IKdfService kdfService , Object _dbLocker)
    {
        _DataContext = dataContext;
        _kdfService = kdfService;
        this._dbLocker = _dbLocker;
    }

    public User? GetUserByID(string id)
    {
        User? user;
        try
        {
            lock (_dbLocker)
            {
                 user = _DataContext.users.Find(Guid.Parse(id));
            }
           
        }
        catch
        {
            return null;
        }

        return user;
    }

    public User? Authorize(string email, string password)
    {
        var user = _DataContext.users.FirstOrDefault(u => u.Email == email);
        if (user == null || user.DerivedKey != _kdfService.DerivedKey(user.Salt, password))
        {
            return null;
        }
        return user;
        
    }

    public void SignUp(User user)
    {
        if (user.Id == default)
        {
            user.Id = Guid.NewGuid();
        }

        _DataContext.users.Add(user);
        _DataContext.SaveChanges();
    }
}
// DAL - (Data Access Layer) - сукупність усіх DAO
// DAO - (Data Access Object) - нaбір методів для роботи з сутністю