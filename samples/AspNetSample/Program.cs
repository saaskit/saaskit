namespace AspNetSample
{
	using System.IO;

	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Server.Kestrel;
	using Microsoft.Extensions.Options;

	public class Program
	{
		public static void Main(string[] args)
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.UseStartup<Startup>()
				.UseUrls("http://localhost:60000", "http://localhost:60001", "http://localhost:60002", "http://localhost:60003")
				.Build();

			host.Run();
		}
	}
}