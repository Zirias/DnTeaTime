using PalmenIt.dntt.TeaTimer.Contracts;

namespace PalmenIt.dntt.TeaTimer.RegistryRepository
{
    internal class Entry<TValue> : IEntry<TValue>
    {
        public TValue Value { get; set; }

        internal Entry(TValue value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
