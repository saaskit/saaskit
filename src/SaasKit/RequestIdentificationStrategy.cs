using Microsoft.Owin;

namespace SaasKit
{
    /// <summary>
    /// Defines a strategy for obtaining a tenant identifier from the <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <returns></returns>
    public delegate string RequestIdentificationStrategy(IOwinRequest request);
}
