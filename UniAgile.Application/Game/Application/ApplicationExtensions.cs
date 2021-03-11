using System;
using System.Collections.Generic;
using UniAgile.Dependency;

namespace UniAgile.Game
{
    public static class ApplicationExtensions
    {
        public static void ApplyRepositoryRule(
            this List<Func<IDependencyService, Type, IDependencyInfo>> automaticRules)
        {
            automaticRules.Add(CreateDependencyInfoForDictionaryValueTypeOrDefault);
        }

        private static bool IsAnyDictionary(Type genericTypeDefinition)
        {
            return genericTypeDefinition == typeof(IDictionary<,>)
                   || genericTypeDefinition == typeof(IReadOnlyDictionary<,>);
        }
        private static IDependencyInfo CreateDependencyInfoForDictionaryValueTypeOrDefault(
            IDependencyService dependencyService,
            Type type)
        {
            if (type.IsGenericType
                && IsAnyDictionary(type.GetGenericTypeDefinition()))
            {
                // taking the second type which is for value
                var appModel = dependencyService.Resolve<ApplicationModel>();
                var valueType = type.GetGenericArguments()[1];

                return new DependencyInfo(type,
                                          service => service.Resolve<ApplicationModel>().GetRepository(valueType));
            }

            return default;
        }

        public static void Register<T>(this List<IDependencyInfo> dependencies,
                                       Func<IDependencyService, T> factory)
        {
            dependencies.Add(new DependencyInfo<T>(factory));
        }

        public static void Register<T1, T2>(this List<IDependencyInfo> dependencies,
                                            Func<IDependencyService, T2> factory)
            where T2 : T1
        {
            dependencies.Add(new DependencyInfo<T2>(factory));
            dependencies.Add(new DependencyInfo<T1>(ioc => ioc.Resolve<T2>()));
        }

        public static void Register<T1, T2, T3>(this List<IDependencyInfo> dependencies,
                                                Func<IDependencyService, T3> factory)
            where T3 : T1, T2
        {
            dependencies.Add(new DependencyInfo<T3>(factory));
            dependencies.Add(new DependencyInfo<T2>(ioc => ioc.Resolve<T3>()));
            dependencies.Add(new DependencyInfo<T1>(ioc => ioc.Resolve<T3>()));
        }
    }
}