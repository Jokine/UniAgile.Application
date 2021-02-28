using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace UniAgile.Game
{
    public class ApplicationModel : IReadOnlyDictionary<string, INotifyCollectionChanged>
    {
        public ApplicationModel(IReadOnlyList<IRepository> repositories)
        {
            Repositories = new Dictionary<Type, IRepository>(repositories.Count);

            foreach (var repository in repositories) Repositories.Add(repository.RepositoryType, repository);
        }

        private readonly List<INotifiableDataChange>     NotifiableDataChangesCachedList = new List<INotifiableDataChange>();
        private readonly IDictionary<string, Notifiable> Notifiables                     = new Dictionary<string, Notifiable>();

        private readonly IDictionary<Type, IRepository> Repositories;

        public INotifyCollectionChanged this[string key]
        {
            get
            {
                TryGetValue(key, out var notifiable);

                return notifiable;
            }
        }

        public IEnumerator<KeyValuePair<string, INotifyCollectionChanged>> GetEnumerator()
        {
            return Notifiables.Select(kvp => new KeyValuePair<string, INotifyCollectionChanged>(kvp.Key, kvp.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Notifiables.Count;

        public bool ContainsKey(string key)
        {
            return Notifiables.ContainsKey(key);
        }

        public bool TryGetValue(string                       key,
                                out INotifyCollectionChanged value)
        {
            if (!Notifiables.TryGetValue(key, out var notifiable))
            {
                // we know the type is NotifiableDataChange<
                notifiable = new Notifiable();
                Notifiables.Add(key, notifiable);
            }

            value = notifiable;

            return true;
        }

        public IEnumerable<string>                   Keys   => Notifiables.Keys;
        public IEnumerable<INotifyCollectionChanged> Values => Notifiables.Select(kvp => (INotifyCollectionChanged) kvp.Value);

        public IDictionary<string, T> GetRepository<T>()
            where T : struct
        {
            if (!Repositories.TryGetValue(typeof(T), out var repository))
            {
                repository = new Repository<T>();
                Repositories.Add(typeof(T), repository);
                
            }
            
            return (Repository<T>) repository;
        }

        public void NotifyChanges()
        {
            foreach (var repoKvp in Repositories) repoKvp.Value.PopDataChangesNonAlloc(Notifiables, NotifiableDataChangesCachedList);

            for (var i = 0; i < NotifiableDataChangesCachedList.Count; i++)
                using (var notifiableDataChange = NotifiableDataChangesCachedList[i])
                {
                    notifiableDataChange.Notify();
                }

            // can be possibly optimized with reverse for loop and removal while iterating
            NotifiableDataChangesCachedList.Clear();
        }
    }
}