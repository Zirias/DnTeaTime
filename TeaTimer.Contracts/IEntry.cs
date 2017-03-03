namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public interface IEntry<TValue>
    {
        TValue Value { get; set; }
    }
}
