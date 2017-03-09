using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class Notification : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Icon _notificationIcon;
        private readonly Action<string, string> _notificationAction;
        private readonly Forms.ToastNotification _formsToast;

        private const string APPUSERMODELID = "PalmenIt.DnTeaTime";
        private const string TOASTFORMAT = "<toast><visual><binding template=\"ToastImageAndText02\">"
            + "<image id=\"1\" src=\"file:///{0}\" alt=\"DnTeaTime\"/>"
            + "<text id=\"1\">{1}</text><text id=\"2\">{2}</text>"
            + "<audio src=\"ms-winsoundevent:Notification.Reminder\" loop=\"false\"/>"
            + "</binding></visual></toast>";

        private bool _disposed = false;

        public Notification(NotifyIcon notifyIcon, Icon notificationIcon)
        {
            _notifyIcon = notifyIcon;
            _notificationIcon = notificationIcon;
            if (VersionCheck.HasWinRT)
            {
                _notificationAction = ShowToastNotification;
            }
            else
            {
                _formsToast = new Forms.ToastNotification(notificationIcon);
                _notificationAction = ShowFormsToastNotification;
            }
        }

        private void ShowToastNotification(string title, string text)
        {
            var imageFileName = CreateTempPng();
            try
            {
                var image = _notificationIcon.ToBitmap(maxWidth: 32, minWidth: 32);
                image.Save(imageFileName, ImageFormat.Png);
                var toastXml = new XmlDocument();
                toastXml.LoadXml(string.Format(TOASTFORMAT, imageFileName, title, text));
                var toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier(APPUSERMODELID).Show(toast);
            }
            finally
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        Thread.Sleep(1000);
                        File.Delete(imageFileName);
                    }
                    catch
                    {
                        // Can't delete temp file and can't do anything about it. Well.
                    }
                });
            }
        }

        private static string CreateTempPng()
        {
            int attempt = 0;
            var path = Path.GetTempPath();
            while (true)
            {
                var filename = path + Guid.NewGuid().ToString() + ".png";
                try
                {
                    using (new FileStream(filename, FileMode.CreateNew)) { }
                    return filename;
                }
                catch (IOException ex)
                {
                    if (++attempt == 10)
                        throw new IOException("No unique temporary file name is available.", ex);
                }
            }
        }

        private void ShowFormsToastNotification(string title, string text)
        {
            _formsToast.Show(title, text, 15000);
            SystemSounds.Beep.Play();
        }

        public void ShowNotification(string title, string text)
        {
            _notificationAction(title, text);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_formsToast != null) _formsToast.Dispose();
            }
        }
    }
}
