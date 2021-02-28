using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace UniAgile.Game
{
    public class ApplicationModel : IReadOnlyDictionary<string, INotifyCollectionChanged>
    {
        private readonly IDictionary<string, Notifiable> Notifiables                     = new Dictionary<string, Notifiable>();
        private          List<INotifiableDataChange>     NotifiableDataChangesCachedList = new List<INotifiableDataChange>();
        
        protected        IReadOnlyList<IRepository>      Repositories { get; set; } = new IRepository[0];

        public INotifyCollectionChanged this[string key]
        {
            get
            {
                var result = TryGetValue(key, out var notifiable);

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

        public void Clear()
        {
            if (Repositories == null) return;

            foreach (var rep in Repositories) rep.Clear();
            NotifyChanges();
        }


        public KeyValuePair<string, T> GetModel<T>(string key)
            where T : struct
        {
            try
            {
                return new KeyValuePair<string, T>(key, GetRepository<T>()[key]);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to get model {key} due to an error. Original message: {e.Message}");
            }
        }

        public KeyValuePair<string, T> GetModelOrDefault<T>(string key)
            where T : struct
        {
            try
            {
                return new KeyValuePair<string, T>(key, GetRepository<T>()[key]);
            }
            catch (Exception)
            {
                return default;
            }
        }

        // todo: composite keys should later on to be cached and made into indices
        private string FormId(Type   type,
                              string baseId)
        {
            return $"{type}.{baseId}";
        }


        private IDictionary<string, T> GetRepository<T>()
            where T : struct
        {
            return (Repository<T>) Repositories.First(r => r.RepositoryType == typeof(T));
        }

        public void NotifyChanges()
        {
            for (int i = 0; i < Repositories.Count; i++)
            {
                Repositories[i].PopDataChangesNonAlloc(Notifiables, NotifiableDataChangesCachedList);
            }

            for (int i = 0; i < NotifiableDataChangesCachedList.Count; i++)
            {
                using (var notifiableDataChange = NotifiableDataChangesCachedList[i]) 
                {
                    notifiableDataChange.Notify();
                }
            }

            // can be possibly optimized with reverse for loop and removal while iterating
            NotifiableDataChangesCachedList.Clear();
        }
    }
}