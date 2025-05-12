using System.Net;

namespace Bookify.Api.Middleware;

public class LocalOnlyHealthCheckMiddleware
{
    private readonly RequestDelegate _next;

    public LocalOnlyHealthCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            IPAddress? remoteIp = context.Connection.RemoteIpAddress;

            if (!IPAddress.IsLoopback(remoteIp!))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: Only local access allowed.");
                return;
            }
        }

        await _next(context);
    }
}
