using System.Text;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;
using NLog;


namespace CodeBlocksMiddleware
{

    public class JsonExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public JsonExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

            _jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }
        public async Task Invoke(HttpContext context)
        {
            var requestData = await FormatRequest(context.Request);
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                CreateErrorMessage(sb, ex);
                _logger.Error(ex, $"Unhandled error for request: {requestData}| {sb}");
                await context.Response.WriteAsync(sb.ToString());
            }
        }

        public static void CreateErrorMessage(StringBuilder sb, Exception ex, int depth = 0)
        {
            var aggrEx = ex as AggregateException;

            void AppendExceptionMessage(Exception ex2)
            {
                if (aggrEx != null)
                {
                    sb.Append("Aggregate ex:");
                }
                else
                {
                    sb.Append(ex2.GetType().Name);
                    sb.Append(": ");
                    sb.Append(ex2.Message);
                }
            }

            if (depth == 0)
                AppendExceptionMessage(ex);
            else
            {
                sb.AppendLine();
                for (var i = 0; i < depth; i++)
                    sb.Append("\t");
                AppendExceptionMessage(ex);
            }

            if (aggrEx != null)
            {
                foreach (Exception innerEx in aggrEx.InnerExceptions)
                {
                    CreateErrorMessage(sb, innerEx, depth + 1);
                }
            }
            else if (ex.InnerException != null)
            {
                CreateErrorMessage(sb, ex.InnerException, depth + 1);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body.Seek(0, SeekOrigin.Begin);

            return $"{request.Method} {request.ContentType} {request.Path} {request.QueryString} {request.HttpContext.Connection} " +
                   $"{request.HttpContext.Request.Host.Value} {request.Cookies.Count} {request.Protocol} {request.Scheme} {bodyAsText}";
        }
  
    }
    public static class JsonExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseJsonExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JsonExceptionMiddleware>();
        }
    }
}
