using System;
using System.Collections.Specialized;

namespace UniAgile.Game
{
    public interface INotifiableDataChange : IDisposable
    {
        void Notify();
    }

    public class Notifiable : INotifyCollectionChanged

    {
        private static readonly NotifyCollectionChangedEventArgs DummyArgs =
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Invoke(object param)
        {
            CollectionChanged?.Invoke(param, DummyArgs);
        }

        public void RemoveAllListeners()
        {
            CollectionChanged = null;
        }
    }

    internal class NotifiableDataChange<T> : INotifiableDataChange, IDataChange<T>
        where T : struct
    {
        public NotifiableDataChange(Notifiable notifiable,
                                    DataChange<T> dataChange)
        {
            Notifiable = notifiable ?? throw new NullReferenceException();
            DataChange = dataChange;
        }

        private readonly DataChange<T> DataChange;

        private readonly Notifiable Notifiable;

        public ChangeType ChangeType => DataChange.ChangeType;

        public string Id => DataChange.Id;

        public T New => DataChange.New;

        public T Old => DataChange.Old;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public void Notify()
        {
            Notifiable.Invoke(this);
        }
    }
}