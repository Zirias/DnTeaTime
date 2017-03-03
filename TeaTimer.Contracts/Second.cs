using PalmenIt.CoreTypes;
using System;

namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public class Second : IntRange
    {
        private Second(IntRange value) : base(value) { }

        private static Func<int, Maybe<IntRange>> CreateValue = Create(0, 59);

        public static Maybe<Second> Create(int value)
        {
            var val = CreateValue(value);
            if (val.IsError) return (Error)val;
            return new Second(val);
        }

        public Maybe<Second> Previous()
        {
            return Create(this - 1);
        }

        public Maybe<Second> Next()
        {
            return Create(this + 1);
        }
    }
}
