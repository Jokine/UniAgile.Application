using System;
using System.Collections.Generic;
using System.Linq;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public abstract class ApplicationModel : IDataRecorder
    {
        private readonly IDictionary<string, IListenableSignal> Signals = new Dictionary<string, IListenableSignal>();
        protected        ulong                                  CurrentDataCommitId { get; private set; }


        protected IReadOnlyList<IRepository> Repositories { get; set; } = new IRepository[0];

        public void CommitRecord<T>(ulong                        commitId,
                                    IReadOnlyList<DataChange<T>> dataChanges)
            where T : struct
        {
            // todo: save the data changes for rollbacking
            // remember memory limits, serialization and delta rollbacking

            // NotifyChanges();
        }

        public IListenerHandle BindToDataChange<T>(string    id,
                                                   Action<T> listener)
            where T : struct
        {
            id = FormId(typeof(T), id);

            Signal<T> signal;

            if (!Signals.TryGetValue(id, out var uncastSignal)) signal = new Signal<T>();
            else signal                                                = (Signal<T>) uncastSignal;

            return new ListenerHandle<T>(signal, listener);
        }

        public IListenerHandle BindToDataChange<T>(string                          id,
                                                   Action<KeyValuePair<string, T>> listener)
            where T : struct
        {
            id = FormId(typeof(T), id);

            Signal<T> signal;

            if (!Signals.TryGetValue(id, out var uncastSignal)) signal = new Signal<T>();
            else signal                                                = (Signal<T>) uncastSignal;

            return new KvpListenerHandle<T>(signal, id, listener);
        }

        private void NotifyChanges()
        {
            for (var i = 0; i < Repositories.Count; ++i) Repositories[i].CommitData(CurrentDataCommitId, this);
            CurrentDataCommitId++;
        }

        public void Clear()
        {
            if (Repositories == null) return;

            foreach (var rep in Repositories) rep.Clear();
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


        private Repository<T> GetRepository<T>()
            where T : struct
        {
            return (Repository<T>) Repositories.First(r => r.RepositoryType == typeof(T));
        }
    }
}