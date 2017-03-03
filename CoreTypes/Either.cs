using System;

namespace PalmenIt.CoreTypes
{
    public class Either<TLeft, TRight>
    {
        private readonly bool _isLeft;
        private readonly TLeft _left;
        private readonly TRight _right;

        public Either(TLeft left)
        {
            _isLeft = true;
            _left = left;
        }

        public Either(TRight right)
        {
            _isLeft = false;
            _right = right;
        }

        public bool IsLeft { get { return _isLeft; } }

        public static implicit operator Either<TLeft, TRight>(TLeft x)
        {
            return new Either<TLeft, TRight>(x);
        }

        public static implicit operator Either<TLeft, TRight>(TRight x)
        {
            return new Either<TLeft, TRight>(x);
        }

        public static implicit operator TLeft(Either<TLeft, TRight> x)
        {
            if (!x.IsLeft) throw new InvalidCastException(
                string.Format("This instance of {0} doesn't contain a {1}.",
                    x.GetType().FullName, typeof(TLeft).FullName));
            return x._left;
        }

        public static implicit operator TRight(Either<TLeft, TRight> x)
        {
            if (x.IsLeft) throw new InvalidCastException(
                string.Format("This instance of {0} doesn't contain a {1}.",
                    x.GetType().FullName, typeof(TRight).FullName));
            return x._right;
        }
    }
}
