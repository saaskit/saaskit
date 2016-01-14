# Useful Info

- [StructureMap 4.0.0](http://structuremap.github.io/documentation/)
    - [Profiles and Child Containers](http://structuremap.github.io/the-container/profiles-and-child-containers/)
- [StructureMap.Dnx](https://github.com/structuremap/structuremap.dnx)
	- Used to configure StructureMap and create the standard `IServiceProvider` that Asp.Net 5 uses for Dependency Injection
	- `MultiTenantStructureMapServiceScopeFactory.cs` was adapted from [StructureMapServiceScopeFactory.cs](https://github.com/structuremap/structuremap.dnx/blob/master/src/StructureMap.Dnx/StructureMapServiceScopeFactory.cs)
- Check the sample project "SaasKit.Multitenancy.Samples.Mvc.AspNet5.StructureMap" for example implementation and overriding default StructureMap configuration for a tenant


# Multitenancy with StructureMap

The sample website has a `IMessageService` interface, with two implemenations: `MessageService` and `OtherMessageService`.

1. Add a nuget reference to this project "SaasKit.Multitenancy.AspNet5.StructureMap" (which includes a a reference to StructureMap.Dnx v0.4.0-alpha and StructureMap v4 at time of writing)
2. Implement the applications versions of the sample site's `AppTenant` and `AppTenantResolver`
3. Extend `MultiTenantStructureMapServiceScopeFactory<TTenant>` for the application using `AppTenant` as the type. Implement the following methods:
    - **(Required)** ` GetTenantProfileName`: get a key (needs to be consistant between calls) for the current tenant's StructureMap profile
    - `ConfigureTenantProfileContainer`: add any tenant specific configuration for the current request. eg:

```csharp
public class AppMultiTenantStructureMapServiceScopeFactory : MultiTenantStructureMapServiceScopeFactory<AppTenant>
{
	public AppMultiTenantStructureMapServiceScopeFactory(IContainer container) : base(container) { }

    // required
	protected override string GetTenantProfileName(TenantContext<AppTenant> tenantContext)
	{
		return tenantContext.Tenant.Name;
	}

    // optional: Defaults to just using the main Container defaults
	protected override void ConfigureTenantProfileContainer(IContainer tenantProfile, TenantContext<AppTenant> tenantContext)
	{
		// for Tenant 2, want to override IMessageService to use OtherMessageService
		if (tenantProfile.ProfileName.Equals("Tenant 2", StringComparison.InvariantCultureIgnoreCase))
		{
			tenantProfile.Configure(c => c.For<IMessageService>().Use<OtherMessageService>());
		}
	}
}
```

In `Startup.cs > ConfigureServices(IServiceCollection services)`, we configure the default StructureMap container to work with AspNet 5 MVC 6 using something like:

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
	// Add MVC services to the services container.
	services.AddMvc();

	// do after adding other services
	var container = new Container();

	// Here we populate the container using the service collection.
	// This will register all services from the collection
	// into the container with the appropriate lifetime.
	container.Populate(services);

	// IMPORTANT: configuration needs to be done after calling the "container.Populate(services)" call above, so we can override default AspNet classes
	// configure StructureMap as per usual with registries / scans / manual configuration
	container.Configure(c =>
	{
		c.Scan(scan =>
		{
			scan.TheCallingAssembly();
			scan.WithDefaultConventions();
		});
		
        // instead of standard StructureMap.Dnx StructureMapServiceScopeFactory, need to use a multitenant version instead
		c.For<IServiceScopeFactory>().LifecycleIs(Lifecycles.Container).Use<AppMultiTenantStructureMapServiceScopeFactory>();
	});

	// Make sure we return an IServiceProvider, 
	// this makes DNX use the StructureMap container.
	return container.GetInstance<IServiceProvider>();
}
```

As we are configuring the StructureMap container `WithDefaultConventions()` the default implementation of `IMessageService` will
be `MessageService`.

## Overriding StructureMap config per tenant

For "Tenant 2", we have overriden the default configuration to use `OtherMesageService`.

There are a couple of ways to do this (Check "More Info" links below for details):

- configure the tenant's container on each request by overriding `MultiTenantStructureMapServiceScopeFactory<TTenant>.ConfigureTenantProfileContainer(IContainer tenantProfile, TenantContext<TTenant> tenantContext)`
    - eg as per sample `AppMultiTenantStructureMapServiceScopeFactory` code above
- configure each tenant's profile details once in Setup


