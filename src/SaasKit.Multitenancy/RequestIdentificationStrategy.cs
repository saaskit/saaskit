using System.Collections.Generic;

namespace SaasKit.Multitenancy
{
    public delegate string RequestIdentificationStrategy(IDictionary<string, object> environment);
}
