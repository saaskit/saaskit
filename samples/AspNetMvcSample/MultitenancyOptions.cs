using System.Collections.ObjectModel;

namespace AspNetMvcSample
{
    public class MultitenancyOptions
    {
        public Collection<AppTenant> Tenants { get; set; }
    }
}
