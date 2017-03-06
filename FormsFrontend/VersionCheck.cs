using System;

namespace PalmenIt.dntt.FormsFrontend
{
    internal static class VersionCheck
    {
        internal static bool HasWinRT
        {
            get
            {
                var major = Environment.OSVersion.Version.Major;
                if (major > 6) return true;
                if (major == 6) return Environment.OSVersion.Version.Minor >= 2;
                return false;
            }
        }
    }
}
