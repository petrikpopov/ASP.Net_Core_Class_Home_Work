using System.Runtime.InteropServices.JavaScript;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using ASP_.Net_Core_Class_Home_Work.Services.Kdf;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

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

    public User? GetUserByToken(Guid token)
    {
        User? user;
        lock (_dbLocker)
        {
            user = _DataContext.Token.Include(t => t.User).FirstOrDefault(t => t.Id == token)?.User;
        }

        return user;
    }
   
    public Token CreateTokenForUser(User user)
    {
        return CreateTokenForUser(user.Id);
    }
    public Token? CreateTokenForUser(Guid userid)
    {
        Token existingToken = _DataContext.Token
            .FirstOrDefault(t => t.UserID == userid && t.ExpireDt > DateTime.Now);
        if (existingToken != null)
        { 
            return existingToken; 
        }
        
        Token token = new() 
        {
            Id = Guid.NewGuid(),
            UserID = userid,
            SubmitDt = DateTime.Now,
            ExpireDt = DateTime.Now.AddDays(1),
        }; 
        _DataContext.Token.Add(token);
        try
        {

            lock (_dbLocker)
            {
                _DataContext.SaveChanges();
            }

            return token;
        }
        catch(Exception ex)
        {
            _DataContext.Token.Remove(token);
            return null;
        }
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

    public Boolean ConfirmEmail(string email, string code)
    {
        User? user;
        lock(_dbLocker)
        {
           user= _DataContext.users.FirstOrDefault(u => u.Email == email);
        }
        if (user == null || user.EmailConfirmCode!=code)
        {
            return false;
        }

        user.EmailConfirmCode = null;
        lock (_dbLocker)
        {
            _DataContext.SaveChanges();
        }

        return true;
    }
}
// DAL - (Data Access Layer) - сукупність усіх DAO
// DAO - (Data Access Object) - нaбір методів для роботи з сутністю