using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SaasKit.Multitenancy.Tests
{
    public class StructureMapTests
    {
        [Fact]
        public void Nested_container_should_resolve_singleton_from_parent()
        {
            var container = new Container();
            container.Configure(cfg =>
            {
                cfg.ForSingletonOf<IMaterial>().Use<Wood>();
            });

            var tenant = container.CreateChildContainer();
            tenant.Configure(cfg =>
            {
                cfg.ForSingletonOf<IColor>().Use<Red>();
            });

            Assert.Equal(container.GetInstance<IMaterial>(), tenant.GetInstance<IMaterial>());

            var request = tenant.GetNestedContainer();

            //Assert.Same(tenant.GetInstance<IColor>(), request.GetInstance<IColor>());
        }

        [Fact]
        public void child_and_nested_container_usage_of_singletons()
        {
            var container = new Container();
            var child = container.CreateChildContainer();
            child.Configure(_ => { _.ForSingletonOf<IColorCache>().Use<ColorCache>(); });

            var singleton = child.GetInstance<IColorCache>();

            // SingletonThing's should be resolved from the child container
            using (var nested = child.GetNestedContainer())
            {
                // Fails, nested gets it's own instance
                Assert.Same(singleton, nested.GetInstance<IColorCache>());
            }
        }

        public interface IColor
        {

        }
        
        public interface IColorCache
        {

        }

        public class ColorCache : IColorCache
        {

        }

        public class Red : IColor
        {

        }

        public class Blue : IColor
        {

        }


        public interface IMaterial
        {

        }

        public class Wood : IMaterial
        {

        }
    }
}
