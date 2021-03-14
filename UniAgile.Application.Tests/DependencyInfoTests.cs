using System;
using System.Collections.Generic;
using Moq;
using UniAgile.Dependency;
using UniAgile.Game;
using UniAgile.Game.Integration;
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

            dependencyList.Add(new DependencyInfo<Unit>(s => factoryMethod.Object()));

            IDependencyService dependencyService = new DependencyService(dependencyList);

            var outcome = dependencyService.Resolve<Unit>();

            factoryMethod.is_called_once();
            outcome.is_not_null();
        }

        [Fact]
        public void Dependency_service_registers_everything_as_singletons()
        {
            var factoryMethod = MockData.function_which_returns<Unit>();
            var dependencyList = new List<IDependencyInfo>();

            dependencyList.Add(new DependencyInfo<Unit>(s => factoryMethod.Object()));

            IDependencyService dependencyService = new DependencyService(dependencyList);

            var outcome1 = dependencyService.Resolve<Unit>();
            var outcome2 = dependencyService.Resolve<Unit>();

            Assert.Equal(outcome1, outcome2);
        }

        [Fact]
        public void Dependency_service_can_resolve_concrete_classes_without_registering()
        {
            var dependencyList = new List<IDependencyInfo>();

            IDependencyService dependencyService = new DependencyService(dependencyList);

            var outcome = dependencyService.Resolve<Unit>();

            outcome.is_not_null();
        }

        [Fact]
        public void Dependency_service_can_lazily_create_registered_types()
        {
            var factoryMethod = MockData.function_which_returns<Unit>();
            var dependencyList = new List<IDependencyInfo>();

            dependencyList.Add(new DependencyInfo<Unit>(s => factoryMethod.Object(), true));

            IDependencyService dependencyService = new DependencyService(dependencyList);

            factoryMethod.is_not_called();

            var outcome = dependencyService.Resolve<Unit>();
            outcome.is_not_null();

            factoryMethod.is_called_once();
        }

        [Fact]
        public void Dependency_service_can_register_integrations()
        {
            var dependencyList = new List<IDependencyInfo>();
            var test1Key = "Test1";
            var test2Key = "Test2";
            
            dependencyList.RegisterIntegration<ITestIntegration>(new []
            {
                ApplicationExtensions.CreateIntegration<TestIntegration1>(test1Key),
                ApplicationExtensions.CreateIntegration<TestIntegration2>(test2Key)
            });


            var dependencyService = new DependencyService(dependencyList);

            var integration = dependencyService.Resolve<Integrations<ITestIntegration>>();
            Assert.Equal(typeof(TestIntegration1), integration.IntegrationMappings[test1Key].GetType());
            Assert.Equal(typeof(TestIntegration2), integration.IntegrationMappings[test2Key].GetType());
        }

        public class TestIntegration1 : ITestIntegration
        {
            
        }
        
        public class TestIntegration2 : ITestIntegration
        {
            
        }
        
        public interface ITestIntegration
        {
            
        }


        [Fact]
        public void Dependency_service_can_add_automatic_resolving_rules()
        {
            var dependencyList = new List<IDependencyInfo>();
            dependencyList.Register(service => new ApplicationModel(new IRepository[0]));
            var automaticRules = new List<Func<IDependencyService, Type, IDependencyInfo>>();

            automaticRules.ApplyRepositoryRule();

            var dependencyService = new DependencyService(dependencyList, automaticRules);

            var test = dependencyService.Resolve<TestClass>();
            Assert.NotNull(test);
            Assert.NotNull(test.Repo);
            Assert.NotNull(test.ReadOnlyRepo);
        }

        private class TestClass
        {
            public TestClass(IDictionary<string, int> repo,
                             IReadOnlyDictionary<string, int> readOnlyRepo)
            {
                Repo = repo;
                ReadOnlyRepo = readOnlyRepo;
            }

            public readonly IReadOnlyDictionary<string, int> ReadOnlyRepo;
            public readonly IDictionary<string, int> Repo;
        }
    }
}