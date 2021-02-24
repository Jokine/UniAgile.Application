using System;

namespace UniAgile.Dependency
{
    public interface IDependencyService
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}