using System.Collections.Generic;
using UniAgile.Dependency;
using UniAgile.Testing;
using Xunit;

namespace UniAgile.Application.Tests.ApplicationTests
{
    public class Unit
    {
        [Fact]
        public void Dependency_service_can_resolve_a_registered_type()
        {
            var factoryMethod  = MockData.function_which_returns<DependencyServiceTests.Unit>();
            var dependencyList = new List<IDependencyInfo>();

            dependencyList.Add(new DependencyInfo<DependencyServiceTests.Unit>(s => factoryMethod.Object()));

            IDependencyService dependencyService = new DependencyService(dependencyList);
            var                outcome           = dependencyService.Resolve<DependencyServiceTests.Unit>();

            factoryMethod.is_called_once();
            outcome.is_not_null();
        }
    }
}