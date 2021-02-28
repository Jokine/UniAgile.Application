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
        private static readonly NotifyCollectionChangedEventArgs AddEvent     = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add);
        private static readonly NotifyCollectionChangedEventArgs RemoveEvent  = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove);
        private static readonly NotifyCollectionChangedEventArgs ReplaceEvent = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace);

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Invoke(object     param,
                           ChangeType changeType)
        {
            CollectionChanged?.Invoke(param, ChooseArgs(changeType));
        }

        private static NotifyCollectionChangedEventArgs ChooseArgs(ChangeType changeType)
        {
            switch (changeType)
            {
                case ChangeType.Change: return ReplaceEvent;
                case ChangeType.Add:    return AddEvent;
                case ChangeType.Remove: return RemoveEvent;
                default:                throw new ArgumentOutOfRangeException(nameof(changeType), changeType, null);
            }
        }
    }

    internal class NotifiableDataChange<T> : INotifiableDataChange, IDataChange<T>
        where T : struct
    {
        public NotifiableDataChange(Notifiable    notifiable,
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
            Notifiable.Invoke(this, DataChange.ChangeType);
        }
    }
}