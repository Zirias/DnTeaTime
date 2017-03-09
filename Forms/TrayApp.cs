using System.Windows.Forms;

namespace PalmenIt.Forms
{
    public static class TrayApp
    {
        public static void Run(ITrayApp app)
        {
            Application.Run(new TrayAppContext(app));
        }
    }
}
