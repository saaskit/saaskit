using System.Collections.Generic;

namespace SaasKit
{
    public interface ITenant
    {
        string Name { get; }

        IEnumerable<string> RequestIdentifiers { get; }
    }
}
