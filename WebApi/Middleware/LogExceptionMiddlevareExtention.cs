using CodeBlocksMiddleware;
using Microsoft.AspNetCore.Builder;

namespace ApiMIddleware.Middleware
{
    public static class LogExceptionMiddlevareExtention
    {
        public static IApplicationBuilder UseLogExceptionMiddlevare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogExceptionMiddlevare>();
        }
    }
}