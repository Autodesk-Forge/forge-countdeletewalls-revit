using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Autodesk.Forge.Core;
using Autodesk.Forge.DesignAutomation;

namespace forgesample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).ConfigureAppConfiguration(builder =>
            {
                builder.AddForgeAlternativeEnvironmentVariables();
            }).ConfigureServices((hostContext, services) =>
            {
                services.AddDesignAutomation(hostContext.Configuration);
            }).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
