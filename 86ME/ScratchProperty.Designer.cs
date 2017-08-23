namespace _86ME_ver2
{
    partial class ScratchProperty
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScratchProperty));
            this.Cancel = new System.Windows.Forms.Button();
            this.OK_button = new System.Windows.Forms.Button();
            this.portLabel = new System.Windows.Forms.Label();
            this.Serial_radioButton = new System.Windows.Forms.RadioButton();
            this.bt_radioButton = new System.Windows.Forms.RadioButton();
            this.bt_groupBox = new System.Windows.Forms.GroupBox();
            this.btPortComboBox = new System.Windows.Forms.ComboBox();
            this.btBaudComboBox = new System.Windows.Forms.ComboBox();
            this.btBaudLabel = new System.Windows.Forms.Label();
            this.btPortLabel = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathMaskedTextBox = new System.Windows.Forms.MaskedTextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.bt_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(342, 292);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 20;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK_button
            // 
            this.OK_button.Location = new System.Drawing.Point(261, 292);
            this.OK_button.Name = "OK_button";
            this.OK_button.Size = new System.Drawing.Size(75, 23);
            this.OK_button.TabIndex = 19;
            this.OK_button.Text = "Save";
            this.OK_button.UseVisualStyleBackColor = true;
            this.OK_button.Click += new System.EventHandler(this.OK_button_Click);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(12, 120);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(27, 12);
            this.portLabel.TabIndex = 21;
            this.portLabel.Text = "Port:";
            // 
            // Serial_radioButton
            // 
            this.Serial_radioButton.AutoSize = true;
            this.Serial_radioButton.Location = new System.Drawing.Point(12, 12);
            this.Serial_radioButton.Name = "Serial_radioButton";
            this.Serial_radioButton.Size = new System.Drawing.Size(49, 16);
            this.Serial_radioButton.TabIndex = 15;
            this.Serial_radioButton.TabStop = true;
            this.Serial_radioButton.Text = "Serial";
            this.Serial_radioButton.UseVisualStyleBackColor = true;
            this.Serial_radioButton.CheckedChanged += new System.EventHandler(this.Serial_radioButton_CheckedChanged);
            // 
            // bt_radioButton
            // 
            this.bt_radioButton.AutoSize = true;
            this.bt_radioButton.Location = new System.Drawing.Point(12, 34);
            this.bt_radioButton.Name = "bt_radioButton";
            this.bt_radioButton.Size = new System.Drawing.Size(14, 13);
            this.bt_radioButton.TabIndex = 16;
            this.bt_radioButton.TabStop = true;
            this.bt_radioButton.UseVisualStyleBackColor = true;
            this.bt_radioButton.CheckedChanged += new System.EventHandler(this.bt_radioButton_CheckedChanged);
            // 
            // bt_groupBox
            // 
            this.bt_groupBox.Controls.Add(this.btPortComboBox);
            this.bt_groupBox.Controls.Add(this.btBaudComboBox);
            this.bt_groupBox.Controls.Add(this.btBaudLabel);
            this.bt_groupBox.Controls.Add(this.btPortLabel);
            this.bt_groupBox.Location = new System.Drawing.Point(36, 34);
            this.bt_groupBox.Name = "bt_groupBox";
            this.bt_groupBox.Size = new System.Drawing.Size(381, 65);
            this.bt_groupBox.TabIndex = 17;
            this.bt_groupBox.TabStop = false;
            this.bt_groupBox.Text = "Bluetooth";
            // 
            // btPortComboBox
            // 
            this.btPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.btPortComboBox.FormattingEnabled = true;
            this.btPortComboBox.Items.AddRange(new object[] {
            "Serial1",
            "Serial2",
            "Serial3"});
            this.btPortComboBox.Location = new System.Drawing.Point(260, 25);
            this.btPortComboBox.Name = "btPortComboBox";
            this.btPortComboBox.Size = new System.Drawing.Size(100, 20);
            this.btPortComboBox.TabIndex = 11;
            // 
            // btBaudComboBox
            // 
            this.btBaudComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.btBaudComboBox.FormattingEnabled = true;
            this.btBaudComboBox.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.btBaudComboBox.Location = new System.Drawing.Point(79, 25);
            this.btBaudComboBox.Name = "btBaudComboBox";
            this.btBaudComboBox.Size = new System.Drawing.Size(100, 20);
            this.btBaudComboBox.TabIndex = 10;
            // 
            // btBaudLabel
            // 
            this.btBaudLabel.AutoSize = true;
            this.btBaudLabel.Location = new System.Drawing.Point(16, 28);
            this.btBaudLabel.Name = "btBaudLabel";
            this.btBaudLabel.Size = new System.Drawing.Size(57, 12);
            this.btBaudLabel.TabIndex = 8;
            this.btBaudLabel.Text = "Baud Rate:";
            // 
            // btPortLabel
            // 
            this.btPortLabel.AutoSize = true;
            this.btPortLabel.Location = new System.Drawing.Point(206, 28);
            this.btPortLabel.Name = "btPortLabel";
            this.btPortLabel.Size = new System.Drawing.Size(53, 12);
            this.btPortLabel.TabIndex = 9;
            this.btPortLabel.Text = "Used Port:";
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(10, 169);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(28, 12);
            this.pathLabel.TabIndex = 22;
            this.pathLabel.Text = "Path:";
            // 
            // pathMaskedTextBox
            // 
            this.pathMaskedTextBox.Location = new System.Drawing.Point(45, 166);
            this.pathMaskedTextBox.Name = "pathMaskedTextBox";
            this.pathMaskedTextBox.Size = new System.Drawing.Size(343, 22);
            this.pathMaskedTextBox.TabIndex = 23;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(394, 167);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(23, 20);
            this.browseButton.TabIndex = 24;
            this.browseButton.Text = "...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(45, 117);
            this.portTextBox.MaxLength = 5;
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(78, 22);
            this.portTextBox.TabIndex = 25;
            this.portTextBox.Text = "50209";
            this.portTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.portTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.numbercheck);
            // 
            // ScratchProperty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 343);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.pathMaskedTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK_button);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.Serial_radioButton);
            this.Controls.Add(this.bt_radioButton);
            this.Controls.Add(this.bt_groupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(460, 382);
            this.MinimumSize = new System.Drawing.Size(460, 382);
            this.Name = "ScratchProperty";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Properties for the generated Scratch 2.0 project";
            this.bt_groupBox.ResumeLayout(false);
            this.bt_groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK_button;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.RadioButton Serial_radioButton;
        private System.Windows.Forms.RadioButton bt_radioButton;
        private System.Windows.Forms.GroupBox bt_groupBox;
        private System.Windows.Forms.ComboBox btPortComboBox;
        private System.Windows.Forms.ComboBox btBaudComboBox;
        private System.Windows.Forms.Label btBaudLabel;
        private System.Windows.Forms.Label btPortLabel;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.MaskedTextBox pathMaskedTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox portTextBox;
    }
}