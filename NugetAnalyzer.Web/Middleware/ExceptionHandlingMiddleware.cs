using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NugetAnalyzer.Web.Middleware
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
            }

            await nextRequestDelegate.Invoke(context);
        }
    }
}