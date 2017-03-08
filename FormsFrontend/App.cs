using PalmenIt.dntt.TeaTimer.Contracts;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class App : ITrayApp
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly Notification _notification;
        private readonly SetupForm _setupForm;
        private readonly AboutForm _aboutForm;
        private readonly Setup _setup;
        private readonly ITeaTimerProcessor _timerProcessor;
        private readonly Icon _idleIcon;
        private readonly Icon _activeIcon;

        private bool _disposed = false;

        public event EventHandler Exit;

        public App(ITeaTimerRepository timerRepository, ITeaTimerProcessor timerProcessor)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            _setup = new Setup(timerRepository);
            _timerProcessor = timerProcessor;

            _idleIcon = new Icon(GetType(), "idle.ico");
            _activeIcon = new Icon(GetType(), "active.ico");

            _setupForm = new SetupForm(_setup);
            _aboutForm = new AboutForm(_activeIcon);
            _notifyIcon = new NotifyIcon();

            var cm = new ContextMenu();
            cm.Popup += (s, e) =>
            {
                cm.MenuItems.Clear();
                cm.MenuItems.Add("About", (s1, e1) => _aboutForm.Show());
                cm.MenuItems.Add("-");
                cm.MenuItems.AddRange(GetTeaMenuItemsWithHandlers());
                cm.MenuItems.Add("-");
                cm.MenuItems.Add("Setup", (s1, e1) => _setupForm.Show());
                cm.MenuItems.Add("Exit", (s1, e1) => OnExit());
            };

            _notifyIcon.ContextMenu = cm;
            _notifyIcon.Text = "DnTeaTime v" + version.ToString(2);

            _notifyIcon.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                    mi.Invoke(_notifyIcon, null);
                }
            };

            _notification = new Notification(_notifyIcon, _activeIcon);

            SetIcon(_idleIcon);
        }

        private void OnExit()
        {
            Exit?.Invoke(this, new EventArgs());
        }

        private void SetIcon(Icon icon)
        {
            _notifyIcon.Icon = icon;
            _setupForm.Icon = icon;
            _aboutForm.Icon = icon;
        }

        private MenuItem[] GetTeaMenuItemsWithHandlers()
        {
            return _setup.Repository
                .Select(e =>
                {
                    if (_setup.Handles.ContainsKey(e))
                    {
                        return new MenuItem(
                            string.Format("{0} ({1} running ...)", e.Value.Name, _setup.Handles[e].GetRemainingTime()),
                            (s, ev) =>
                            {
                                _setup.Handles[e].Cancel();
                                _setup.Handles.Remove(e);
                                _setup.EmitTimerStopped(e);
                                if (_setup.Handles.Count == 0) SetIcon(_idleIcon);
                            });
                    }
                    else
                    {
                        return new MenuItem(
                            string.Format("{0} ({1})", e.Value.Name, e.Value.Time),
                            (s, ev) =>
                            {
                                _setup.Handles[e] = _timerProcessor.Process(e.Value, h =>
                                {
                                    _setup.Handles.Remove(e);
                                    ShowTeaNotification(e.Value);
                                    _setup.EmitTimerStopped(e);
                                    if (_setup.Handles.Count == 0) SetIcon(_idleIcon);
                                });
                                _setup.EmitTimerStarted(e);
                                SetIcon(_activeIcon);
                            });
                    }
                })
                .ToArray();
        }

        private void ShowTeaNotification(TeaTimerDefinition value)
        {
            _notification.ShowNotification(
                string.Format("{0} is ready.", value.Name),
                string.Format("It brewed for {0} minutes.", value.Time));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _setupForm.Dispose();
                _aboutForm.Dispose();
                _notification.Dispose();
                _notifyIcon.Dispose();
                if (_setup.Repository is IDisposable)
                {
                    ((IDisposable)_setup.Repository).Dispose();
                }
                _disposed = true;
            }
        }

        public void Show()
        {
            _notifyIcon.Visible = true;
        }
    }
}
