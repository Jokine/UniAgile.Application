using System;
using System.Collections.Generic;
using UniAgile.Observable;

namespace UniAgile.Game
{
    public interface IDataRecorder
    {
        void CommitChanges<T>(ulong                        commitId,
                              IReadOnlyList<DataChange<T>> dataChanges)
            where T : struct;

        void CommitCurrentChanges();


        IListenerHandle BindToDataChange<T>(string                id,
                                            Action<DataChange<T>> listener)
            where T : struct;

        IListenerHandle BindToDataChange<T>(string                          id,
                                            Action<KeyValuePair<string, DataChange<T>>> listener)
            where T : struct;
    }
}