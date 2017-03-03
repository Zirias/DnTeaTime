using System;

namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public interface ITeaTimerProcessor
    {
        ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer);
        ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed);
        ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed,
            Action<ITeaTimerProcessingHandle> tick);
        ITeaTimerProcessingHandle Process(TeaTimerDefinition teaTimer, Action<ITeaTimerProcessingHandle> completed,
            Action<ITeaTimerProcessingHandle> tick, Action<ITeaTimerProcessingHandle> canceled);
    }
}
