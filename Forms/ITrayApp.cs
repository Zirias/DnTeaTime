using System;

namespace PalmenIt.Forms
{
    public interface ITrayApp : IDisposable
    {
        event EventHandler Exit;
        void Show();
    }
}
