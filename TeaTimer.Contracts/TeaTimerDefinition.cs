using PalmenIt.CoreTypes;

namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public class TeaTimerDefinition
    {
        public string Name { get; }
        public MinuteAndSecond Time { get; }
        private TeaTimerDefinition(string name, MinuteAndSecond time)
        {
            Name = name;
            Time = time;
        }

        public static Maybe<TeaTimerDefinition> Create(string name, MinuteAndSecond time)
        {
            if (string.IsNullOrEmpty(name)) return new ValidationError<string>(
                    "Name", name, "Name must not be empty");
            if (time == null) return new ValidationError<MinuteAndSecond>(
                    "Time", time, "Time must not be null");

            return new TeaTimerDefinition(name, time);
        }
        public static Maybe<TeaTimerDefinition> Create(string name, Minute minutes, Second seconds)
        {
            var time = MinuteAndSecond.Create(minutes, seconds);
            if (time.IsError) return (Error)time;

            return Create(name, time);
        }

        public static Maybe<TeaTimerDefinition> Create(string name, int minutes, int seconds)
        {
            var time = MinuteAndSecond.Create(minutes, seconds);
            if (time.IsError) return (Error)time;

            return Create(name, time);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Time);
        }
    }
}
