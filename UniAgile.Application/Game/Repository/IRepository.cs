using System;

namespace UniAgile.Game
{
    public interface IRepository
    {
        Type RepositoryType { get; }

        void CommitData(ulong         commitId,
                        IDataRecorder recorder);

        void Clear();
    }
}