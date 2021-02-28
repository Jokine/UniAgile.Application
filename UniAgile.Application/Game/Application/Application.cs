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
        public Application(ApplicationModel applicationModel, List<IDependencyInfo> dependencyList)
        {
            ApplicationModel = applicationModel;
            dependencyList.Add(new DependencyInfo<ApplicationModel>((ioc) => ApplicationModel));
            DependencyService = new DependencyService(dependencyList);
        }

        public IDependencyService DependencyService { get; private set; }
        public ApplicationModel   ApplicationModel  { get; private set; }
        
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