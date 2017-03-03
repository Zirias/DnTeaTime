using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    public partial class AboutForm : Form
    {
        public AboutForm(Icon icon)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            InitializeComponent();
            IconLabel.Image = icon.ToBigBitmap();
            TitleLabel.Text = "DnTeaTime v" + version.ToString(2);
            BuildInfoLabel.Text = "Build number " + version.ToString();
            FormClosing += (s, e) => { e.Cancel = true; Hide(); };
            OkButton.Click += (s, e) => Hide();
        }

        public new Icon Icon
        {
            get { return base.Icon; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke((Action)(() => base.Icon = value));
                }
                else base.Icon = value;
            }
        }
    }
}
