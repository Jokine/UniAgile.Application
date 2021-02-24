using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniAgile.Dependency;
#pragma warning disable 1998

namespace UniAgile.Game
{
    public abstract class Application
    {
        internal Application()
        {
        }

        private   IDependencyService DependencyServiceField;
        // protected UpdateUISignal     UpdateUiSignal;

        public static Application Instance          { get; private set; }
        public static bool        ApplicationExists => Instance != null;


        public static IDependencyService DependencyService
        {
            get
            {
                if (Instance == null) throw new Exception("Attempting to access applications dependency service before the application has been correctly initialized");

                return Instance.DependencyServiceField;
            }
        }

        private void OnInternalInitialized()
        {
            var dependencyList = GetDependencies();
            ApplicationModelDependencyInfo(dependencyList);
            // UpdateUiSignal = new UpdateUISignal();
            // dependencyList.Bind(ioc => UpdateUiSignal);
            DependencyServiceField = new DependencyService(dependencyList);
            OnInitialized();
        }

        public abstract void Start();

        public abstract void Reset();

        protected void NotifyChanges()
        {
        }

        protected abstract void ApplicationModelDependencyInfo(List<IDependencyInfo> dependencyInfos);

        // public IListenerHandle ListenForChanges<TContainingObject, T>(TContainingObject containingObject, Func<TContainingObject, T> getvalue, Action<>)

        public virtual async Task Loop(TimeSpan deltaTime)
        {
        }

        protected abstract void OnInitialized();

        /// <summary>
        ///     Starts a new application and assigns it as the singleton instance
        /// </summary>
        /// <typeparam name="TApplication"></typeparam>
        /// <returns></returns>
        public static TApplication Make<TApplication>()
            where TApplication : Application, new()
        {
            var app = new TApplication();
            Instance = app;
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
            Instance = app;
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
            var app = factory();
            Instance = app;
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
            var app = Activator.CreateInstance(type);
            Instance = (Application) app;
            Instance.OnInternalInitialized();

            return Instance;
        }

        protected abstract List<IDependencyInfo> GetDependencies();
    }
}