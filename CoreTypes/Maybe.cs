namespace PalmenIt.CoreTypes
{
    public class Maybe<TValue> : Either<TValue, Error>
    {
        public Maybe(TValue value) : base(value) { }

        public Maybe(Error error) : base(error) { }

        public bool IsError { get { return !IsLeft; } }

        public static implicit operator Maybe<TValue>(TValue x)
        {
            return new Maybe<TValue>(x);
        }

        public static implicit operator Maybe<TValue>(Error x)
        {
            return new Maybe<TValue>(x);
        }
    }
}
