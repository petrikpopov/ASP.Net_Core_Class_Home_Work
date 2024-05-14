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
          // Токени передаються за стандартною схемою - заголовком
          // Authorization: Bearer 1233242
          // де 1233242 - токен
          /*Це друга авторизація , яка працює поруч з сесійною. Дані ма/ть не перезапитуватись, а додаватись*/
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

              Token? tokenData = dataAccessor.UserDao.GetTokenById(tokenId);
              if (tokenData == null || tokenData.ExpireDt < DateTime.UtcNow)
              {
                  context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                  await context.Response.WriteAsync("Token invalid or expired!");
                  return;
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
              //logger.LogWarning(context.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? "");
              //logger.LogWarning(ex.Message);
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


/*
 * Авторизація токенами
 */
 /* REST - сеії не використоруємо (для API), кожен запит авторизуємо окремо 
  * REST - Representation State Transfer.
  * Набір вимог до архітектури та роботи серсерву
  * - відсутність "пам'яті" - кожен запит обробляється незалежно від історіі попередніх запитів
  * - реалізація CRUD
  * - стантартизація запитів запитів та відповідей(як запити, так і відповідей мають шаблонну структуру)
  * наприклад , всі запити методом PATCH спрямовані на повну детацізацію іформації про об'єкт, авторизація має єдину схему , відомості про локалізацію передаються у  заголовку "Local"
  * а всі відповіді містят у собі поля з назвою сервісу та час запиту
  *
  * PATCH /room/123 <------> {server: "room API", time:"18173", data:.......}
  * PATCH /user/123 <------> {server: "room API", time:"16378", data:.......}
  */