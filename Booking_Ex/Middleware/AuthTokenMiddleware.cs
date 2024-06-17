using System.Security.Claims;
using ASP_.Net_Core_Class_Home_Work.Data.DAL;
using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using Microsoft.Extensions.Primitives;

namespace ASP_.Net_Core_Class_Home_Work.Middleware;

public class AuthTokenMiddleware
{
      private readonly RequestDelegate _next;
      public AuthTokenMiddleware(RequestDelegate next)
      {
       _next = next;
      }
    
      public async Task InvokeAsync(HttpContext context, DataAccessor dataAccessor, ILogger<AuthTokenMiddleware> logger)
      {
          var authHeader = context.Request.Headers["Authorization"];
          try
          {
              
              if (authHeader == StringValues.Empty)
              {
                  throw new Exception("Authentication required");
              }
              string authValue = authHeader.First()!;
              if (!authValue.StartsWith("Bearer"))
              {
                 
                  throw new Exception("Bearer scheme required!");
              }
              string token = authValue[7..];
              Guid tokenId;
              try
              {
                  tokenId = Guid.Parse(token);
              }
              catch
              {
                 
                  throw new Exception("Token invalid GUID required!") ;
              }

              User? user = dataAccessor.UserDao.GetUserByToken(tokenId);
              if (user == null)
              {
                
                  throw new Exception("Token invalid or expired!");
              }
              
              Claim[] claims = new Claim[]
             {
                 new (ClaimTypes.Sid, user.Id.ToString()),
                 new (ClaimTypes.Email, user.Email),
                 new (ClaimTypes.Name,user.Name),
                 new (ClaimTypes.UserData, user.AvaratUrl ?? ""),
                 new (ClaimTypes.Role, user.Role ??""),
                 new ("EmailConfirmCode", user.EmailConfirmCode??"")
             };
             context.User.AddIdentities(
                 new[]
                 {
                     new ClaimsIdentity(
                         claims, nameof(AuthTokenMiddleware))
                 });
             
             
          }
          catch (Exception ex)
          {
              context.Items.Add(new (nameof(AuthTokenMiddleware), ex.Message));
          }
          await _next(context);
      }
}

public static class AuthTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthToken(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthTokenMiddleware>();
    }
}
