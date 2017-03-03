using PalmenIt.dntt.TeaTimer.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class Setup
    {
        internal class TeaTimerEventArgs: EventArgs
        {
            IEntry<TeaTimerDefinition> Entry { get; }

            public TeaTimerEventArgs(IEntry<TeaTimerDefinition> entry)
            {
                Entry = entry;
            }
        }

        internal Dictionary<IEntry<TeaTimerDefinition>, ITeaTimerProcessingHandle> Handles { get; }
        internal ITeaTimerRepository Repository { get; }

        internal Setup(ITeaTimerRepository repository)
        {
            Repository = repository;
            Handles = new Dictionary<IEntry<TeaTimerDefinition>, ITeaTimerProcessingHandle>();
        }

        internal event EventHandler<TeaTimerEventArgs> TimerStarted;

        internal event EventHandler<TeaTimerEventArgs> TimerStopped;

        internal void EmitTimerStarted(IEntry<TeaTimerDefinition> entry)
        {
            TimerStarted?.Invoke(this, new TeaTimerEventArgs(entry));
        }

        internal void EmitTimerStopped(IEntry<TeaTimerDefinition> entry)
        {
            TimerStopped?.Invoke(this, new TeaTimerEventArgs(entry));
        }

    }
}
