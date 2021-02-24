using System;
using System.Collections.Generic;
using UniAgile.Game;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public interface IDataRecorder
    {
        void CommitRecord<T>(ulong                        commitId,
                             IReadOnlyList<DataChange<T>> dataChanges)
            where T : struct;

        IListenerHandle BindToDataChange<T>(string    id,
                                            Action<T> listener)
            where T : struct;

        IListenerHandle BindToDataChange<T>(string                          id,
                                            Action<KeyValuePair<string, T>> listener)
            where T : struct;
    }
}