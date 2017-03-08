using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class TrayAppContext : ApplicationContext
    {
        private bool _disposed = false;

        public ITrayApp App { get; }

        public TrayAppContext(ITrayApp app)
        {
            Application.ApplicationExit += (s, e) => Dispose();
            app.Exit += (s, e) => Dispose();
            App = app;
            App.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                App.Dispose();
                _disposed = true;
            }
            base.Dispose(disposing);
            ExitThread();
        }
    }
}
