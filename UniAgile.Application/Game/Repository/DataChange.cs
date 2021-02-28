namespace UniAgile.Game
{
    public struct DataChange<T>
        where T : struct
    {
        public ChangeType ChangeType;
        public string     Id;
        public T          New;
        public T          Old;
    }

    public interface IDataChange<T>
    {
        ChangeType ChangeType { get; }
        string     Id         { get; }
        T          New        { get; }
        T          Old        { get; }
    }

    public enum ChangeType
    {
        Change,
        Add,
        Remove
    }
}