namespace _86ME_ver1
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
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // fb_button
            // 
            resources.ApplyResources(this.fb_button, "fb_button");
            this.fb_button.Name = "fb_button";
            this.fb_button.UseVisualStyleBackColor = true;
            this.fb_button.Click += new System.EventHandler(this.fb_button_Click);
            // 
            // web_button
            // 
            resources.ApplyResources(this.web_button, "web_button");
            this.web_button.Name = "web_button";
            this.web_button.UseVisualStyleBackColor = true;
            this.web_button.Click += new System.EventHandler(this.web_button_Click);
            // 
            // github_button
            // 
            resources.ApplyResources(this.github_button, "github_button");
            this.github_button.Name = "github_button";
            this.github_button.UseVisualStyleBackColor = true;
            this.github_button.Click += new System.EventHandler(this.github_button_Click);
            // 
            // author_label
            // 
            resources.ApplyResources(this.author_label, "author_label");
            this.author_label.Name = "author_label";
            // 
            // license_label
            // 
            resources.ApplyResources(this.license_label, "license_label");
            this.license_label.Name = "license_label";
            // 
            // about
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.Controls.Add(this.license_label);
            this.Controls.Add(this.author_label);
            this.Controls.Add(this.github_button);
            this.Controls.Add(this.web_button);
            this.Controls.Add(this.fb_button);
            this.Controls.Add(this.richTextBox1);
            this.Name = "about";
            this.ShowInTaskbar = false;
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
    }
}