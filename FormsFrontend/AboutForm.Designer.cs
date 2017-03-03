namespace PalmenIt.dntt.FormsFrontend
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.IconLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.BuildInfoLabel = new System.Windows.Forms.Label();
            this.OkButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // IconLabel
            // 
            this.IconLabel.Location = new System.Drawing.Point(8, 8);
            this.IconLabel.Margin = new System.Windows.Forms.Padding(8);
            this.IconLabel.Name = "IconLabel";
            this.tableLayoutPanel1.SetRowSpan(this.IconLabel, 4);
            this.IconLabel.Size = new System.Drawing.Size(256, 256);
            this.IconLabel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.IconLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TitleLabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.DescriptionLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.BuildInfoLabel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.OkButton, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(594, 275);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(275, 0);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Padding = new System.Windows.Forms.Padding(0, 16, 0, 12);
            this.TitleLabel.Size = new System.Drawing.Size(316, 61);
            this.TitleLabel.TabIndex = 1;
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.DescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionLabel.Location = new System.Drawing.Point(275, 61);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(316, 143);
            this.DescriptionLabel.TabIndex = 0;
            this.DescriptionLabel.Text = resources.GetString("DescriptionLabel.Text");
            // 
            // BuildInfoLabel
            // 
            this.BuildInfoLabel.AutoSize = true;
            this.BuildInfoLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BuildInfoLabel.Location = new System.Drawing.Point(275, 204);
            this.BuildInfoLabel.Name = "BuildInfoLabel";
            this.BuildInfoLabel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.BuildInfoLabel.Size = new System.Drawing.Size(316, 42);
            this.BuildInfoLabel.TabIndex = 3;
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.Location = new System.Drawing.Point(516, 249);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 4;
            this.OkButton.Text = "Ok";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // AboutForm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 275);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.Text = "About DnTeaTime";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label IconLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Label BuildInfoLabel;
        private System.Windows.Forms.Button OkButton;
    }
}