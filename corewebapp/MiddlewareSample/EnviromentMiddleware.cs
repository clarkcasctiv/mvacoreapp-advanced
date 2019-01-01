using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace corewebapp.MiddlewareSample
{
    public class EnviromentMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostingEnvironment _env;

        public EnviromentMiddleware(RequestDelegate next, IHostingEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var timer = Stopwatch.StartNew();

            context.Response.Headers.Add("X-HostingEnviromentName", new[] {
            _env.EnvironmentName});

            await _next(context);

            if (_env.IsDevelopment() && context.Response.ContentType != null && context.Response.ContentType == "text/html")
            {
                await context.Response.WriteAsync($"<p> From { _env.EnvironmentName } in {timer.ElapsedMilliseconds} ms </p>");
            }
        }
    }

    public static class MiddlewareHelpers
    {
        public static IApplicationBuilder UseEnviromentMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<EnviromentMiddleware>();
        }
    }
}