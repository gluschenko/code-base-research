using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Wishmaster
{
    public class WebStartup
    {
        private readonly IHostEnvironment _env;

        public WebStartup(IHostEnvironment env)
        {
            _env = env;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStatusCodePages("text/plain", "Error. Status code: {0}");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(_env.ContentRootPath, "spa")),
                RequestPath = "/"
            });

            app.Map("/ping", builder => builder.Run(async context =>
            {
                await context.Response.WriteAsync($"OK (uptime: {DateTime.Now - Process.GetCurrentProcess().StartTime})");
            }));
        }
    }
}
