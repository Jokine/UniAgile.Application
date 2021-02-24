using System;
using System.Collections.Generic;
using System.Linq;

namespace UniAgile.Dependency
{
    public class DependencyService : IDependencyService
    {
        public DependencyService(IReadOnlyList<IDependencyInfo> dependencies)
        {
            if (dependencies == null) throw new NullReferenceException();

            for (var i = 0; i < dependencies.Count; i++)
            {
                var dependency     = dependencies[i];
                var dependencyType = dependency.Type;

                if (Dependencies.ContainsKey(dependencyType)
                    || Singletons.ContainsKey(dependencyType)) throw new Exception($"Dependency {dependencyType} has already been registered");

                Dependencies.Add(dependencyType, dependency);
                if (!dependency.IsLazy) NonLazyDependencies.Add(dependency);
            }

            for (var i = 0; i < NonLazyDependencies.Count; i++)
                GetOrCreate(this,
                            NonLazyDependencies[i],
                            Singletons,
                            CurrentlyResolving);
        }

        private readonly List<Type> CurrentlyResolving = new List<Type>();

        private readonly Dictionary<Type, IDependencyInfo> Dependencies        = new Dictionary<Type, IDependencyInfo>();
        private readonly List<IDependencyInfo>             NonLazyDependencies = new List<IDependencyInfo>();
        private readonly Dictionary<Type, object>          Singletons          = new Dictionary<Type, object>();

        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if (!Dependencies.ContainsKey(type))
            {
                if (!type.IsAbstract
                    && !type.IsInterface)
                {
                    var constructorWithMostparams = type.GetConstructors()
                                                        .Aggregate((best,
                                                                    next) =>
                                                                       best.GetParameters().Length > next.GetParameters().Length ? best : next);

                    var parameters = constructorWithMostparams.GetParameters();

                    Dependencies.Add(type,
                                     new DependencyInfo(type,
                                                        service =>
                                                        {
                                                            try
                                                            {
                                                                return constructorWithMostparams.Invoke(parameters
                                                                                                        .Select(pInfo => service.Resolve(pInfo.ParameterType))
                                                                                                        .ToArray());
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                throw new Exception(
                                                                                    $"Unable to automatically resolve dependency of type {type.Name}\nOriginal exception: {e.Message}");
                                                            }
                                                        },
                                                        true));
                }
                else
                {
                    throw new Exception(
                                        $"Cannot resolve type {type.Name} as it has not been registered and it cannot be auto resolved");
                }
            }

            return GetOrCreate(this,
                               Dependencies[type],
                               Singletons,
                               CurrentlyResolving);
        }

        private static object GetOrCreate(IDependencyService       service,
                                          IDependencyInfo          dependency,
                                          Dictionary<Type, object> singletons,
                                          List<Type>               currentlyResolving)
        {
            if (!singletons.ContainsKey(dependency.Type)) singletons.Add(dependency.Type, Resolve(service, dependency, currentlyResolving));


            return singletons[dependency.Type];
        }

        private static object Resolve(IDependencyService service,
                                      IDependencyInfo    dependency,
                                      List<Type>         currentlyResolving)
        {
            if (currentlyResolving.Contains(dependency.Type))
                throw new Exception(
                                    $"You have circular dependency with type {dependency.Type} calling the same {nameof(Resolve)} method. This would cause a stack overflow");

            currentlyResolving.Add(dependency.Type);
            var dependencyInstance = dependency.Factory(service);

            if (dependencyInstance == null) throw new Exception($"Created instance was null for type {dependency.Type}");

            currentlyResolving.Remove(dependency.Type);


            return dependencyInstance;
        }
    }
}