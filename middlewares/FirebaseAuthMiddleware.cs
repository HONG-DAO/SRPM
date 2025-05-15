using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SRPM.Middlewares
{
    public class FirebaseAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public FirebaseAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                string idToken = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    var token = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, token.Uid),
                        new Claim(ClaimTypes.Name, token.Claims["name"]?.ToString() ?? ""),
                        new Claim(ClaimTypes.Email, token.Claims["email"]?.ToString() ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, "firebase");
                    var principal = new ClaimsPrincipal(identity);

                    context.User = principal;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Token không hợp lệ: " + ex.Message);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Token không hợp lệ");
                    return;
                }
            }

            await _next(context); // Cho phép request đi tiếp nếu không cần xác thực
        }
    }
}
