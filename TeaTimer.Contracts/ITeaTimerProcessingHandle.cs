namespace PalmenIt.dntt.TeaTimer.Contracts
{
    public interface ITeaTimerProcessingHandle
    {
        MinuteAndSecond GetRemainingTime();
        void Cancel();
        TeaTimerDefinition TeaTimer { get; }
    }
}
