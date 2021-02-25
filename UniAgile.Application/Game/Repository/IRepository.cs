using System;

namespace UniAgile.Game
{
    public interface IRepository
    {
        Type RepositoryType { get; }

        void NotifyChanges();

        void Clear();
    }
}