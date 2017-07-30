﻿using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace AspNetStructureMapSample
{
    public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseUrls("http://localhost:60000", "http://localhost:60001", "http://localhost:60002")
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			host.Run();
		}
	}
}