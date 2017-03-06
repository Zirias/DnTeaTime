using PalmenIt.CoreTypes;
using PalmenIt.dntt.TeaTimer.Contracts;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PalmenIt.dntt.FormsFrontend
{
    internal partial class SetupForm : Form
    {
        private const string _saveToolTipText = "Unable to modify this tea while a timer for it is running";
        private readonly Setup _setup;
        private readonly ToolTip _saveToolTip;
        private bool _toolTipShown;

        internal SetupForm(Setup setup)
        {
            _setup = setup;
            _saveToolTip = new ToolTip();
            InitializeComponent();
            _setup.TimerStarted += Setup_TimerStarted;
            _setup.TimerStopped += Setup_TimerStopped;
            TeaRepositoryView.Items.Clear();
            TeaRepositoryView.Items.AddRange(_setup.Repository.ToArray());
            TeaRepositoryView.Items.Add("<New>");
            TeaRepositoryView.SelectedIndex = TeaRepositoryView.Items.Count - 1;
            TeaRepositoryView.MouseDown += TeaRepositoryView_MouseDown;
            TeaRepositoryView.DragOver += TeaRepositoryView_DragOver;
            TeaRepositoryView.DragDrop += TeaRepositoryView_DragDrop;
            TeaRepositoryView.ContextMenu = new ContextMenu();
            TeaRepositoryView.ContextMenu.MenuItems.Add("Delete", TeaRepositoryView_DeleteItem);

            _toolTipShown = false;
            ButtonsPanel.MouseMove += ButtonsPanel_MouseMove;
            SaveBtn.Click += SaveBtn_Click;
            CancelBtn.Click += CancelBtn_Click;
        }

        private void UpdateFormFieldsFromRepositoryView(int itemIndex)
        {
            var item = TeaRepositoryView.Items[itemIndex];
            if (item is IEntry<TeaTimerDefinition>)
            {
                var entry = (IEntry<TeaTimerDefinition>)item;
                NameTextBox.Text = entry.Value.Name;
                MinuteUpDown.Value = entry.Value.Time.Minute;
                SecondUpDown.Value = entry.Value.Time.Second;
                TeaEditGroup.Text = entry.Value.Name;
                CancelBtn.Enabled = true;
                SaveBtn.Text = "Save";
                SaveBtn.Enabled = !_setup.Handles.ContainsKey(entry);
            }
            else
            {
                NameTextBox.Text = string.Empty;
                MinuteUpDown.Value = 4;
                SecondUpDown.Value = 0;
                TeaEditGroup.Text = item.ToString();
                CancelBtn.Enabled = false;
                SaveBtn.Enabled = true;
                SaveBtn.Text = "Create";
            }
            NameTextBox.Focus();
        }

        private void TeaRepositoryView_DragDrop(object sender, DragEventArgs e)
        {
            var location = TeaRepositoryView.PointToClient(new Point(e.X, e.Y));
            int draggedIndex = TeaRepositoryView.IndexFromPoint(location);
            if (draggedIndex < 0) draggedIndex = TeaRepositoryView.Items.Count - 2;
            var fromIndex = (int)e.Data.GetData(typeof(int));
            if (fromIndex == draggedIndex) return;
            var item = TeaRepositoryView.Items[fromIndex];
            if (item is IEntry<TeaTimerDefinition>)
            {
                var entry = (IEntry<TeaTimerDefinition>)item;
                _setup.Repository.MoveTo(entry, draggedIndex);
                TeaRepositoryView.Items.Clear();
                TeaRepositoryView.Items.AddRange(_setup.Repository.ToArray());
                TeaRepositoryView.Items.Add("<New>");
                TeaRepositoryView.SelectedIndex = draggedIndex;
            }
        }

        private void TeaRepositoryView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TeaRepositoryView_MouseDown(object sender, MouseEventArgs e)
        {
            var pointedIndex = TeaRepositoryView.IndexFromPoint(e.Location);
            if (pointedIndex < 0) return;

            if (e.Button == MouseButtons.Right)
            {
                TeaRepositoryView.ContextMenu.Show(TeaRepositoryView, e.Location);
            }
            else
            {
                UpdateFormFieldsFromRepositoryView(pointedIndex);

                if (pointedIndex != TeaRepositoryView.Items.Count - 1)
                {
                    TeaRepositoryView.DoDragDrop(pointedIndex, DragDropEffects.Move);
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            NameTextBox.Focus();
            base.OnShown(e);
        }

        private void TeaRepositoryView_DeleteItem(object sender, EventArgs e)
        {
            var idx = TeaRepositoryView.SelectedIndex;
            var item = TeaRepositoryView.Items[idx];
            if (item is IEntry<TeaTimerDefinition>)
            {
                var entry = (IEntry<TeaTimerDefinition>)item;
                if (MessageBox.Show(this, "Are you sure you " + Environment.NewLine
                    + "want to delete " + Environment.NewLine
                    + entry.Value.Name + "?", string.Format("Delete {0}", entry.Value.Name),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)
                    == DialogResult.Yes)
                {
                    _setup.Repository.Remove(entry);
                    TeaRepositoryView.Items.RemoveAt(idx);
                    TeaRepositoryView.SelectedIndex = TeaRepositoryView.Items.Count - 1;
                    UpdateFormFieldsFromRepositoryView(TeaRepositoryView.SelectedIndex);
                }
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            var idx = TeaRepositoryView.SelectedIndex;
            var item = TeaRepositoryView.Items[idx];
            var newDefinition = TeaTimerDefinition.Create(NameTextBox.Text, (int)MinuteUpDown.Value, (int)SecondUpDown.Value);
            if (newDefinition.IsError)
            {
                MessageBox.Show(this, ((Error)newDefinition).Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            TeaRepositoryView.BeginUpdate();
            if (item is IEntry<TeaTimerDefinition>)
            {
                var entry = (IEntry<TeaTimerDefinition>)item;
                entry.Value = newDefinition;
                _setup.Repository.Update(entry);
                TeaRepositoryView.Items[idx] = entry;
            }
            else
            {
                var entry = _setup.Repository.Add(newDefinition);
                TeaRepositoryView.Items.Insert(TeaRepositoryView.Items.Count - 1, entry);
                SaveBtn.Text = "Save";
            }
            TeaRepositoryView.SelectedIndex = idx;
            TeaRepositoryView.EndUpdate();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            var idx = TeaRepositoryView.SelectedIndex;
            if (idx == TeaRepositoryView.Items.Count - 1)
            {
                NameTextBox.Text = string.Empty;
                MinuteUpDown.Value = 4;
                SecondUpDown.Value = 0;
            }
            else
            {
                TeaRepositoryView.SelectedIndex = TeaRepositoryView.Items.Count - 1;
            }
            NameTextBox.Focus();
        }

        private void ButtonsPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var ctl = ButtonsPanel.GetChildAtPoint(e.Location);
            if (ctl != null && SaveBtn == ctl)
            {
                if (!_toolTipShown && !SaveBtn.Enabled)
                {
                    _saveToolTip.Show(_saveToolTipText, SaveBtn, -54, -16);
                    _toolTipShown = true;
                }
            }
            else
            {
                _saveToolTip.Hide(SaveBtn);
                _toolTipShown = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            e.Cancel = true;
            Hide();
        }

        private void UpdateSaveButton()
        {
            var item = TeaRepositoryView.Items[TeaRepositoryView.SelectedIndex];
            if (item is IEntry<TeaTimerDefinition>)
            {
                var entry = (IEntry<TeaTimerDefinition>)item;
                SaveBtn.Enabled = !_setup.Handles.ContainsKey(entry);
            }
        }

        private void Setup_TimerStopped(object sender, Setup.TeaTimerEventArgs e)
        {
            if (InvokeRequired) Invoke((Action)(() => UpdateSaveButton())); else UpdateSaveButton();
        }

        private void Setup_TimerStarted(object sender, Setup.TeaTimerEventArgs e)
        {
            if (InvokeRequired) Invoke((Action)(() => UpdateSaveButton())); else UpdateSaveButton();
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
