using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SaasKit.Multitenancy
{
   public class TenantNotFoundMiddleware<TTenant>
   {
      private readonly Func<IDictionary<string, object>, Task> next;

      private readonly Func<Task<TenantContext<TTenant>>> _tenantFunc;

      public TenantNotFoundMiddleware(
         Func<IDictionary<string, object>, Task> next,
         Func<Task<TenantContext<TTenant>>> tenantFunc)
      {
         Ensure.Argument.NotNull(next, "next");
         Ensure.Argument.NotNull(tenantFunc, "tenantFunc");
         this.next = next;
         this._tenantFunc = tenantFunc;
      }


      public async Task Invoke(IDictionary<string, object> environment){
         Ensure.Argument.NotNull(environment, "environment");

         var tenantContext = environment.GetTenantContext<TTenant>() ?? _tenantFunc().Result;

         if (tenantContext != null)
            environment.SetTenantContext(tenantContext); 

         await next(environment);

      }


   }
}
