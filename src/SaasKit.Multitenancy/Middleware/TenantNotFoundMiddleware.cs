using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace SaasKit.Multitenancy
{
   public class TenantNotFoundMiddleware<TTenant>
   {
      private readonly Func<IDictionary<string, object>, Task> next;
      private readonly Func<TTenant> tenantFunc;

      public TenantNotFoundMiddleware(Func<IDictionary<string, object>, Task> next,
         Func<TTenant> tenantFunc )
      {
         Ensure.Argument.NotNull(next, "next");
         Ensure.Argument.NotNull(tenantFunc, "tenantFunc");
         this.next = next;
         this.tenantFunc = tenantFunc;
      }


      public async Task Invoke(IDictionary<string, object> environment){
         Ensure.Argument.NotNull(environment, "environment");

         var tenantContext = environment.GetTenantContext<TTenant>();

         if (tenantContext == null)
         {

            tenantFunc();

            return;
         }
         await next(environment);

      }


   }
}
