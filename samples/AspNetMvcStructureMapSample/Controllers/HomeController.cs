using Microsoft.AspNet.Mvc;
using SaasKit.Multitenancy.AspNet5;
using SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap.Models;

namespace SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap.Controllers
{
	public class HomeController : Controller
	{
		private readonly IMessageService _messageService;

		public HomeController(IMessageService messageService)
		{
			_messageService = messageService;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			var viewModel = new HomeViewModel();
			viewModel.TenantContext = HttpContext.GetTenantContext<AppTenant>();
			viewModel.Message = _messageService.GetMessage();

			return View(viewModel);
		}
	}
}
