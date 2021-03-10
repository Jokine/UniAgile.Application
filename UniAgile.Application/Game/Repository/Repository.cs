using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UniAgile.Game
{
    public class Repository<T> : IDictionary<string, T>,
                                 IReadOnlyDictionary<string, T>,
                                 IRepository
        where T : struct
    {
        private readonly IDictionary<string, string> ChangeCache =
            new Dictionary<string, string>();

        private readonly IDictionary<string, T> CurrentData =
            new Dictionary<string, T>();

        private readonly List<DataChange<T>> DataChanges =
            new List<DataChange<T>>();


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
                Id = kvp.Key,
                Old = kvp.Value,
                ChangeType = ChangeType.Remove
            });

            DataChanges.AddRange(changes);

            CurrentData.Clear();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return CurrentData.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, T>[] array,
                           int arrayIndex)
        {
            CurrentData.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            return CurrentData.Remove(item);
        }

        public int Count => CurrentData.Count;
        public bool IsReadOnly => CurrentData.IsReadOnly;

        public void Add(string key,
                        T value)
        {
            if (CurrentData.ContainsKey(key))
                throw new Exception($"{typeof(T)} already has key {key}");


            CurrentData[key] = value;

            var dataChange = new DataChange<T>
            {
                Id = key,
                New = value,
                Old = default,
                ChangeType = ChangeType.Add
            };

            DataChanges.Add(dataChange);
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

            DataChanges.Add(new DataChange<T>
            {
                Id = key,
                Old = currentValue,
                New = default,
                ChangeType = ChangeType.Remove
            });

            return true;
        }

        public bool TryGetValue(string key,
                                out T value)
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
                                  DataChanges);
        }

        public ICollection<string> Keys => CurrentData.Keys;
        public ICollection<T> Values => CurrentData.Values;

        IEnumerable<string> IReadOnlyDictionary<string, T>.Keys => Keys;

        IEnumerable<T> IReadOnlyDictionary<string, T>.Values => Values;

        public void PopDataChangesNonAlloc(
            IDictionary<string, Notifiable> notifiables,
            List<INotifiableDataChange> list)
        {
            for (var i = DataChanges.Count - 1; i >= 0; i--)
            {
                var dc = DataChanges[i];

                if (ChangeCache.ContainsKey(dc.Id)) continue;

                var notifiableDataChange = new NotifiableDataChange<T>(
                 notifiables.GetOrCreateNotifiable(dc.Id),
                 dc);

                ChangeCache.Add(dc.Id, dc.Id);
                list.Add(notifiableDataChange);
            }

            // reusing the list
            ChangeCache.Clear();
            DataChanges.Clear();
        }

        public Type RepositoryType => typeof(T);


        private static void HandleDataChanges(string key,
                                              T value,
                                              IDictionary<string, T>
                                                  currentData,
                                              List<DataChange<T>> changes)
        {
            if (currentData.TryGetValue(key, out var currentValue))
            {
                if (!EqualityComparer<T>.Default.Equals(currentValue, value))
                {
                    currentData[key] = value;

                    changes.Add(new DataChange<T>
                    {
                        New = value,
                        Old = currentValue,
                        Id = key,
                        ChangeType = ChangeType.Change
                    });
                }
            }
            else
            {
                currentData[key] = value;

                changes.Add(new DataChange<T>
                {
                    New = value,
                    Old = default,
                    Id = key,
                    ChangeType = ChangeType.Add
                });
            }
        }

        public void AddRange(IEnumerable<T> enumerable,
                             Func<T, string> idSelector)
        {
            foreach (var e in enumerable) Add(idSelector(e), e);
        }
    }
}