using PalmenIt.CoreTypes;

namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public class MinuteAndSecond
    {
        public Minute Minute { get; }
        public Second Second { get; }

        private MinuteAndSecond(Minute minute, Second second)
        {
            Minute = minute;
            Second = second;
        }

        public static Maybe<MinuteAndSecond> Create(Minute minute, Second second)
        {
            if (minute == null) return new ValidationError<Minute>("minute", minute, "Minute must not be null.");
            if (second == null) return new ValidationError<Second>("second", second, "Second must not be null.");
            return new MinuteAndSecond(minute, second);
        }

        public static Maybe<MinuteAndSecond> Create(int minute, int second)
        {
            var sec = Second.Create(second);
            if (sec.IsError) return (Error)sec;
            var min = Minute.Create(minute);
            if (min.IsError) return (Error)min;

            return Create(min, sec);
        }

        public Maybe<MinuteAndSecond> Previous()
        {
            var sec = Second.Previous();
            if (sec.IsError)
            {
                sec = Second.Create(59);
                var min = Minute.Previous();
                if (min.IsError) return (Error)min;
                return Create(min, sec);
            }
            return Create(Minute, sec);
        }

        public Maybe<MinuteAndSecond> Next()
        {
            var sec = Second.Next();
            if (sec.IsError)
            {
                sec = Second.Create(0);
                var min = Minute.Next();
                if (min.IsError) return (Error)min;
                return Create(min, sec);
            }
            return Create(Minute, sec);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Minute, Second.ToString("D2"));
        }
    }
}
