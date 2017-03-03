using System;
using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class TrayAppContext : ApplicationContext
    {
        private bool _disposed = false;

        public ITrayApp App { get; }

        public TrayAppContext(ITrayApp app)
        {
            App = app;
            App.Exit += App_Exit;
            Application.ApplicationExit += App_Exit;
            App.Show();
        }

        private void App_Exit(object sender, EventArgs e)
        {
            App.Dispose();
            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                App.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
