using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebsocketSecure_Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHost(args).Run();
        }

        private static IWebHost CreateWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    //Listen on 5000 with http
                    options.Listen(IPAddress.Any, 5000);
                    try
                    {
                        //Listen on 5001 with https
                        options.Listen(IPAddress.Any, 5001,
                            listenOptions => { listenOptions.UseHttps("../../Keys/server.p12", "Admin1234"); });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                })
                .UseStartup<Startup>()
                .UseConfiguration(config)
                .Build();
        }
    }
}