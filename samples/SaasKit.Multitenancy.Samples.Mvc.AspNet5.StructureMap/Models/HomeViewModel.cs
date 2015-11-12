using SaasKit.Multitenancy.AspNet5;

namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap.Models
{
	public class HomeViewModel
	{
		public string Message { get; set; }
		public TenantContext<AppTenant> TenantContext { get; set; }
	}
}
