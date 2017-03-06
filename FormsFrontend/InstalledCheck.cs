using Microsoft.Win32;

namespace PalmenIt.dntt.FormsFrontend
{
    internal static class InstalledCheck
    {
        public static bool IsInstalled
        {
            get
            {
                try
                {
                    return (int)(Registry.CurrentUser
                        .OpenSubKey("Software")
                        .OpenSubKey("PalmenIt")
                        .OpenSubKey("DnTeaTime")
                        .GetValue("Installed")) == 1;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
