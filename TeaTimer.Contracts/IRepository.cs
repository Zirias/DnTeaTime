using System.Collections.Generic;

namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public interface IRepository<TValue> : IEnumerable<IEntry<TValue>>
    {
        IEntry<TValue> Add(TValue value);
        void Update(IEntry<TValue> value);
        void Remove(IEntry<TValue> value);
        int Count { get; }
    }
}
