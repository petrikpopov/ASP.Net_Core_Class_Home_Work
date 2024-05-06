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
    // в силу особливостей роботи Middleware інжекція сервісів здійснюється не у конструктор а у параметр методу
    public async Task InvokeAsync(HttpContext context, DataAccessor dataAccessor)
    {
       //чи не запитано вихід
       if (context.Request.Query.ContainsKey("logout"))
       {
           context.Session.Remove("auth-user-id");
           context.Response.Redirect("/");
           return; // без _next це припинить роботу
       }
       else if (context.Session.GetString("auth-user-id") is String userId)
        {
            // прямий хід від запиту да Razoz
            var user = dataAccessor.UserDao.GetUserByID(userId);
            if (user != null)
            {
                 // система авторизаціі ASP передбачає заповнення спеціального поля 
                 //context.User - набору спеціальних Claims-параметрів , кожен з яких видповидає за свій атрибут (id,email,......)
                 Claim[] claims = new Claim[]
                 {
                     new (ClaimTypes.Sid, userId),
                     new (ClaimTypes.Email, user.Email),
                     new (ClaimTypes.Name,user.Name),
                     new (ClaimTypes.UserData, user.AvaratUrl ?? ""),
                     new (ClaimTypes.Role, user.Role ??""),
                 };
                 context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(AuthSessionMiddleware)));
                 //context.Items.Add("auth", "ok");
            }

           
            
        }

        await _next(context);
        // зворотній хід від Razoz до відповіді
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