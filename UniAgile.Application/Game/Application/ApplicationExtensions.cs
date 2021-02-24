using System;
using System.Collections.Generic;
using UniAgile.Dependency;

namespace UniAgile.Game
{
    public static class ApplicationExtensions
    {
        public static void Register<T>(this List<IDependencyInfo>  dependencies,
                                       Func<IDependencyService, T> factory)
        {
            dependencies.Add(new DependencyInfo<T>(factory));
        }

        public static void Register<T1, T2>(this List<IDependencyInfo>   dependencies,
                                            Func<IDependencyService, T2> factory)
            where T2 : T1
        {
            dependencies.Add(new DependencyInfo<T2>(factory));
            dependencies.Add(new DependencyInfo<T1>(ioc => ioc.Resolve<T2>()));
        }

        public static void Register<T1, T2, T3>(this List<IDependencyInfo>   dependencies,
                                                Func<IDependencyService, T3> factory)
            where T3 : T1, T2
        {
            dependencies.Add(new DependencyInfo<T3>(factory));
            dependencies.Add(new DependencyInfo<T2>(ioc => ioc.Resolve<T3>()));
            dependencies.Add(new DependencyInfo<T1>(ioc => ioc.Resolve<T3>()));
        }
    }
}