using System;

namespace UniAgile.Game
{
    public interface IRepository
    {
        Type RepositoryType { get; }

        void ApplyChanges(ulong         commitId,
                          IDataRecorder recorder);

        void Clear();
    }
}