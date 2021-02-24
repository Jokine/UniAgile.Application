namespace UniAgile.Game
{
    public struct DataChange<T>
        where T : struct
    {
        public T          New;
        public T          Old;
        public string     Id;
        public ChangeType ChangeType;
    }

    public enum ChangeType
    {
        Change,
        Add,
        Remove
    }
}