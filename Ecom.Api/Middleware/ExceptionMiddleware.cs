using Ecom.Api.Helper;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;

namespace Ecom.Api.Middleware
{
    public class ExceptionMiddleware
    {        //to rate limit and handle exceptions globally

        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _enviroment;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _ratelimitWindow = TimeSpan.FromSeconds(30);
        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment enviroment, IMemoryCache memoryCache)
        {
            _next = next;
            _enviroment = enviroment;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                ApplySecurity(context);


                if (IsRequestAllowed(context) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.ContentType = "application/json";
                    var response = new ApiException(context.Response.StatusCode, "Too many requests. Please try again later.");
                    //convert response to json
                    await context.Response.WriteAsJsonAsync(response);
                }
                await _next(context);
            }
            catch (Exception ex)
            {

                context.Response.StatusCode=(int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var response = _enviroment.IsDevelopment() ?
                    new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace.ToString())
                    : new ApiException(context.Response.StatusCode, ex.Message);
                //number/message/stacktrace
                //convert response to json
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
        private bool IsRequestAllowed(HttpContext context)
        {
            //Rate limit
            //1-get ip address
            var ip = context.Connection.RemoteIpAddress.ToString();
            var cachkey = $"Rate:{ip}";
            var dateNow = DateTime.Now;

            var (timestamp, count) = _memoryCache.GetOrCreate(cachkey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _ratelimitWindow;
                return(timestamp: dateNow, count: 0);
            });
            if (dateNow - timestamp < _ratelimitWindow)
            {
                if (count >= 8)
                {
                    return false;
                }
                _memoryCache.Set(cachkey, (timestamp, count += 1), _ratelimitWindow);
            }
            else
            {
                _memoryCache.Set(cachkey, (timestamp, count ), _ratelimitWindow);
            }
            return true;
        }
        private void ApplySecurity(HttpContext context)
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }
    }
}
