using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{ 
    private readonly DataAccessor _dataAccessor;
    public AuthController(DataAccessor dataAccessor)
    {
        _dataAccessor = dataAccessor;
    }

   
    [HttpGet]
    public object Get(string email, string password)
    {
        var user = _dataAccessor.UserDao.Authorize(email, password);
        if (user == null)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new { Status = "Auth failed" };
        }
        else
        {
            // Cecіі - збереження данних що будуть доступними після перезавантаження странічки
            HttpContext.Session.SetString("auth-user-id",user.Id.ToString());
            return user;

        }
        
    }
    [HttpPost]
    public object Post()
    {
        return new { Status = "Post work" };
    }
    [HttpPut]
    public object Put()
    {
        return new { Status = "Put work" };
    }

    [HttpPatch]
    public object Patch(string email, string code) // confirm email by code
    {
        if (_dataAccessor.UserDao.ConfirmEmail(email, code))
        {
            Response.StatusCode = StatusCodes.Status202Accepted;
            return new { StatusCode = "OK" };
        }
        else
        {
            Response.StatusCode = StatusCodes.Status409Conflict;
            return new { StatusCode = "ERROR" };
        }
    }

    [HttpGet("token")]
    public Token? GetToken(string email, string password)
    {
        var user = _dataAccessor.UserDao.Authorize(email, password);
        if (user == null)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return null;
        }

        return _dataAccessor.UserDao.CreateTokenForUser(user);
    }
}

