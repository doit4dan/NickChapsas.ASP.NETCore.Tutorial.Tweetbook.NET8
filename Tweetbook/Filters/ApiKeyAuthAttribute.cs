using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Tweetbook.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)] // attribute can only be used on class and method
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // e.g. You could also get Api Key from query string
            // context.HttpContext.Request.Query["ApiKey"]

            // before it gets to controller
            if(!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiKey");

            if(!apiKey.Equals(potentialApiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next(); // calls controller

            // after it passes controller
        }
    }
}
