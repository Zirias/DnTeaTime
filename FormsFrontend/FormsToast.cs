using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    internal class FormsToast : IDisposable
    {
        private class ToastControl : Control
        {
            private bool _isMouseOverToast = false;
            private bool _isMouseOverCloseButton = false;
            private Rectangle _closeButtonRectangle = new Rectangle(0, 12, 13, 13);
            private string _title;
            private string _text;

            private readonly Color _toastBackgroundColor;
            private readonly Color _toastBorderColor;
            private readonly Color _toastTitleColor;
            private readonly Color _toastTextColor;
            private readonly Color _toastCloseColorDark;
            private readonly Color _toastCloseColorBright;
            private readonly Color _toastCloseHoverColorDark;
            private readonly Color _toastCloseHoverColorBright;

            private readonly Image _toastImage;

            public ToastControl(Image image)
            {
                SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

                _toastImage = image;

                _closeButtonRectangle.X = ClientRectangle.Width - 26;

                _toastBackgroundColor = Color.FromArgb(unchecked((int)0xff1f1f1f));
                _toastBorderColor = Color.FromArgb(unchecked((int)0xff484848));
                _toastTitleColor = Color.White;
                _toastTextColor = Color.FromArgb(unchecked((int)0xffa5a5a5));
                _toastCloseColorDark = Color.FromArgb(unchecked((int)0xff6b6b6b));
                _toastCloseColorBright = Color.FromArgb(unchecked((int)0xffeeeeee));
                _toastCloseHoverColorDark = Color.FromArgb(unchecked((int)0xff3e3e3e));
                _toastCloseHoverColorBright = Color.FromArgb(unchecked((int)0xff727272));
            }

            public string Title
            {
                get { return _title; }
                set
                {
                    _title = value;
                    Invalidate();
                }
            }

            public override string Text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    Invalidate();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                var backgroundBrush = new SolidBrush(_toastBackgroundColor);
                e.Graphics.FillRectangle(backgroundBrush, ClientRectangle);
                var borderPen = new Pen(_toastBorderColor);
                var borderRectangle = ClientRectangle;
                borderRectangle.Width -= 1;
                borderRectangle.Height -= 1;
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
                var titleSize = TextRenderer.MeasureText(Title, SystemFonts.CaptionFont);
                var titlePoint = new Point(12, 12);
                var textPoint = new Point(12, 14 + titleSize.Height);
                if (_toastImage != null)
                {
                    titlePoint.X += 12 + _toastImage.Width;
                    textPoint.X += 12 + _toastImage.Width;
                    var imageRectangle = new Rectangle(new Point(12, 12), _toastImage.Size);
                    e.Graphics.DrawImage(_toastImage, imageRectangle);
                }
                TextRenderer.DrawText(e.Graphics, Title, SystemFonts.CaptionFont, titlePoint, _toastTitleColor);
                TextRenderer.DrawText(e.Graphics, Text, SystemFonts.DefaultFont, textPoint, _toastTextColor);
                if (_isMouseOverToast)
                {
                    Color dark;
                    Color bright;
                    if (_isMouseOverCloseButton)
                    {
                        dark = _toastCloseHoverColorDark;
                        bright = _toastCloseHoverColorBright;
                    }
                    else
                    {
                        dark = _toastCloseColorDark;
                        bright = _toastCloseColorBright;
                    }
                    var backPen = new Pen(dark, 2.6f);
                    backPen.StartCap = LineCap.Triangle;
                    backPen.EndCap = LineCap.Triangle;
                    var frontPen = new Pen(bright);
                    var crossRectangle = _closeButtonRectangle;
                    crossRectangle.Inflate(-2, -2);
                    e.Graphics.DrawLine(backPen, crossRectangle.Left, crossRectangle.Top, crossRectangle.Right, crossRectangle.Bottom);
                    e.Graphics.DrawLine(backPen, crossRectangle.Left, crossRectangle.Bottom, crossRectangle.Right, crossRectangle.Top);
                    crossRectangle.Inflate(-1, -1);
                    e.Graphics.DrawLine(frontPen, crossRectangle.Left, crossRectangle.Top, crossRectangle.Right, crossRectangle.Bottom);
                    e.Graphics.DrawLine(frontPen, crossRectangle.Left, crossRectangle.Bottom, crossRectangle.Right, crossRectangle.Top);
                }
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                _isMouseOverToast = true;
                _isMouseOverCloseButton = _closeButtonRectangle.Contains(PointToClient(MousePosition));
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                _isMouseOverToast = false;
                _isMouseOverCloseButton = false;
                Invalidate();
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                var wasMoiseOverCloseButton = _isMouseOverCloseButton;
                _isMouseOverCloseButton = _closeButtonRectangle.Contains(e.Location);
                if (_isMouseOverCloseButton != wasMoiseOverCloseButton) Invalidate();
            }

            protected override void OnMouseClick(MouseEventArgs e)
            {
                if (_closeButtonRectangle.Contains(e.Location)) Parent.Hide();
            }

            protected override void OnSizeChanged(EventArgs e)
            {
                _closeButtonRectangle.X = ClientRectangle.Width - 26;
                base.OnSizeChanged(e);
            }
        }

        private static class WinAPI
        {
            private struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr FindWindow(string strClassName, string strWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


            private static IntPtr GetTrayHandle()
            {
                IntPtr taskBarHandle = FindWindow("Shell_TrayWnd", null);
                if (!taskBarHandle.Equals(IntPtr.Zero))
                {
                    return FindWindowEx(taskBarHandle, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero);
                }
                return IntPtr.Zero;
            }

            public static Rectangle GetTrayRectangle()
            {
                RECT rect;
                GetWindowRect(GetTrayHandle(), out rect);
                return new Rectangle(new Point(rect.left, rect.top), new Size((rect.right - rect.left) + 1, (rect.bottom - rect.top) + 1));
            }
        }

        private readonly Image _toastImage;
        private readonly Form _toastForm;
        private readonly ToastControl _toastControl;
        private readonly Timer _toastTimer;
        private readonly System.Threading.SynchronizationContext _syncContext;

        private bool _disposed = false;

        internal FormsToast(Icon toastIcon)
        {
            _syncContext = System.Threading.SynchronizationContext.Current;
            if (toastIcon == null)
            {
                _toastImage = null;
            }
            else
            {
                _toastImage = toastIcon.ToBitmap(32, 32);
            }
            _toastControl = new ToastControl(_toastImage)
            {
                Dock = DockStyle.Fill
            };
            _toastForm = new Form()
            {
                Name = "ToastNotification",
                AutoSize = false,
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.None,
                ShowInTaskbar = false,
                Padding = new Padding(0),
                TopMost = true,
                Size = new Size(160, 48)
            };
            _toastForm.Controls.Add(_toastControl);

            _toastTimer = new Timer();
            _toastTimer.Tick += ToastTimer_Tick;
        }

        private void ToastTimer_Tick(object sender, EventArgs e)
        {
            _toastForm.Hide();
            _toastTimer.Stop();
        }

        public void Show(string title, string text, int duration)
        {
            _syncContext.Post(delegate
            {
                ShowCore(title, text, duration);
            }, null);
        }

        private void ShowCore(string title, string text, int duration)
        {
            _toastTimer.Stop();
            _toastForm.Hide();

            var trayRect = WinAPI.GetTrayRectangle();
            var screenRect = Screen.GetBounds(trayRect);
            var clientRect = Screen.GetWorkingArea(screenRect);

            var titleSize = TextRenderer.MeasureText(title, SystemFonts.CaptionFont);
            var textSize = TextRenderer.MeasureText(text, SystemFonts.DefaultFont);
            var textPartHeight = titleSize.Height + textSize.Height + 26;
            var textPartWidth = (titleSize.Width > textSize.Width ? titleSize.Width : textSize.Width) + 48;
            var imageWidth = _toastImage == null ? 0 : _toastImage.Size.Width + 12;
            var imageHeight = _toastImage == null ? 0 : _toastImage.Size.Height + 24;

            var dTop = trayRect.Top - screenRect.Top;
            var dLeft = trayRect.Left - screenRect.Left;
            var dRight = screenRect.Right - trayRect.Right;
            var dBottom = screenRect.Bottom - trayRect.Bottom;

            _toastForm.Width = imageWidth + textPartWidth;
            _toastForm.Height = imageHeight > textPartHeight ? imageHeight : textPartHeight;

            if (dLeft > dRight)
            {
                if (dTop > dBottom)
                {
                    _toastForm.Location = new Point(
                        clientRect.Right - _toastForm.Width,
                        clientRect.Bottom - _toastForm.Height - 12);
                }
                else
                {
                    _toastForm.Location = new Point(
                        clientRect.Right - _toastForm.Width,
                        clientRect.Top + 12);
                }
            }
            else
            {
                _toastForm.Location = new Point(
                    clientRect.Left,
                    clientRect.Bottom - _toastForm.Height - 12);
            }

            _toastControl.Title = title;
            _toastControl.Text = text;

            _toastTimer.Interval = duration;
            _toastTimer.Start();

            _toastForm.Show();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _toastTimer.Dispose();
                _toastControl.Dispose();
                _toastForm.Dispose();
                _disposed = true;
            }
        }
    }
}
