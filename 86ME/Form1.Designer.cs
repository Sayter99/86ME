﻿namespace _86ME_ver1._0
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.howToUseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.autocheck = new System.Windows.Forms.CheckBox();
            this.capturebutton = new System.Windows.Forms.Button();
            this.delaytext = new System.Windows.Forms.MaskedTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.typecombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Framelist = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MotionTest = new System.Windows.Forms.Button();
            this.NewMotion = new System.Windows.Forms.Button();
            this.Motionlist = new System.Windows.Forms.ListBox();
            this.MotionCombo = new System.Windows.Forms.ComboBox();
            this.Generate = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.GenerateAllInOne = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.Framelist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1024, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.saveFileToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.newToolStripMenuItem.Text = "File";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.fileToolStripMenuItem.Text = "New Project";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.actionToolStripMenuItem.Text = "Load Project";
            this.actionToolStripMenuItem.Click += new System.EventHandler(this.actionToolStripMenuItem_Click);
            // 
            // saveFileToolStripMenuItem
            // 
            this.saveFileToolStripMenuItem.Name = "saveFileToolStripMenuItem";
            this.saveFileToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveFileToolStripMenuItem.Text = "Save Project";
            this.saveFileToolStripMenuItem.Click += new System.EventHandler(this.saveFileToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.editToolStripMenuItem.Text = "Robot Configuration";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.optionToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToUseToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // howToUseToolStripMenuItem
            // 
            this.howToUseToolStripMenuItem.Name = "howToUseToolStripMenuItem";
            this.howToUseToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.howToUseToolStripMenuItem.Text = "Tutorials";
            this.howToUseToolStripMenuItem.Click += new System.EventHandler(this.howToUseToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // optionToolStripMenuItem
            // 
            this.optionToolStripMenuItem.Name = "optionToolStripMenuItem";
            this.optionToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.optionToolStripMenuItem.Text = "Options";
            this.optionToolStripMenuItem.Click += new System.EventHandler(this.optionToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Location = new System.Drawing.Point(13, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(720, 663);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Action";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Location = new System.Drawing.Point(321, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(399, 144);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Hint";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.richTextBox1.Location = new System.Drawing.Point(3, 18);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(393, 123);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "   ___   __   ____        _\n  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n  / _ \\| \'_ \\|" +
    " | | | | | | | \'_ \\ / _ \\\n | (_) | (_) | |_| | |_| | | | | | (_) |\n  \\___/ \\___/" +
    "|____/ \\__,_|_|_| |_|\\___/";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.autocheck);
            this.panel1.Controls.Add(this.capturebutton);
            this.panel1.Controls.Add(this.delaytext);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.typecombo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(303, 120);
            this.panel1.TabIndex = 4;
            // 
            // autocheck
            // 
            this.autocheck.AutoSize = true;
            this.autocheck.Location = new System.Drawing.Point(226, 67);
            this.autocheck.Name = "autocheck";
            this.autocheck.Size = new System.Drawing.Size(70, 16);
            this.autocheck.TabIndex = 9;
            this.autocheck.Text = "Auto Play";
            this.autocheck.UseVisualStyleBackColor = true;
            this.autocheck.CheckedChanged += new System.EventHandler(this.autocheck_CheckedChanged);
            // 
            // capturebutton
            // 
            this.capturebutton.Location = new System.Drawing.Point(221, 28);
            this.capturebutton.Name = "capturebutton";
            this.capturebutton.Size = new System.Drawing.Size(75, 23);
            this.capturebutton.TabIndex = 7;
            this.capturebutton.Text = "Capture";
            this.capturebutton.UseMnemonic = false;
            this.capturebutton.UseVisualStyleBackColor = true;
            this.capturebutton.Click += new System.EventHandler(this.capturebutton_Click);
            // 
            // delaytext
            // 
            this.delaytext.Location = new System.Drawing.Point(80, 63);
            this.delaytext.Name = "delaytext";
            this.delaytext.Size = new System.Drawing.Size(135, 22);
            this.delaytext.TabIndex = 6;
            this.delaytext.Text = "0";
            this.delaytext.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.delaytext.TextChanged += new System.EventHandler(this.delaytext_TextChanged);
            this.delaytext.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.delaytext_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Delay (ms):";
            // 
            // typecombo
            // 
            this.typecombo.FormattingEnabled = true;
            this.typecombo.Items.AddRange(new object[] {
            "Frame",
            "Delay",
            "Goto",
            "Flag"});
            this.typecombo.Location = new System.Drawing.Point(80, 31);
            this.typecombo.Name = "typecombo";
            this.typecombo.Size = new System.Drawing.Size(135, 20);
            this.typecombo.TabIndex = 0;
            this.typecombo.TextChanged += new System.EventHandler(this.typecombo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Action Type:";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Controls.Add(this.Framelist);
            this.panel2.Location = new System.Drawing.Point(12, 150);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(700, 494);
            this.panel2.TabIndex = 6;
            // 
            // Framelist
            // 
            this.Framelist.AutoScroll = true;
            this.Framelist.BackColor = System.Drawing.Color.Transparent;
            this.Framelist.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Framelist.Controls.Add(this.pictureBox1);
            this.Framelist.Location = new System.Drawing.Point(3, 3);
            this.Framelist.Name = "Framelist";
            this.Framelist.Size = new System.Drawing.Size(694, 488);
            this.Framelist.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(694, 488);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.MotionTest);
            this.groupBox3.Controls.Add(this.NewMotion);
            this.groupBox3.Controls.Add(this.Motionlist);
            this.groupBox3.Controls.Add(this.MotionCombo);
            this.groupBox3.Location = new System.Drawing.Point(739, 28);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(273, 604);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Motion";
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(7, 92);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(100, 15);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = "Action List:";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(7, 47);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(100, 15);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "Motion Name:";
            // 
            // MotionTest
            // 
            this.MotionTest.Location = new System.Drawing.Point(142, 18);
            this.MotionTest.Name = "MotionTest";
            this.MotionTest.Size = new System.Drawing.Size(125, 23);
            this.MotionTest.TabIndex = 3;
            this.MotionTest.Text = "Motion Test";
            this.MotionTest.UseVisualStyleBackColor = true;
            this.MotionTest.Click += new System.EventHandler(this.MotionTest_Click);
            // 
            // NewMotion
            // 
            this.NewMotion.Cursor = System.Windows.Forms.Cursors.Cross;
            this.NewMotion.Location = new System.Drawing.Point(6, 18);
            this.NewMotion.Name = "NewMotion";
            this.NewMotion.Size = new System.Drawing.Size(125, 23);
            this.NewMotion.TabIndex = 2;
            this.NewMotion.Text = "Add Motion";
            this.NewMotion.UseVisualStyleBackColor = true;
            this.NewMotion.Click += new System.EventHandler(this.NewMotion_Click);
            // 
            // Motionlist
            // 
            this.Motionlist.Font = new System.Drawing.Font("PMingLiU", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Motionlist.FormattingEnabled = true;
            this.Motionlist.IntegralHeight = false;
            this.Motionlist.ItemHeight = 24;
            this.Motionlist.Location = new System.Drawing.Point(7, 113);
            this.Motionlist.Name = "Motionlist";
            this.Motionlist.ScrollAlwaysVisible = true;
            this.Motionlist.Size = new System.Drawing.Size(261, 484);
            this.Motionlist.TabIndex = 1;
            this.Motionlist.SelectedIndexChanged += new System.EventHandler(this.Motionlist_SelectedIndexChanged);
            this.Motionlist.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Motionlist_KeyDown);
            this.Motionlist.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Motionlist_KeyUp);
            this.Motionlist.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Motionlist_MouseDown);
            // 
            // MotionCombo
            // 
            this.MotionCombo.FormattingEnabled = true;
            this.MotionCombo.Location = new System.Drawing.Point(7, 66);
            this.MotionCombo.Name = "MotionCombo";
            this.MotionCombo.Size = new System.Drawing.Size(260, 20);
            this.MotionCombo.TabIndex = 0;
            this.MotionCombo.SelectedIndexChanged += new System.EventHandler(this.MotionCombo_SelectedIndexChanged);
            this.MotionCombo.TextChanged += new System.EventHandler(this.MotionCombo_TextChanged);
            // 
            // Generate
            // 
            this.Generate.Location = new System.Drawing.Point(746, 635);
            this.Generate.Name = "Generate";
            this.Generate.Size = new System.Drawing.Size(267, 23);
            this.Generate.TabIndex = 4;
            this.Generate.Text = "Generate Program for 86Duino";
            this.Generate.UseVisualStyleBackColor = true;
            this.Generate.Click += new System.EventHandler(this.Generate_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // GenerateAllInOne
            // 
            this.GenerateAllInOne.Location = new System.Drawing.Point(746, 665);
            this.GenerateAllInOne.Name = "GenerateAllInOne";
            this.GenerateAllInOne.Size = new System.Drawing.Size(267, 23);
            this.GenerateAllInOne.TabIndex = 5;
            this.GenerateAllInOne.Text = "Generate All In One Program for 86Duino";
            this.GenerateAllInOne.UseVisualStyleBackColor = true;
            this.GenerateAllInOne.Click += new System.EventHandler(this.GenerateAllInOne_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1024, 698);
            this.Controls.Add(this.GenerateAllInOne);
            this.Controls.Add(this.Generate);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(1040, 980);
            this.MinimumSize = new System.Drawing.Size(1040, 736);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "86Duino Motion Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.Framelist.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem howToUseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox Motionlist;
        private System.Windows.Forms.ComboBox MotionCombo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MaskedTextBox delaytext;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox typecombo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button NewMotion;
        private System.Windows.Forms.Button capturebutton;
        private System.Windows.Forms.Button MotionTest;
        private System.Windows.Forms.CheckBox autocheck;
        private System.Windows.Forms.Panel Framelist;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Generate;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Button GenerateAllInOne;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

