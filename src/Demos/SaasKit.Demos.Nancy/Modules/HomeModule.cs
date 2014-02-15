using Nancy;
using SaasKit.Integration.Nancy;

namespace SaasKit.Demos.Nancy.Modules
{  
    public class HomeModule : NancyModule
    {       
        public HomeModule()
        {
            Get["/"] = _ =>
            {
                var model = new
                {
                    Tenant = Context.GetTenantInstance()
                };

                return View["home", model];
            };
        }
    }
}