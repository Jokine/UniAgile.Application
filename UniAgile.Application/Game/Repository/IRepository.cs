using System;
using System.Collections.Generic;

namespace UniAgile.Game
{
    public interface IRepository
    {
        Type RepositoryType { get; }

        void Clear();

        void PopDataChangesNonAlloc(IDictionary<string, Notifiable> notifiables,
                                    List<INotifiableDataChange> list);
    }
}