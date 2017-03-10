namespace _86ME_ver2
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
            this.button4 = new System.Windows.Forms.Button();
            this.pic_loaded = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.getQ = new System.Windows.Forms.Button();
            this.init_imu = new System.Windows.Forms.Button();
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
            this.mirror_loaded = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Location = new System.Drawing.Point(561, 606);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Default;
            this.button2.Location = new System.Drawing.Point(642, 606);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 25);
            this.button2.TabIndex = 2;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(102, 9);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(615, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.DropDown += new System.EventHandler(this.comboBox1_DropDown);
            this.comboBox1.DropDownClosed += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "86Duino Board: ";
            // 
            // channelver
            // 
            this.channelver.AutoScroll = true;
            this.channelver.BackColor = System.Drawing.SystemColors.Control;
            this.channelver.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelver.Location = new System.Drawing.Point(14, 160);
            this.channelver.Name = "channelver";
            this.channelver.Size = new System.Drawing.Size(703, 439);
            this.channelver.TabIndex = 5;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(445, 143);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(81, 17);
            this.checkBox2.TabIndex = 9;
            this.checkBox2.Text = "Min      Max";
            this.ttp.SetToolTip(this.checkBox2, "Enable or disable to modify ranges of motors.");
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Cursor = System.Windows.Forms.Cursors.Default;
            this.button3.Location = new System.Drawing.Point(12, 606);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(125, 25);
            this.button3.TabIndex = 10;
            this.button3.Text = "Load Robot Picture";
            this.ttp.SetToolTip(this.button3, "Load a picture for mapping motors while editing frames.");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Offset (-256~255)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(354, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Homeframe";
            // 
            // ttp
            // 
            this.ttp.AutoPopDelay = 10000;
            this.ttp.InitialDelay = 300;
            this.ttp.ReshowDelay = 300;
            this.ttp.ShowAlways = true;
            this.ttp.UseFading = false;
            // 
            // button4
            // 
            this.button4.Cursor = System.Windows.Forms.Cursors.Default;
            this.button4.Location = new System.Drawing.Point(289, 606);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(125, 25);
            this.button4.TabIndex = 17;
            this.button4.Text = "Load Mirror Config";
            this.ttp.SetToolTip(this.button4, "Load a mirror file containing the pairwise relationship of servos.");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // pic_loaded
            // 
            this.pic_loaded.AutoSize = true;
            this.pic_loaded.Location = new System.Drawing.Point(143, 611);
            this.pic_loaded.Name = "pic_loaded";
            this.pic_loaded.Size = new System.Drawing.Size(0, 13);
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
            this.groupBox1.Location = new System.Drawing.Point(14, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(703, 98);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Inertial Measurement Unit (IMU) Settings";
            // 
            // getQ
            // 
            this.getQ.Location = new System.Drawing.Point(455, 53);
            this.getQ.Name = "getQ";
            this.getQ.Size = new System.Drawing.Size(242, 25);
            this.getQ.TabIndex = 26;
            this.getQ.Text = "Auto Detect Orientation";
            this.getQ.UseVisualStyleBackColor = true;
            this.getQ.Click += new System.EventHandler(this.getQ_Click);
            // 
            // init_imu
            // 
            this.init_imu.Location = new System.Drawing.Point(560, 14);
            this.init_imu.Name = "init_imu";
            this.init_imu.Size = new System.Drawing.Size(137, 25);
            this.init_imu.TabIndex = 25;
            this.init_imu.Text = "Initialize IMU";
            this.init_imu.UseVisualStyleBackColor = true;
            this.init_imu.Click += new System.EventHandler(this.init_imu_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(362, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(15, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "z:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(279, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(194, 55);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(15, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "x:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(109, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 21;
            this.label6.Text = "w:";
            // 
            // maskedTextBox4
            // 
            this.maskedTextBox4.Location = new System.Drawing.Point(381, 55);
            this.maskedTextBox4.Name = "maskedTextBox4";
            this.maskedTextBox4.Size = new System.Drawing.Size(57, 20);
            this.maskedTextBox4.TabIndex = 20;
            this.maskedTextBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox4.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // maskedTextBox3
            // 
            this.maskedTextBox3.Location = new System.Drawing.Point(299, 55);
            this.maskedTextBox3.Name = "maskedTextBox3";
            this.maskedTextBox3.Size = new System.Drawing.Size(57, 20);
            this.maskedTextBox3.TabIndex = 19;
            this.maskedTextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox3.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // maskedTextBox2
            // 
            this.maskedTextBox2.Location = new System.Drawing.Point(216, 55);
            this.maskedTextBox2.Name = "maskedTextBox2";
            this.maskedTextBox2.Size = new System.Drawing.Size(57, 20);
            this.maskedTextBox2.TabIndex = 18;
            this.maskedTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox2.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(131, 55);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(57, 20);
            this.maskedTextBox1.TabIndex = 17;
            this.maskedTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.maskedTextBox1.TextChanged += new System.EventHandler(this.Quaternion_TextChanged);
            this.maskedTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.floatcheck);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "Base Orientation:";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(88, 16);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(466, 21);
            this.comboBox2.TabIndex = 15;
            this.comboBox2.DropDown += new System.EventHandler(this.IMU_DropDown);
            this.comboBox2.DropDownClosed += new System.EventHandler(this.IMU_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Select IMU:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(658, 144);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Gain";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(599, 144);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(46, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "IMU Src";
            // 
            // mirror_loaded
            // 
            this.mirror_loaded.AutoSize = true;
            this.mirror_loaded.Location = new System.Drawing.Point(420, 611);
            this.mirror_loaded.Name = "mirror_loaded";
            this.mirror_loaded.Size = new System.Drawing.Size(0, 13);
            this.mirror_loaded.TabIndex = 18;
            // 
            // NewMotion
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(729, 642);
            this.Controls.Add(this.mirror_loaded);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
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
            this.MaximumSize = new System.Drawing.Size(745, 681);
            this.MinimumSize = new System.Drawing.Size(745, 681);
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
        public System.Windows.Forms.Panel channelver;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip ttp;
        public System.Windows.Forms.Label pic_loaded;
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
        public System.Windows.Forms.Label label10;
        public System.Windows.Forms.Label label11;
        public System.Windows.Forms.Button getQ;
        public System.Windows.Forms.Button init_imu;
        public System.Windows.Forms.Label mirror_loaded;
        private System.Windows.Forms.Button button4;
    }
}