using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;


namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap.AppStart
{
	public static class RouteConfig
	{
		public static IRouteBuilder Setup(IRouteBuilder routes)
		{
			routes.MapRoute(
				name: "default",
				template: "{controller=Home}/{action=Index}/{id?}"
			);

			return routes;
		}
	}
}
