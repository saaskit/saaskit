# ASP.NET Core MVC Google Authentication Sample

This sample demonstrates how to use ASP.NET Core authentication middleware in a multi-tenant application.

The authentication middleware was not designed with multi-tenancy in mind and as such is configured once per application.

This presents a problem in multi-tenant applications where the configuration needs to be specific to each tenant.

This sample uses SaasKit's middleware pipelines feature to create a separate request pipeline per tenant, each containing an instance of the authentication middleware, configured using that tenant's Google credentials.

It was blogged about in more detail [here](http://benfoster.io/blog/aspnet-core-multi-tenant-middleware-pipelines).

### Running the sample

The URLs for each tenant are configured in `appsettings.json`. To log in with Google you will need to register your applications in the [Google Developer Console](https://console.developers.google.com/start) and obtain the Client ID and Client Secret.

These should be added to user secrets as below:

Client ID:

```
dotnet user-secrets set TenantX:GoogleClientId xxx
```

Client Secret:

```
dotnet user-secrets set Tenant2:GoogleClientSecret xxx
```

Once done, you should be able to authenticate with Google on each of the tenant's log in pages.