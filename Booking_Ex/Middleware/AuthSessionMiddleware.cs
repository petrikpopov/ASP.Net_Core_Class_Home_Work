using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;

namespace ASP_.Net_Core_Class_Home_Work.Middleware;

public class AuthSessionMiddleware
{
    private readonly RequestDelegate _next;

    public AuthSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
   
    public async Task InvokeAsync(HttpContext context, DataAccessor dataAccessor)
    {
       
       if (context.Request.Query.ContainsKey("logout"))
       {
           context.Session.Remove("auth-user-id");
           context.Response.Redirect("/");
           return; 
       }
       else if (context.Session.GetString("auth-user-id") is String userId)
        {
            
            var user = dataAccessor.UserDao.GetUserByID(userId);
            if (user != null)
            {
                 Claim[] claims = new Claim[]
                 {
                     new (ClaimTypes.Sid, userId),
                     new (ClaimTypes.Email, user.Email),
                     new (ClaimTypes.Name,user.Name),
                     new (ClaimTypes.UserData, user.AvaratUrl ?? ""),
                     new (ClaimTypes.Role, user.Role ??""),
                     new ("EmailConfirmCode", user.EmailConfirmCode??"")
                 };
                 context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(AuthSessionMiddleware)));
                
            }

           
            
        }

        await _next(context);
       
    }
}

public static class AuthSessionMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthSession(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware< AuthSessionMiddleware>();
    }
}