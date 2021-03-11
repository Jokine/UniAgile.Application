using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniAgile.Dependency;

// ReSharper disable VirtualMemberCallInConstructor

#pragma warning disable 1998

namespace UniAgile.Game
{
    public class Application
    {
        public Application(ApplicationModel applicationModel,
                           List<IDependencyInfo> dependencyList,
                            List<Func<IDependencyService, Type, IDependencyInfo>> automaticRules)
        {
            ApplicationModel = applicationModel;

            dependencyList.Add(new DependencyInfo<ApplicationModel>(ioc => ApplicationModel));

            DependencyService = new DependencyService(dependencyList, automaticRules);
        }

        public IDependencyService DependencyService { get; }
        public ApplicationModel ApplicationModel { get; protected set; }

        public virtual void Start()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual async Task Loop(TimeSpan deltaTime)
        {
        }
    }
}