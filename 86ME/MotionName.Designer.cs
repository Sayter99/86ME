namespace _86ME_ver2
{
    partial class MotionName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MotionName));
            this.motionNameText = new System.Windows.Forms.MaskedTextBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.motionNameLabel = new System.Windows.Forms.Label();
            this.warningLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // motionNameText
            // 
            this.motionNameText.Location = new System.Drawing.Point(90, 6);
            this.motionNameText.Name = "motionNameText";
            this.motionNameText.Size = new System.Drawing.Size(265, 22);
            this.motionNameText.TabIndex = 0;
            this.motionNameText.KeyDown += new System.Windows.Forms.KeyEventHandler(motionNameText_KeyDown);
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(199, 63);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(280, 63);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // motionNameLabel
            // 
            this.motionNameLabel.AutoSize = true;
            this.motionNameLabel.Location = new System.Drawing.Point(12, 9);
            this.motionNameLabel.Name = "motionNameLabel";
            this.motionNameLabel.Size = new System.Drawing.Size(72, 12);
            this.motionNameLabel.TabIndex = 3;
            this.motionNameLabel.Text = "Motion Name:";
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.ForeColor = System.Drawing.Color.Red;
            this.warningLabel.Location = new System.Drawing.Point(12, 35);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(0, 12);
            this.warningLabel.TabIndex = 4;
            // 
            // MotionName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 98);
            this.ControlBox = false;
            this.Controls.Add(this.warningLabel);
            this.Controls.Add(this.motionNameLabel);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.motionNameText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MotionName";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Input Motion Name";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox motionNameText;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label motionNameLabel;
        private System.Windows.Forms.Label warningLabel;
    }
}