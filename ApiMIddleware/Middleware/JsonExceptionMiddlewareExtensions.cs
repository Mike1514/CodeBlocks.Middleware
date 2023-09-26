using CodeBlocksMiddleware;
using Microsoft.AspNetCore.Builder;

namespace ApiMIddleware.Middleware
{
    public static class JsonExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseJsonExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JsonExceptionMiddleware>();
        }
    }
}
