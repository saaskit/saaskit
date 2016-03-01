using System.Collections.ObjectModel;

namespace AspNetMvcAuthSample
{
    public class MultitenancyOptions
    {
        public Collection<AppTenant> Tenants { get; set; }
    }
}
