using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NugetAnalyzer.Common.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate nextRequestDelegate;
        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(RequestDelegate nextRequestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.nextRequestDelegate = nextRequestDelegate ?? throw new ArgumentNullException(nameof(nextRequestDelegate));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await nextRequestDelegate(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        protected virtual Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}