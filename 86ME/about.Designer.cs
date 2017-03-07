namespace _86ME_ver2
{
    partial class about
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(about));
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.fb_button = new System.Windows.Forms.Button();
            this.web_button = new System.Windows.Forms.Button();
            this.github_button = new System.Windows.Forms.Button();
            this.author_label = new System.Windows.Forms.Label();
            this.license_label = new System.Windows.Forms.Label();
            this.version_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 11F);
            this.richTextBox1.Location = new System.Drawing.Point(12, 73);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(260, 167);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "86ME is an open source project and it is derived from RoboME of RBgod. (https://g" +
    "ithub.com/RoBoardGod/RoBoME)";
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // fb_button
            // 
            this.fb_button.Location = new System.Drawing.Point(12, 246);
            this.fb_button.Name = "fb_button";
            this.fb_button.Size = new System.Drawing.Size(75, 25);
            this.fb_button.TabIndex = 1;
            this.fb_button.Text = "FaceBook";
            this.fb_button.UseVisualStyleBackColor = true;
            this.fb_button.Click += new System.EventHandler(this.fb_button_Click);
            // 
            // web_button
            // 
            this.web_button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.web_button.Location = new System.Drawing.Point(105, 246);
            this.web_button.Name = "web_button";
            this.web_button.Size = new System.Drawing.Size(75, 25);
            this.web_button.TabIndex = 2;
            this.web_button.Text = "86Duino";
            this.web_button.UseVisualStyleBackColor = true;
            this.web_button.Click += new System.EventHandler(this.web_button_Click);
            // 
            // github_button
            // 
            this.github_button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.github_button.Location = new System.Drawing.Point(197, 246);
            this.github_button.Name = "github_button";
            this.github_button.Size = new System.Drawing.Size(75, 25);
            this.github_button.TabIndex = 3;
            this.github_button.Text = "GitHub";
            this.github_button.UseVisualStyleBackColor = true;
            this.github_button.Click += new System.EventHandler(this.github_button_Click);
            // 
            // author_label
            // 
            this.author_label.AutoSize = true;
            this.author_label.Font = new System.Drawing.Font("Consolas", 10F);
            this.author_label.Location = new System.Drawing.Point(12, 5);
            this.author_label.Name = "author_label";
            this.author_label.Size = new System.Drawing.Size(120, 17);
            this.author_label.TabIndex = 4;
            this.author_label.Text = "Author: Sayter";
            // 
            // license_label
            // 
            this.license_label.AutoSize = true;
            this.license_label.Font = new System.Drawing.Font("Consolas", 10F);
            this.license_label.Location = new System.Drawing.Point(12, 28);
            this.license_label.Name = "license_label";
            this.license_label.Size = new System.Drawing.Size(160, 17);
            this.license_label.TabIndex = 5;
            this.license_label.Text = "License: GNU GPL V2";
            // 
            // version_label
            // 
            this.version_label.AutoSize = true;
            this.version_label.Font = new System.Drawing.Font("Consolas", 10F);
            this.version_label.Location = new System.Drawing.Point(12, 51);
            this.version_label.Name = "version_label";
            this.version_label.Size = new System.Drawing.Size(216, 17);
            this.version_label.TabIndex = 7;
            this.version_label.Text = "Version: 2.0.0, 2017/03/06";
            // 
            // about
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(284, 283);
            this.Controls.Add(this.version_label);
            this.Controls.Add(this.license_label);
            this.Controls.Add(this.author_label);
            this.Controls.Add(this.github_button);
            this.Controls.Add(this.web_button);
            this.Controls.Add(this.fb_button);
            this.Controls.Add(this.richTextBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(300, 322);
            this.MinimumSize = new System.Drawing.Size(300, 322);
            this.Name = "about";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button fb_button;
        private System.Windows.Forms.Button web_button;
        private System.Windows.Forms.Button github_button;
        private System.Windows.Forms.Label author_label;
        private System.Windows.Forms.Label license_label;
        private System.Windows.Forms.Label version_label;
    }
}