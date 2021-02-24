using System;
using System.Collections.Generic;
using System.Linq;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public class ApplicationModel : IDataRecorder
    {
        private readonly IDictionary<string, bool>              NotifiedThisFrame = new Dictionary<string, bool>();
        private readonly IDictionary<string, IListenableSignal> Signals           = new Dictionary<string, IListenableSignal>();
        protected        ulong                                  CurrentDataCommitId { get; private set; }
        protected        IReadOnlyList<IRepository>             Repositories        { get; set; } = new IRepository[0];


        public void CommitChanges<T>(ulong                        commitId,
                                     IReadOnlyList<DataChange<T>> dataChanges)
            where T : struct
        {
            // todo: save the data changes for rollbacking
            // remember memory limits, serialization and delta rollbacking

            // all changes are in order, last change will always mean that is the current state
            for (var i = dataChanges.Count - 1; i >= 0; i--)
            {
                var change = dataChanges[i];

                if (NotifiedThisFrame.ContainsKey(change.Id)) continue;

                var signal = GetCastSignal<DataChange<T>>(Signals, change.Id);

                signal.Invoke(change);
                NotifiedThisFrame.Add(change.Id, true);
            }
        }

        public IListenerHandle BindToDataChange<T>(string    id,
                                                   Action<DataChange<T>> listener)
            where T : struct
        {
            id = FormId(typeof(T), id);

            var signal = GetCastSignal<DataChange<T>>(Signals, id);

            return new ListenerHandle<DataChange<T>>(signal, listener);
        }

        public IListenerHandle BindToDataChange<T>(string                          id,
                                                   Action<KeyValuePair<string, DataChange<T>>> listener)
            where T : struct
        {
            id = FormId(typeof(T), id);

            var signal = GetCastSignal<DataChange<T>>(Signals, id);

            return new KvpListenerHandle<DataChange<T>>(signal, id, listener);
        }

        public void CommitCurrentChanges()
        {
            for (var i = 0; i < Repositories.Count; ++i) Repositories[i].ApplyChanges(CurrentDataCommitId, this);
            CurrentDataCommitId++;
            NotifiedThisFrame.Clear();
        }

        private static Signal<T> GetCastSignal<T>(IDictionary<string, IListenableSignal> signals,
                                                  string                                 id)
        {
            Signal<T> castSignal;

            if (!signals.TryGetValue(id, out var uncastSignal))
            {
                castSignal = new Signal<T>();
                signals.Add(id, castSignal);
            }
            else
            {
                castSignal = (Signal<T>) uncastSignal;
            }

            return castSignal;
        }

        public void Clear()
        {
            if (Repositories == null) return;

            foreach (var rep in Repositories) rep.Clear();
            CommitCurrentChanges();
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