namespace _86ME_ver1
{
    partial class NewMotion
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.channelver = new System.Windows.Forms.Panel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ttp = new System.Windows.Forms.ToolTip(this.components);
            this.pic_loaded = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.maskedTextBox4 = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox3 = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.init_imu = new System.Windows.Forms.Button();
            this.getQ = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Location = new System.Drawing.Point(395, 559);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Default;
            this.button2.Location = new System.Drawing.Point(510, 559);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(102, 8);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(584, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.DropDown += new System.EventHandler(this.comboBox1_DropDown);
            this.comboBox1.DropDownClosed += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "86Duino Board: ";
            // 
            // channelver
            // 
            this.channelver.AutoScroll = true;
            this.channelver.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelver.Location = new System.Drawing.Point(12, 146);
            this.channelver.Name = "channelver";
            this.channelver.Size = new System.Drawing.Size(680, 407);
            this.channelver.TabIndex = 5;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(443, 130);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(81, 16);
            this.checkBox2.TabIndex = 9;
            this.checkBox2.Text = "min      Max";
            this.ttp.SetToolTip(this.checkBox2, "Enable or disable to modify ranges of motors.");
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Cursor = System.Windows.Forms.Cursors.Default;
            this.button3.Location = new System.Drawing.Point(105, 559);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(125, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Load Robot Picture";
            this.ttp.SetToolTip(this.button3, "Load a picture for mapping motors while editing frames.");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(225, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "offset (-256~255)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(352, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "homeframe";
            // 
            // ttp
            // 
            this.ttp.AutoPopDelay = 10000;
            this.ttp.InitialDelay = 300;
            this.ttp.ReshowDelay = 300;
            this.ttp.ShowAlways = true;
            this.ttp.UseFading = false;
            // 
            // pic_loaded
            // 
            this.pic_loaded.AutoSize = true;
            this.pic_loaded.Location = new System.Drawing.Point(236, 564);
            this.pic_loaded.Name = "pic_loaded";
            this.pic_loaded.Size = new System.Drawing.Size(0, 12);
            this.pic_loaded.TabIndex = 13;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.getQ);
            this.groupBox1.Controls.Add(this.init_imu);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.maskedTextBox4);
            this.groupBox1.Controls.Add(this.maskedTextBox3);
            this.groupBox1.Controls.Add(this.maskedTextBox2);
            this.groupBox1.Controls.Add(this.maskedTextBox1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(14, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(678, 90);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IMU";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(362, 51);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(13, 12);
            this.label9.TabIndex = 24;
            this.label9.Text = "z:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(279, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(194, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 12);
            this.label7.TabIndex = 22;
            this.label7.Text = "x:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 12);
            this.label6.TabIndex = 21;
            this.label6.Text = "w:";
            // 
            // maskedTextBox4
            // 
            this.maskedTextBox4.Location = new System.Drawing.Point(381, 51);
            this.maskedTextBox4.Name = "maskedTextBox4";
            this.maskedTextBox4.Size = new System.Drawing.Size(57, 22);
            this.maskedTextBox4.TabIndex = 20;
            this.maskedTextBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox4.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // maskedTextBox3
            // 
            this.maskedTextBox3.Location = new System.Drawing.Point(299, 51);
            this.maskedTextBox3.Name = "maskedTextBox3";
            this.maskedTextBox3.Size = new System.Drawing.Size(57, 22);
            this.maskedTextBox3.TabIndex = 19;
            this.maskedTextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox3.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // maskedTextBox2
            // 
            this.maskedTextBox2.Location = new System.Drawing.Point(216, 51);
            this.maskedTextBox2.Name = "maskedTextBox2";
            this.maskedTextBox2.Size = new System.Drawing.Size(57, 22);
            this.maskedTextBox2.TabIndex = 18;
            this.maskedTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox2.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(131, 51);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(57, 22);
            this.maskedTextBox1.TabIndex = 17;
            this.maskedTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox1.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "Initial Quaternion:";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(88, 15);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(466, 20);
            this.comboBox2.TabIndex = 15;
            this.comboBox2.DropDown += new System.EventHandler(this.IMU_DropDown);
            this.comboBox2.DropDownClosed += new System.EventHandler(this.IMU_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 12);
            this.label4.TabIndex = 15;
            this.label4.Text = "Used IMU:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(578, 131);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(33, 12);
            this.label10.TabIndex = 15;
            this.label10.Text = "PGain";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(623, 131);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 12);
            this.label11.TabIndex = 16;
            this.label11.Text = "Source";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(531, 131);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 12);
            this.label12.TabIndex = 17;
            this.label12.Text = "Enable";
            // 
            // init_imu
            // 
            this.init_imu.Location = new System.Drawing.Point(560, 13);
            this.init_imu.Name = "init_imu";
            this.init_imu.Size = new System.Drawing.Size(112, 23);
            this.init_imu.TabIndex = 25;
            this.init_imu.Text = "Initialize IMU";
            this.init_imu.UseVisualStyleBackColor = true;
            this.init_imu.Click += new System.EventHandler(this.init_imu_Click);
            // 
            // getQ
            // 
            this.getQ.Location = new System.Drawing.Point(455, 49);
            this.getQ.Name = "getQ";
            this.getQ.Size = new System.Drawing.Size(217, 23);
            this.getQ.TabIndex = 26;
            this.getQ.Text = "Get Current Quaternion";
            this.getQ.UseVisualStyleBackColor = true;
            this.getQ.Click += new System.EventHandler(this.getQ_Click);
            // 
            // NewMotion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 594);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pic_loaded);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.channelver);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(720, 632);
            this.MinimumSize = new System.Drawing.Size(720, 632);
            this.Name = "NewMotion";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Robot Configuration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel channelver;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip ttp;
        private System.Windows.Forms.Label pic_loaded;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.MaskedTextBox maskedTextBox4;
        public System.Windows.Forms.MaskedTextBox maskedTextBox3;
        public System.Windows.Forms.MaskedTextBox maskedTextBox2;
        public System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        public System.Windows.Forms.Button getQ;
        public System.Windows.Forms.Button init_imu;
    }
}