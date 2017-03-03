using PalmenIt.CoreTypes;
using System;

namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public class Minute : IntRange
    {
        private Minute(IntRange value) : base(value) { }

        private static Func<int, Maybe<IntRange>> CreateValue = Create(0, 59);

        public static Maybe<Minute> Create(int value)
        {
            var val = CreateValue(value);
            if (val.IsError) return (Error)val;
            return new Minute(val);
        }

        public Maybe<Minute> Previous()
        {
            return Create(this - 1);
        }

        public Maybe<Minute> Next()
        {
            return Create(this + 1);
        }
    }
}
