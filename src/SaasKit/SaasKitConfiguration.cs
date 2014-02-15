
using System;
namespace SaasKit
{
    public class SaasKitConfiguration
    {
        public RequestIdentificationStrategy IdentificationStrategy { get; set; }

        // TODO - this may need to be a Func<ITenantResolver>
        public ITenantResolver TenantResolver { get; set; }

        public Action<string> Logger { get; set; }

        public SaasKitConfiguration()
        {
            IdentificationStrategy = RequestIdentificationStategies.FromHostname;
            Logger = log => { };
        }

        public bool IsValid
        {
            get
            {
                // TODO
                return true;
            }
        }
    }
}
