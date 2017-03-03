using System;

namespace PalmenIt.CoreTypes
{
    public class IntRange
    {
        public int Value { get; }

        protected IntRange(int value)
        {
            Value = value;
        }

        protected static Func<int, Maybe<IntRange>> Create(int min, int max)
        {
            return (value) =>
            {
                if (value < min) return new ValidationError<int>("value", value,
                    string.Format("The value must not be smaller than {0}.", min));
                if (value > max) return new ValidationError<int>("value", value,
                    string.Format("The value must not be greater than {0}.", max));
                return new IntRange(value);
            };
        }

        public virtual string ToString(string format, IFormatProvider provider)
        {
            return Value.ToString(format, provider);
        }

        public virtual string ToString(IFormatProvider provider)
        {
            return Value.ToString(provider);
        }

        public virtual string ToString(string format)
        {
            return Value.ToString(format);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator int(IntRange x)
        {
            return x.Value;
        }
    }
}
