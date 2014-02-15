
using System.Collections.Generic;
namespace SaasKit
{
    public class Tenant : ITenant
    {
        public virtual string Name { get; set; }
        public virtual IEnumerable<string> RequestIdentifiers { get; set; }
    }
}
