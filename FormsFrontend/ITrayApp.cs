using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PalmenIt.dntt.FormsFrontend
{
    internal interface ITrayApp : IDisposable
    {
        void Show();
    }
}
