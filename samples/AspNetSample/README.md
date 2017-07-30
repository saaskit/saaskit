# ASP.NET Core Multi-tenancy Sample

This sample demonstrates the basic use of the SaasKit multi-tenancy middleware in ASP.NET Core.

It uses a Caching Tenant Resolver to resolve tenants at the beginning of the request and cache the tenant for subsequent request. An example of a non-caching resolver is also provided.

The URLs for each tenant are hardcoded in `CachingAppTenantResolver`.

The multi-tenancy middleware is registered in the `Startup` class.

### Default Tenant

This sample also demonstrates one of the options for handling unresolved tenants, as previously blogged about [here](http://benfoster.io/blog/handling-unresolved-tenants-in-saaskit).

The tenant resolver middleware always registers a "default" tenant if a tenant can not be resolved. A separate middleware component then checks if the default tenant exists and redirects to `/onboarding`, which has been forked from the default pipeline.

With this approach you can have both tenant and onboarding areas within a single application.