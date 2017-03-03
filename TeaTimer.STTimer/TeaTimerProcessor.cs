using PalmenIt.dntt.TeaTimer.Contracts;
using System;
using System.Threading;

namespace PalmenIt.dntt.TeaTimer.STTimer
{
    public class TeaTimerProcessor : ITeaTimerProcessor
    {
        private class Handle : ITeaTimerProcessingHandle
        {
            private readonly Timer _timer;
            private MinuteAndSecond _remainingTime;
            private readonly Action<ITeaTimerProcessingHandle> _cancelHandler;

            public TeaTimerDefinition TeaTimer { get; }

            internal Handle(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed, Action<ITeaTimerProcessingHandle> tick, Action<ITeaTimerProcessingHandle> canceled)
            {
                TeaTimer = teaTimer;
                _remainingTime = teaTimer.Time;
                _cancelHandler = canceled;

                _timer = new Timer(arg =>
                {
                    lock (this)
                    {
                        var remaining = _remainingTime.Previous();
                        if (remaining.IsError)
                        {
                            _timer.Dispose();
                            if (completed != null) completed(this);
                        }
                        else
                        {
                            _remainingTime = remaining;
                            if (tick != null) tick(this);
                        }
                    }
                }, null, 1000, 1000);
            }

            public void Cancel()
            {
                lock(this)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer.Dispose();
                    if (_cancelHandler != null) _cancelHandler(this);
                }
            }

            public MinuteAndSecond GetRemainingTime()
            {
                return _remainingTime;
            }
        }

        public ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer)
        {
            return Process(teaTimer, null);
        }

        public ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed)
        {
            return Process(teaTimer, completed, null);
        }

        public ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed, Action<ITeaTimerProcessingHandle> tick)
        {
            return Process(teaTimer, completed, tick, null);
        }

        public ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed, Action<ITeaTimerProcessingHandle> tick, Action<ITeaTimerProcessingHandle> canceled)
        {
            return new Handle(teaTimer, completed, tick, canceled);
        }
    }
}
