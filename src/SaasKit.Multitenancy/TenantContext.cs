using System;
using System.Collections.Generic;

namespace SaasKit.Multitenancy
{
    public class TenantContext<TTenant> : IDisposable
    {
        public TenantContext(TTenant tenant)
        {
            Ensure.Argument.NotNull(tenant, "tenant");

            Tenant = tenant;
            Properties = new Dictionary<string, object>();
        }
        
        public TTenant Tenant { get; private set; }
        public IDictionary<string, object> Properties { get; private set; }

        public void Dispose()
        {
            foreach (var prop in Properties)
            {
                TryDispose(prop.Value as IDisposable);
            }
        }

        private void TryDispose(IDisposable obj)
        {
            if (obj == null)
            {
                return;
            }
            
            try
            {
                obj.Dispose();
            }
            catch (ObjectDisposedException) { }
        }
    }
}
