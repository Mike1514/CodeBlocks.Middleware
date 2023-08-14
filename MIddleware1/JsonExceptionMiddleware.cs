using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CodeBlocksMiddleware
{

    public class JsonExceptionMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly ILogger<JsonExceptionMiddleware> _logger;
        private IMiddleware _middlewareImplementation;

        public JsonExceptionMiddleware(RequestDelegate next, ILogger<JsonExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next ?? throw new ArgumentNullException(nameof(next));

            _jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var resuestData = await FormatRequest(context.Request);
            try
            {
                var originalBodyStream = context.Response.Body;
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled error for request: {resuestData}");
            }
        }

        public static void CreateErrorMessage(StringBuilder sb, Exception ex, int depth = 0)
        {
            var aggrEx = ex as AggregateException;

            void appendExceptionMessage(Exception ex2)
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
                appendExceptionMessage(ex);
            else
            {
                sb.AppendLine();
                for (var i = 0; i < depth; i++)
                    sb.Append("\t");
                appendExceptionMessage(ex);
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

            return $"{request.Method} {request.Path} {request.QueryString} {bodyAsText}";
        }
    }
}
