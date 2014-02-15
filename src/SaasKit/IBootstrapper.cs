using System;

namespace SaasKit
{
    public interface ISaasKitBootstrapper : IDisposable
    {
        void Initialize();

        ISaasKitEngine GetEngine();
    }
}
