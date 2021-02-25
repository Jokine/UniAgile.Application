using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniAgile.Dependency;

#pragma warning disable 1998

namespace UniAgile.Game
{
    public class Application
    {
        internal Application()
        {
        }

        protected IDependencyService DependencyServiceField { get; private set; }
        public    ApplicationModel   ApplicationModel       { get; private set; }

        public static Application MainApplication   { get; private set; }
        public static bool        ApplicationExists => MainApplication != null;


        public static IDependencyService DependencyService
        {
            get
            {
                if (MainApplication == null) throw new Exception("Attempting to access applications dependency service before the application has been correctly initialized");

                return MainApplication.DependencyServiceField;
            }
        }

        private void OnInternalInitialized()
        {
            var dependencyList = GetDependencies();
            ApplicationModelDependencyInfo(dependencyList);
            DependencyServiceField = new DependencyService(dependencyList);
            ApplicationModel       = DependencyServiceField.Resolve<ApplicationModel>();
            OnInitialized();
        }

        public virtual void Start()
        {
        }

        public virtual void Reset()
        {
        }

        protected virtual void ApplicationModelDependencyInfo(List<IDependencyInfo> dependencyInfos)
        {
            dependencyInfos.Register(ioc => new ApplicationModel());
        }

        public virtual async Task Loop(TimeSpan deltaTime)
        {
        }

        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        ///     Starts a new application and assigns it as the singleton instance
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <returns></returns>
        public static TApplication Make<TApplication>()
            where TApplication : Application, new()
        {
            var app                                         = new TApplication();
            if (MainApplication == default) MainApplication = app;
            app.OnInternalInitialized();

            return app;
        }

        /// <summary>
        ///     Starts a new application and assigns it as the singleton instance
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <returns></returns>
        public static TApplication Make<TApplication>(Func<TApplication> factory)
            where TApplication : Application
        {
            var app = factory();

            if (MainApplication == default) MainApplication = app;
            app.OnInternalInitialized();

            return app;
        }

        /// <summary>
        ///     Starts a new application and assigns it as the singleton instance
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <returns></returns>
        public static Application Make(Func<Application> factory)
        {
            var app                                         = factory();
            if (MainApplication == default) MainApplication = app;
            app.OnInternalInitialized();

            return app;
        }

        /// <summary>
        ///     Starts a new application and assigns it as the singleton instance
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <returns></returns>
        public static Application Make(Type type)
        {
            var app                                         = Activator.CreateInstance(type);
            if (MainApplication == default) MainApplication = (Application) app;
            ((Application) app).OnInternalInitialized();

            return MainApplication;
        }

        protected virtual List<IDependencyInfo> GetDependencies()
        {
            return new List<IDependencyInfo>();
        }
    }
}