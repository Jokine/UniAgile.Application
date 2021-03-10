using System.Collections.Generic;
using UniAgile.Dependency;
using UniAgile.Testing;
using Xunit;

namespace UniAgile.Application.Tests.DependencyServiceTests
{
    public class Unit
    {
        [Fact]
        public void Dependency_service_can_resolve_a_registered_type()
        {
            var factoryMethod = MockData.function_which_returns<Unit>();
            var dependencyList = new List<IDependencyInfo>();

            dependencyList.Add(new DependencyInfo<Unit>(s => factoryMethod
                                   .Object()));

            IDependencyService dependencyService =
                new DependencyService(dependencyList);

            var outcome = dependencyService.Resolve<Unit>();

            factoryMethod.is_called_once();
            outcome.is_not_null();
        }

        [Fact]
        public void Dependency_service_registers_everything_as_singletons()
        {
            var factoryMethod = MockData.function_which_returns<Unit>();
            var dependencyList = new List<IDependencyInfo>();

            dependencyList.Add(new DependencyInfo<Unit>(s => factoryMethod
                                   .Object()));

            IDependencyService dependencyService =
                new DependencyService(dependencyList);

            var outcome1 = dependencyService.Resolve<Unit>();
            var outcome2 = dependencyService.Resolve<Unit>();

            Assert.Equal(outcome1, outcome2);
        }

        [Fact]
        public void
            Dependency_service_can_resolve_concrete_classes_without_registering()
        {
            var dependencyList = new List<IDependencyInfo>();

            IDependencyService dependencyService =
                new DependencyService(dependencyList);

            var outcome = dependencyService.Resolve<Unit>();

            outcome.is_not_null();
        }

        [Fact]
        public void Dependency_service_can_lazily_create_registered_types()
        {
            var factoryMethod = MockData.function_which_returns<Unit>();
            var dependencyList = new List<IDependencyInfo>();

            dependencyList.Add(new DependencyInfo<Unit>(s => factoryMethod
                                       .Object(),
                                   true));

            IDependencyService dependencyService =
                new DependencyService(dependencyList);

            factoryMethod.is_not_called();

            var outcome = dependencyService.Resolve<Unit>();
            outcome.is_not_null();

            factoryMethod.is_called_once();
        }
    }
}