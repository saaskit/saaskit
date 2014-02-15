using SaasKit;
using System;

namespace Owin
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseSaasKit(this IAppBuilder app, ISaasKitEngine engine)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }

            if (engine == null)
            {
                throw new ArgumentNullException("engine");
            }

            return app.Use(typeof(SaasKitOwinAdapter), engine);
        }
    }
}