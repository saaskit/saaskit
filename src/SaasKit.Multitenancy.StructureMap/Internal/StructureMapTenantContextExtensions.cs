using StructureMap;

namespace SaasKit.Multitenancy.StructureMap.Internal
{
    internal static class StructureMapTenantContextExtensions
    {
        private const string TenantContainerKey = "saaskit.TenantContainer";

        public static IContainer GetTenantContainer<TTenant>(this TenantContext<TTenant> tenantContext)
        {
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));

            object tenantContainer;
            if (tenantContext.Properties.TryGetValue(TenantContainerKey, out tenantContainer))
            {
                return tenantContainer as IContainer;
            }

            return null;
        }

        public static void SetTenantContainer<TTenant>(this TenantContext<TTenant> tenantContext, IContainer tenantContainer)
        {
            Ensure.Argument.NotNull(tenantContext, nameof(tenantContext));
            Ensure.Argument.NotNull(tenantContainer, nameof(tenantContainer));

            tenantContext.Properties[TenantContainerKey] = tenantContainer;
        }
    }
}
