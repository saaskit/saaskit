using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace SaasKit
{
    public class SaasKitOwinAdapter : OwinMiddleware
    {
        private readonly ISaasKitEngine engine;

        public SaasKitOwinAdapter(OwinMiddleware next, ISaasKitEngine engine) : base(next)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }

            this.engine = engine;
        }
        
        public async override Task Invoke(IOwinContext context)
        {
            await engine.BeginRequest(context);
            await Next.Invoke(context);
            await engine.EndRequest(context);
        }
    }
}