﻿using Nancy;
using Nancy.Owin;
using SaasKit.Model;

namespace SaasKit.Integration.Nancy
{
    public static class NancyContextExtensions
    {
        public static Tenant GetTenantInstance(this NancyContext context)
        {
            var owinEnvironment = context.GetOwinEnvironment();
            object tenant;
            return owinEnvironment.TryGetValue(Constants.OwinCurrentTenant, out tenant) ? (Tenant)tenant : null;
        }
    }
}
