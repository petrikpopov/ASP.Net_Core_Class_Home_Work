using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP_.Net_Core_Class_Home_Work.Controllers;

public class BackendController: ControllerBase, IActionFilter
{
    protected bool isAuthentication;
    protected bool iaAdmin;
    protected IEnumerable<Claim>? claims;
    protected String? GetAdminAuthMessage()
    {
        if (!isAuthentication)
        {
            // якщо авторизація не пройдена то повідомлення а Items
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? "Auth required";
        }
        if (!iaAdmin)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return "Access to API forbidden!";
        }

        return null;
    }
    [NonAction]
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var identity = User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthSessionMiddleware));
        identity ??=  User.Identities.FirstOrDefault(i => i.AuthenticationType == nameof(AuthTokenMiddleware));
        this.isAuthentication = identity != null;
        String? useRole = identity?.Claims.FirstOrDefault(c=>c.Type==ClaimTypes.Role)?.Value; 
        this.iaAdmin = "Admin".Equals(useRole);
        claims = identity?.Claims;

    }
    [NonAction] // Якщо неможна зробити метод private то позначаємо атрибутом [NonAction]
    public void OnActionExecuted(ActionExecutedContext context)
    {
       
    }
}