using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace UniAgile.Game
{
    public class Repository<T> : IDictionary<string, T>, INotifyCollectionChanged, IReadOnlyDictionary<string, T>, IRepository
        where T : struct
    {
        private static readonly NotifyCollectionChangedEventArgs AddEvent     = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add);
        private static readonly NotifyCollectionChangedEventArgs RemoveEvent  = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove);
        private static readonly NotifyCollectionChangedEventArgs ReplaceEvent = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace);

        public Repository()
        {
            Changes = new List<DataChange<T>>();
        }

        private readonly IDictionary<string, bool> AlreadyNotified = new Dictionary<string, bool>();

        private readonly List<DataChange<T>> Changes;

        private readonly IDictionary<string, T> CurrentData = new Dictionary<string, T>();

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return CurrentData.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, T> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            var changes = CurrentData.Select(kvp => new DataChange<T>
            {
                Id         = kvp.Key,
                Old        = kvp.Value,
                ChangeType = ChangeType.Remove
            });

            Changes.AddRange(changes);

            CurrentData.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return CurrentData.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, T>[] array,
                           int                       arrayIndex)
        {
            CurrentData.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return CurrentData.Remove(item);
        }

        public int  Count      => CurrentData.Count;
        public bool IsReadOnly => CurrentData.IsReadOnly;

        public void Add(string key,
                        T      value)
        {
            if (CurrentData.ContainsKey(key)) throw new Exception($"{typeof(T)} already has key {key}");


            CurrentData[key] = value;

            Changes.Add(new DataChange<T>
            {
                Id         = key,
                New        = value,
                Old        = default,
                ChangeType = ChangeType.Add
            });
        }

        public bool ContainsKey(string key)
        {
            return CurrentData.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            T currentValue = default;

            if (!CurrentData.TryGetValue(key, out currentValue)) return false;

            CurrentData.Remove(key);

            Changes.Add(new DataChange<T>
            {
                Id         = key,
                Old        = currentValue,
                New        = default,
                ChangeType = ChangeType.Remove
            });

            return true;
        }

        public bool TryGetValue(string key,
                                out T  value)
        {
            return CurrentData.TryGetValue(key, out value);
        }

        public T this[string key]
        {
            get
            {
                try
                {
                    return CurrentData[key];
                }
                catch (Exception)
                {
                    Add(key, default);

                    return default;
                }
            }
            set =>
                HandleDataChanges(key,
                                  value,
                                  CurrentData,
                                  Changes);
        }

        public ICollection<string> Keys   => CurrentData.Keys;
        public ICollection<T>      Values => CurrentData.Values;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        IEnumerable<string> IReadOnlyDictionary<string, T>.Keys => Keys;

        IEnumerable<T> IReadOnlyDictionary<string, T>.Values => Values;

        public Type RepositoryType => typeof(T);

        public void NotifyChanges()
        {
            // reversing so the most recent change is found first and multiple change notifications to same value can be skipped
            for (var i = Changes.Count - 1; i >= 0; i--)
            {
                var change = Changes[i];

                if (AlreadyNotified.ContainsKey(change.Id)) continue;

                AlreadyNotified.Add(change.Id, true);
                CollectionChanged?.Invoke(change.New, ChooseArgs(change.ChangeType));
            }

            // reusing the list
            AlreadyNotified.Clear();
            Changes.Clear();
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

        private static void HandleDataChanges(string                 key,
                                              T                      value,
                                              IDictionary<string, T> currentData,
                                              List<DataChange<T>>    changes)
        {
            if (currentData.TryGetValue(key, out var currentValue))
            {
                if (!EqualityComparer<T>.Default.Equals(currentValue, value))
                {
                    currentData[key] = value;

                    changes.Add(new DataChange<T>
                    {
                        New        = value,
                        Old        = currentValue,
                        Id         = key,
                        ChangeType = ChangeType.Change
                    });
                }
            }
            else
            {
                currentData[key] = value;

                changes.Add(new DataChange<T>
                {
                    New        = value,
                    Old        = default,
                    Id         = key,
                    ChangeType = ChangeType.Add
                });
            }
        }

        public void AddRange(IEnumerable<T>  enumerable,
                             Func<T, string> idSelector)
        {
            foreach (var e in enumerable) Add(idSelector(e), e);
        }
    }
}