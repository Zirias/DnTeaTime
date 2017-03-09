using PalmenIt.dntt.TeaTimer.Contracts;
using PalmenIt.dntt.TeaTimer.RegistryRepository;
using PalmenIt.dntt.TeaTimer.STTimer;
using PalmenIt.Forms;
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

                if (VersionCheck.HasWinRT && !InstalledCheck.IsInstalled)
                {
                    if (MessageBox.Show(null,
                        "You are running DnTeaTime directly without installing it." + Environment.NewLine +
                        "Due to the way notifications work on Windows 8 and later," + Environment.NewLine +
                        "You won't get notified when your tea is ready. Please" + Environment.NewLine +
                        "install DnTeaTime first!" + Environment.NewLine +
                        Environment.NewLine +
                        "Click Cancel to exit now or OK to continue anyways.", "DnTeaTime Warning",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                        == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                var repository = new TeaTimerRepository();
                if (repository.Count == 0)
                {
                    repository.Add(TeaTimerDefinition.Create("Black Tea", 4, 0));
                    repository.Add(TeaTimerDefinition.Create("Earl Grey", 4, 30));
                    repository.Add(TeaTimerDefinition.Create("Herbal Tea", 10, 0));
                    repository.Add(TeaTimerDefinition.Create("Green Tea", 2, 30));
                }

                var processor = new TeaTimerProcessor();

                TrayApp.Run(new App(repository, processor));
            }
        }
    }
}
