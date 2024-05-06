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
}
