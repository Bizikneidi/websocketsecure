using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
                    try
                    {
                        //Listen on 5001 with https
                        options.Listen(IPAddress.Any, 5001,
                            listenOptions => { listenOptions.UseHttps("../Keys/server.p12", "Admin1234"); });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}