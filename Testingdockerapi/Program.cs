using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Testingdockerapi
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        //private static void InitializeConfiguration()
        //{
        //    var builder = new ConfigurationBuilder()
        //        .AddUserSecrets<Program>();

        //    Configuration = builder.Build();
        //}

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             //.ConfigureLogging(loggingBuilder =>
             //{
             //    loggingBuilder.ClearProviders();
             //    loggingBuilder
             //        .AddDebug()
             //        .AddEventLog();
             //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
