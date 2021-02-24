using System;
using System.Collections.Generic;
using UniAgile.Dependency;

namespace UniAgile.Game
{
    public static class ApplicationExtensions
    {
        public static void Bind<T>(this List<IDependencyInfo>  dependencies,
                                   Func<IDependencyService, T> factory)
        {
            dependencies.Add(new DependencyInfo<T>(factory));
        }

        public static void Bind<T1, T2>(this List<IDependencyInfo>   dependencies,
                                        Func<IDependencyService, T1> factory)
            where T1 : T2
        {
            dependencies.Add(new DependencyInfo<T1>(factory));
            dependencies.Add(new DependencyInfo<T2>(ioc => ioc.Resolve<T1>()));
        }

        public static void Bind<T1, T2, T3>(this List<IDependencyInfo>   dependencies,
                                            Func<IDependencyService, T1> factory)
            where T1 : T2, T3
        {
            dependencies.Add(new DependencyInfo<T1>(factory));
            dependencies.Add(new DependencyInfo<T2>(ioc => ioc.Resolve<T1>()));
            dependencies.Add(new DependencyInfo<T3>(ioc => ioc.Resolve<T1>()));
        }
    }
}