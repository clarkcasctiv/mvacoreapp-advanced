using System.Linq;
using corewebapp.MiddlewareSample;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace corewebapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Information);

            var logger = loggerFactory.CreateLogger("MiddleWare Demo");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Use(async (context, next) =>
            //{
            //    var timer = Stopwatch.StartNew();
            //    logger.LogInformation($"==> beginning request in {env.EnvironmentName}");
            //    await next();

            //    logger.LogInformation($"==> completed request in {timer.ElapsedMilliseconds} ms");
            //});

            //app.UseMiddleware<EnviromentMiddleware>();
            app.UseEnviromentMiddleware();

            app.Map("/Contacts", a => a.Run(async context =>
            {
                await context.Response.WriteAsync("Here are your contacts");
            }));

            app.MapWhen(context =>

           context.Request.Headers["User-Agent"].First().Contains("Firefox"), FirefoxRoute)
            ;

            app.UseStaticFiles();
            // End of the pipeline
            // Similar to app.UseMvc()

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Hello World!");
            });
        }

        private void FirefoxRoute(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Hello Firefox");
            });
        }
    }
}