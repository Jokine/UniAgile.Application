using System;

namespace UniAgile.Dependency
{
    // TODO: Async initialization pattern with success and fail return value, container making sure everything is correctly initialized before giving up control
    public interface IDependencyInfo
    {
        Func<IDependencyService, object> Factory { get; }
        Type Type { get; }
        bool IsLazy { get; }
    }

    public class DependencyInfo : IDependencyInfo
    {
        public DependencyInfo(Type type,
                              Func<IDependencyService, object> factory,
                              bool isLazy = false)
        {
            IsLazy = isLazy;
            Type = type ?? throw new NullReferenceException();
            Factory = factory ?? throw new NullReferenceException();
        }

        public Func<IDependencyService, object> Factory { get; }
        public Type Type { get; }
        public bool IsLazy { get; }
    }

    public class DependencyInfo<T> : DependencyInfo
    {
        public DependencyInfo(Func<IDependencyService, T> factory) : base(typeof(T), service => factory(service))
        {
        }

        public DependencyInfo(Func<IDependencyService, T> factory,
                              bool isLazy) : base(typeof(T), service => factory(service), isLazy)
        {
        }
    }
}