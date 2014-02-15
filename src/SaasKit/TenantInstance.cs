using System;
using System.Collections.Generic;

namespace SaasKit
{
    /// <summary>
    /// Represents a running tenant instance.
    /// </summary>
    public class TenantInstance : IDisposable
    {
        private bool isDisposed;

        public Guid Id { get; private set; }
        public ITenant Tenant { get; private set; }

        public IDictionary<string, object> Properties { get; private set; }

        public TenantInstance(ITenant tenant)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException("tenant");
            }

            Tenant = tenant;
            Id = Guid.NewGuid();
            Properties = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            return string.Format("Tenant: {0} Instance: {1}", Tenant.Name, Id);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    try
                    {
                        // dispose container here
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }

                Console.WriteLine("Instance disposed");
                // set container to null 
                isDisposed = true;
            }
        }
    }
}
