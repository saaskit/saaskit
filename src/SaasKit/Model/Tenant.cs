
using System.Collections.Generic;
namespace SaasKit.Model
{
    public class BaseTenant : ITenant
    {
        public virtual string Id { get; set; }

        public override string ToString()
        {
            return string.Format("Tenant: {0}", Id);
        }
    }
}
