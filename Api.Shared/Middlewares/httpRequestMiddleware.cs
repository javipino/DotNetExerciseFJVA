using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Api.Shared.Middlewares
{
    public class HttpRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        public HttpRequestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _loggerFactory = loggerFactory;
            _logger = loggerFactory?.CreateLogger<HttpRequestMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Global error management
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Type type = ex.TargetSite.ReflectedType;
                ILogger logger;

                if (type != null && _loggerFactory != null)
                {
                    logger = _loggerFactory?.CreateLogger(type);
                }
                else
                {
                    logger = _logger;
                }

                logger.LogError(ex.Demystify(), string.Format("[{0}] {1}: {2}", DateTime.Now.ToString(), ex.GetType().Name, ex.Message));

                context.Response.StatusCode = 500;
            }
        }
    }

    /// <summary>
    /// Extension method to use the middleware from Startup
    /// </summary>
    public static class HttpRequestMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpRequestMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpRequestMiddleware>();
        }
    }
}
