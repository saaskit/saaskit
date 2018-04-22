using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AspNetMvcSample
{
    public class Program
	{
        public static void Main(string[] args)
        {
            Console.Title = "AspNetMvcSample";
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:60000", "http://localhost:60001", "http://localhost:60002", "http://localhost:60003")
                .UseStartup<Startup>()
                .Build();
    }
}