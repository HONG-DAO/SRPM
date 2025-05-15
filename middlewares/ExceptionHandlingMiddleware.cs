using System.Net;
using System.Text.Json;

namespace SRPM.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // tiếp tục pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi không mong muốn");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    status = context.Response.StatusCode,
                    message = "Đã xảy ra lỗi máy chủ. Vui lòng thử lại sau.",
#if DEBUG
                    details = ex.Message
#endif
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
            }
        }
    }
}
