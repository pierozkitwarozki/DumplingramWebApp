using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dumplingram.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dumplingram.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            /*using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DataContxt>();
                    context.Database.Migrate();
                    Seed.SeedUsers(context);
                }
                catch(Exception exception)
                {   
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Error occured during migration"); 
                }
            }*/
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
