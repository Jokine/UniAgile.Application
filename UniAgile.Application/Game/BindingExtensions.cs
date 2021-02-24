using System;
using System.Collections.Generic;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public static class BindingExtensions
    {
        public static IListenerHandle Bind<TContainingObject, TValue>(this
                                                                          TContainingObject containingObject,
                                                                      string                          fieldName,
                                                                      Func<TContainingObject, TValue> getMethod,
                                                                      Action<TValue>                  onChange,
                                                                      ISignal                         signal)
        {
            return new BindingListenerHandle<TContainingObject, TValue>(signal,
                                                                        new BindingInfo<TContainingObject, TValue>(containingObject, getMethod, onChange));
        }
    }


    internal class BindingInfo<TContainingObject, TValue>
    {
        public BindingInfo(TContainingObject               containingObject,
                           Func<TContainingObject, TValue> getValue,
                           Action<TValue>                  onChange)
        {
            ContainingObject = containingObject;
            GetValue         = getValue;
            OnChange         = onChange;
            CurrentValue     = getValue(containingObject);
        }

        private readonly TContainingObject               ContainingObject;
        private readonly Func<TContainingObject, TValue> GetValue;
        private readonly Action<TValue>                  OnChange;
        private          TValue                          CurrentValue;


        // todo: optimize, will be a bottleneck as it polls each binding each frame
        public void OnUpdate()
        {
            var newValue = GetValue(ContainingObject);

            if (!EqualityComparer<TValue>.Default.Equals(CurrentValue, newValue))
            {
                CurrentValue = newValue;
                OnChange(newValue);
            }
        }
    }

    internal class BindingListenerHandle<TContainingObject, TValue> : IListenerHandle
    {
        public BindingListenerHandle(IListenableSignal                      listenedSignal,
                                     BindingInfo<TContainingObject, TValue> bindingInfo)
        {
            ListenedSignal = listenedSignal;
            BindingInfo    = bindingInfo;
        }

        private readonly BindingInfo<TContainingObject, TValue> BindingInfo;
        private readonly IListenableSignal                      ListenedSignal;

        public bool IsSubscribed { get; private set; }

        public void Dispose()
        {
            Unsubscribe();
            GC.SuppressFinalize(this);
        }

        public void Subscribe()
        {
            ListenedSignal.AddListener(OnSignal);
            IsSubscribed = true;
        }

        public void Unsubscribe()
        {
            ListenedSignal.RemoveListener(OnSignal);
            IsSubscribed = false;
        }

        private void OnSignal()
        {
            BindingInfo.OnUpdate();
        }
    }
}