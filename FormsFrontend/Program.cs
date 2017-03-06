using PalmenIt.dntt.TeaTimer.Contracts;
using PalmenIt.dntt.TeaTimer.RegistryRepository;
using PalmenIt.dntt.TeaTimer.STTimer;
using System;
using System.Threading;
using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    static class Program
    {
        private static Mutex mutex = new Mutex(true, "{F87FEF1D-AC6C-4488-BDAC-FCC102324539}");

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var repository = new TeaTimerRepository();
                if (repository.Count == 0)
                {
                    repository.Add(TeaTimerDefinition.Create("Black Tea", 4, 0));
                    repository.Add(TeaTimerDefinition.Create("Earl Grey", 4, 30));
                    repository.Add(TeaTimerDefinition.Create("Herbal Tea", 10, 0));
                    repository.Add(TeaTimerDefinition.Create("Green Tea", 2, 30));
                }

                var processor = new TeaTimerProcessor();

                var app = new App(repository, processor);

                Application.Run(new TrayAppContext(app));
            }
        }
    }
}
