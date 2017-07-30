# ASP.NET Core StructureMap Multi-tenancy Sample

This sample demonstrates multi-tenant dependency injection in ASP.NET Core using StructureMap, as previously blogged about [here](http://benfoster.io/blog/asp-net-core-dependency-injection-multi-tenant).

It uses StructureMap to work around the built-in container's lack of custom scopes. Often we need to provide different implementations of services for each tenant, or scope services as a singleton-per-tenant. Using StructureMap we're able to make use of it's support for child containers and register a unique container per tenant, each of which can be configured at runtime.

The URLs for each tenant are hardcoded in `AppTenantResolver`. 

When you run the application and browse to one of the tenant URLs you'll see an output similar to the following:

```
e6b1cbb8-e976-4937-87b4-6961d68be6d3: Tenant "Tenant 2".
```

The GUID generated here is unique per tenant but constant between requests for that tenant. This is because the `MessageService` is registered per tenant using the `ConfigureTenants` extension in `SaasKit.Multitenancy.StructureMap`, in `Startup`.

There is also an example of how to register your own `ITenantContainerBuilder` which you may prefer if you're tenant dependency registration is more involved.