using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public struct CurrencyModel
    {
        public string Type;
        public int    Amount;
    }

    public static class CurrencyExtensions
    {
        // public interface 
        public enum CurrencyType
        {
            Soft,
            Hard
        }

        public static IEnumerable<KeyValuePair<string, CurrencyModel>> GetCurrenciesOfType(this IDictionary<string, CurrencyModel> repository,
                                                                                           CurrencyType                            currencyType)
        {
            return repository.Where(kvp => kvp.Value.Type == currencyType.ToString());
        }
    }


    public class ApplicationModel
    {
        protected IReadOnlyList<IRepository> Repositories { get; set; } = new IRepository[0];

        public void NotifyRepositoryChanges()
        {
            for (var i = 0; i < Repositories.Count; ++i) Repositories[i].NotifyChanges();
        }

        public void Clear()
        {
            if (Repositories == null) return;

            foreach (var rep in Repositories) rep.Clear();
            NotifyRepositoryChanges();
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