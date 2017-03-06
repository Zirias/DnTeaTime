using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class Notification
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Icon _notificationIcon;
        private readonly Action<string, string> _notificationAction;

        private const string APPUSERMODELID = "PalmenIt.DnTeaTime";
        private const string TOASTFORMAT = "<toast><visual><binding template=\"ToastImageAndText02\">"
            + "<image id=\"1\" src=\"{0}\" alt=\"DnTeaTime\"/>"
            + "<text id=\"1\">{1}</text><text id=\"2\">{2}</text>"
            + "<audio src=\"ms-winsoundevent:Notification.Reminder\" loop=\"false\"/>"
            + "</binding></visual></toast>";

        public Notification(NotifyIcon notifyIcon, Icon notificationIcon)
        {
            _notifyIcon = notifyIcon;
            _notificationIcon = notificationIcon;

            if (Environment.OSVersion.Version.Major < 6 || Environment.OSVersion.Version.Minor < 2)
            {
                // Balloon notification for Windows versions prior to 8.0 (NT 6.2)
                _notificationAction = ShowBalloonNotification;
            }
            else
            {
                _notificationAction = ShowToastNotification;
            }
        }

        private void ShowToastNotification(string title, string text)
        {
            var imageFileName = Path.GetTempFileName();
            try
            {
                _notificationIcon.SaveImageToFile(imageFileName, 48, 48, 32);
                var toastXml = new XmlDocument();
                toastXml.LoadXml(string.Format(TOASTFORMAT, "file:///" + imageFileName, title, text));
                var toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier(APPUSERMODELID).Show(toast);
            }
            finally
            {
                try
                {
                    File.Delete(imageFileName);
                }
                catch
                {
                    // Can't delete temp file and can't do anything about it. Well.
                }
            }
        }

        private void ShowBalloonNotification(string title, string text)
        {
            _notifyIcon.ShowBalloonTip(8000, title, text, ToolTipIcon.Info);
            SystemSounds.Beep.Play();
        }

        public void ShowNotification(string title, string text)
        {
            _notificationAction(title, text);
        }
    }
}
