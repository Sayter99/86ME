namespace _86ME_ver1._0
{
    partial class TriggerSet
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Keyvalue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(57, 33);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(65, 22);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(144, 33);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(65, 22);
            this.textBox2.TabIndex = 3;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "Trigger:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "+";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 98);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button1_KeyDown);
            this.button1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.button1_PreviewKeyDown);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(122, 98);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Cancle";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.button2_KeyDown);
            this.button2.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.button2_PreviewKeyDown);
            // 
            // Keyvalue
            // 
            this.Keyvalue.Enabled = false;
            this.Keyvalue.Location = new System.Drawing.Point(57, 65);
            this.Keyvalue.Name = "Keyvalue";
            this.Keyvalue.Size = new System.Drawing.Size(102, 22);
            this.Keyvalue.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "Key:";
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(11, 9);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(216, 15);
            this.textBox3.TabIndex = 10;
            this.textBox3.Text = "Set a hotkey from keyboard, eg. Z or Ctrl+Z";
            // 
            // TriggerSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(231, 125);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Keyvalue);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(247, 163);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(247, 163);
            this.Name = "TriggerSet";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Trigger";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox Keyvalue;
        private System.Windows.Forms.TextBox textBox3;
    }
}