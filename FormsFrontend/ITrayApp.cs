using System;

namespace PalmenIt.dntt.FormsFrontend
{
    internal interface ITrayApp : IDisposable
    {
        event EventHandler Exit;
        void Show();
    }
}
