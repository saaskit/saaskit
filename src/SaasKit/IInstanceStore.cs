using System;
using SaasKit.Model;

namespace SaasKit
{
    public interface IInstanceStore<T> 
        where T : ITenant 
    {
        T Get(string requestIdentifier);
        void Add(T instance, Action<string, T> removedCallback);
        void Remove(string instanceId);
    }
}
