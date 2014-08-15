using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using SaasKit.Integration.Nancy;
using SaasKit.Model;

namespace SaasKit.Demos.Nancy
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            this.Conventions.ViewLocationConventions.Add((viewName, model, context) =>
            {
                var tenant = context.Context.GetTenantInstance() as Tenant;
                return string.Concat("views/", tenant.Id, "/", viewName);
            });
        }
    }
}
