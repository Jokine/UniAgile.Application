using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UniAgile.Game;

namespace UniAgile.Application
{
    
    public class CurrencyService
    {
        public CurrencyService((IDictionary<string, CurrencyModel> Models, INotifyCollectionChanged Listenable) currency)
        {
            if (currency.Listenable == default) throw new NullReferenceException();
            if (currency.Models     == default) throw new NullReferenceException();

            Currency = currency;
        }


        public readonly (IDictionary<string, CurrencyModel> Models, INotifyCollectionChanged Listenable) Currency;

    }

    public class CurrencyHeader
    {
        public CurrencyHeader(CurrencyService currencyService)
        {
            CurrencyService = currencyService;

            CurrencyService.Currency.Listenable.CollectionChanged += OnChange;
        }

        private void OnChange(object                           change,
                              NotifyCollectionChangedEventArgs _)
        {
            var dataChange = (DataChange<CurrencyModel>) change;
            
        }

        private readonly CurrencyService CurrencyService;
    }

}