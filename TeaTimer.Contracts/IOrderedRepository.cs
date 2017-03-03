namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public interface IOrderedRepository<TValue> : IRepository<TValue>
    {
        int IndexOf(IEntry<TValue> value);
        void MoveTo(IEntry<TValue> value, int position);
    }
}
