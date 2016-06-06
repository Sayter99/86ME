using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace _86ME_ver1
{
    public partial class NewMotion : Form
    {
        public Dictionary<string, string> NewMotion_lang_dic;
        public Arduino arduino = null;
        public Panel[] fpanel = new Panel[45];
        public Label[] flabel = new Label[45];
        public Label[] flabel2 = new Label[45];
        public ComboBox[] fbox = new ComboBox[45];
        public ComboBox[] fbox2 = new ComboBox[45];
        public MaskedTextBox[] ftext = new MaskedTextBox[45];
        public MaskedTextBox[] ftext2 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext3 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext4 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext5 = new MaskedTextBox[45];
        public CheckBox[] fcheck = new CheckBox[45];
        public HScrollBar[] fbar_off = new HScrollBar[45];
        public HScrollBar[] fbar_home = new HScrollBar[45];
        int[] offset = new int[45];
        int last_index;
        int last_IMU;
        int[] autoframe = new int[45];
        int[] autogain = new int[45];
        uint[] homeframe = new uint[45];
        uint[] Max = new uint[45];
        uint[] min = new uint[45];
        public double[] p_gain = new double[45];
        public Quaternion q = new Quaternion();
        private Quaternion autoq = new Quaternion();
        char[] delimiterChars = { ' ', '\t', '\r', '\n' };
        public string picfilename = null;
        public int[] channelx = new int[45];
        public int[] channely = new int[45];
        public bool newflag = false;
        string last_picfilename = null;
        public Thread sync;
        public ManualResetEvent thread_event = new ManualResetEvent(true);
        private Progress init_ProcessBar = null;
        private delegate bool IncreaseHandle(int nValue);
        private IncreaseHandle progress_Increase = null;
        private object serial_lock = new object();
        private bool send_msg = false;
        private bool renew_quaternion = true;

        public NewMotion(Dictionary<string, string> lang_dic)
        {
            InitializeComponent();
            NewMotion_lang_dic = lang_dic;
            comboBox1.Items.AddRange(new object[] { "86Duino_One",
                                                    "86Duino_Zero",
                                                    "86Duino_EduCake",
                                                    });
            comboBox1.SelectedIndex = 0;

            comboBox2.Items.AddRange(new object[] { "NONE",
                                                    "86Duino One On-Board IMU",
                                                    "RM-G146",
                                                    });
            comboBox2.SelectedIndex = 0;
            maskedTextBox1.Text = (q.w).ToString();
            maskedTextBox2.Text = (q.x).ToString();
            maskedTextBox3.Text = (q.y).ToString();
            maskedTextBox4.Text = (q.z).ToString();
            maskedTextBox1.Enabled = false;
            maskedTextBox2.Enabled = false;
            maskedTextBox3.Enabled = false;
            maskedTextBox4.Enabled = false;
            getQ.Enabled = false;
            init_imu.Enabled = false;
            for (int i = 0; i < 45; i++)
            {
                offset[i] = 0;
                min[i] = 600;
                Max[i] = 2400;
                homeframe[i] = 1500;
                channelx[i] = 0;
                channely[i] = 0;
                p_gain[i] = 0;
            }
            create_panel(0, 45, 0);
            label10.Enabled = false;
            label11.Enabled = false;
            applyLang();
            sync = new Thread(() => synchronizer());
            sync.IsBackground = true;
            sync.Start();
        }

        private void update_autoframe(int i)
        {
            if (fcheck[i].Checked == true)
            {
                if (fbox2[i].SelectedIndex != 2 && getQ.Enabled == true)
                {
                    try
                    {
                        if (renew_quaternion)
                        {
                            arduino.getQ();
                            DateTime time_start = DateTime.Now;
                            while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 100) ;
                            arduino.dataRecieved = false;
                            autoq.w = arduino.quaternion[0];
                            autoq.x = arduino.quaternion[1];
                            autoq.y = arduino.quaternion[2];
                            autoq.z = arduino.quaternion[3];
                            renew_quaternion = false;
                        }
                        RollPitchYaw rpy = (autoq.Normalized().Round(4) * q.Normalized().Round(4).Inverse()).toRPY();
                        int gain = (int)Math.Round(rpy.rpy[fbox2[i].SelectedIndex] * (180 / Math.PI) * p_gain[i]);
                        if (Math.Abs(autogain[i] - gain) > 4 * p_gain[i])
                            autogain[i] = gain;
                    }
                    catch
                    {
                        autogain[i] = 0;
                    }
                }
                else
                {
                    autogain[i] = 0;
                }
                int pos = (int)homeframe[i] + offset[i] + autogain[i];
                if ((uint)pos >= min[i] && (uint)pos <= Max[i])
                {
                    if (autoframe[i] != pos)
                    {
                        autoframe[i] = pos;
                        send_msg = true;
                    }
                    return;
                }
            }
            autoframe[i] = 0;
        }

        private void synchronizer()
        {
            while (true)
            {
                lock (serial_lock)
                {
                    if (arduino != null)
                    {
                        for (int i = 0; i < 45; i++)
                            update_autoframe(i);
                        renew_quaternion = true;
                        if (send_msg)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, 0);
                                Thread.Sleep(33);
                                send_msg = false;
                            }
                            catch { }
                        }
                    }
                    thread_event.WaitOne();
                }
            }
        }

        public void applyLang()
        {
            this.Text = NewMotion_lang_dic["NewMotion_title"];
            checkBox2.Text = NewMotion_lang_dic["NewMotion_checkBox2_Text"];
            label1.Text = NewMotion_lang_dic["NewMotion_label1_Text"];
            label2.Text = NewMotion_lang_dic["NewMotion_label2_Text"];
            label3.Text = NewMotion_lang_dic["NewMotion_label3_Text"];
            label4.Text = NewMotion_lang_dic["NewMotion_label4_Text"];
            label5.Text = NewMotion_lang_dic["NewMotion_label5_Text"];
            button3.Text = NewMotion_lang_dic["NewMotion_button3_Text"];
            init_imu.Text = NewMotion_lang_dic["NewMotion_init_imu_Text"];
            getQ.Text = NewMotion_lang_dic["NewMotion_getQ_Text"];
            ttp.SetToolTip(button3, NewMotion_lang_dic["NewMotion_loadpic_ToolTip"]);
            ttp.SetToolTip(checkBox2, NewMotion_lang_dic["NewMotion_minMax_ToolTip"]);
            for (int i = 0; i < 45; i++)
            {
                fcheck[i].Text = NewMotion_lang_dic["NewMotion_fcheckText"];
                ttp.SetToolTip(fcheck[i], NewMotion_lang_dic["NewMotion_fcheck_ToolTip"]);
            }
        }

        private void IMU_DropDown(object sender, EventArgs e)
        {
            last_IMU = comboBox2.SelectedIndex;
        }

        public void SetIMUUI(int imu)
        {
            if (imu == 0)
            {
                init_imu.Enabled = false;
                getQ.Enabled = false;
                maskedTextBox1.Enabled = false;
                maskedTextBox2.Enabled = false;
                maskedTextBox3.Enabled = false;
                maskedTextBox4.Enabled = false;
                label10.Enabled = false;
                label11.Enabled = false;
                for (int i = 0; i < 45; i++)
                {
                    ftext5[i].Enabled = false;
                    fbox2[i].Enabled = false;
                }
            }
            else
            {
                if (arduino != null)
                    init_imu.Enabled = true;
                getQ.Enabled = false;
                maskedTextBox1.Enabled = true;
                maskedTextBox2.Enabled = true;
                maskedTextBox3.Enabled = true;
                maskedTextBox4.Enabled = true;
                label10.Enabled = true;
                label11.Enabled = true;
                for (int i = 0; i < 45; i++)
                {
                    ftext5[i].Enabled = true;
                    fbox2[i].Enabled = true;
                }
            }
        }

        private void IMU_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == last_IMU)
                return;
            else
                SetIMUUI(comboBox2.SelectedIndex);
        }

        private void Quaternion_TextChanged(object sender, EventArgs e)
        {
            double output;
            if (double.TryParse(((MaskedTextBox)sender).Text, out output))
            {
                switch(((MaskedTextBox)sender).Name)
                {
                    case "maskedTextBox1":
                        q.w = output;
                        break;
                    case "maskedTextBox2":
                        q.x = output;
                        break;
                    case "maskedTextBox3":
                        q.y = output;
                        break;
                    case "maskedTextBox4":
                        q.z = output;
                        break;
                    default:
                        break;
                }
            }
            else if (((MaskedTextBox)sender).Text == "-" || ((MaskedTextBox)sender).Text == "" ||
                     ((MaskedTextBox)sender).Text == "-." || ((MaskedTextBox)sender).Text == ".")
            {
                switch (((MaskedTextBox)sender).Name)
                {
                    case "maskedTextBox1":
                        q.w = 1;
                        break;
                    case "maskedTextBox2":
                        q.x = 0;
                        break;
                    case "maskedTextBox3":
                        q.y = 0;
                        break;
                    case "maskedTextBox4":
                        q.z = 0;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                MessageBox.Show(NewMotion_lang_dic["errorMsg19"]);
                ((MaskedTextBox)sender).SelectAll();
            }
        }

        public void floatcheck(object sender, KeyPressEventArgs e) //Text number check
        {
            if ((int)e.KeyChar == 46 && ((MaskedTextBox)sender).Text.IndexOf('.') != -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)('-') && ((MaskedTextBox)sender).Text.IndexOf('-') != -1)
            {
                e.Handled = true;
            }
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 46 & (int)e.KeyChar != 8 & e.KeyChar != (char)('-'))
            {
                e.Handled = true;
            }
        }

        public void numbercheck(object sender, KeyPressEventArgs e) //Text number check
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & (int)e.KeyChar != 189)
            {
                e.Handled = true;
            }
        }

        public void numbercheck_offset(object sender, KeyPressEventArgs e) //Text number check
        {
            if (e.KeyChar == (char)('-') && ((MaskedTextBox)sender).Text.IndexOf('-') != -1)
            {
                e.Handled = true;
            }
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & e.KeyChar != (char)('-'))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            thread_event.Reset();
            pic_loaded.Text = last_picfilename;
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thread_event.Reset();
            if (String.Compare(comboBox1.SelectedItem.ToString(), "---unset---") == 0)
                MessageBox.Show(NewMotion_lang_dic["NewMotion_err1"]);
            else
                this.DialogResult = DialogResult.OK;
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            last_index = comboBox1.SelectedIndex;
        }

        public void clear_Channels()
        {
            for (int i = 0; i < 45; i++)
            {
                if (fpanel[i] != null)
                    fpanel[i].Controls.Clear();
            }
            channelver.Controls.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (last_index == comboBox1.SelectedIndex)
            {
            }
            else if (string.Compare(comboBox1.Text, "86Duino_One") == 0)
            {
                clear_Channels();
                create_panel(0, 45, 0);
            }
            else if (string.Compare(comboBox1.Text, "86Duino_Zero") == 0)
            {
                clear_Channels();
                create_panel(0, 14, 0);
                create_panel(42, 45, 14);
            }
            else if (string.Compare(comboBox1.Text, "86Duino_EduCake") == 0)
            {
                clear_Channels();
                create_panel(0, 21, 0);
                create_panel(31, 33, 21);
                create_panel(42, 45, 23);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                for (int i = 0; i < 45; i++)
                {
                    ftext3[i].Enabled = true;
                    ftext4[i].Enabled = true;
                }
            }
            else
            {
                for (int i = 0; i < 45; i++)
                {
                    ftext3[i].Enabled = false;
                    ftext4[i].Enabled = false;
                }
            }
        }

        public void write_back()
        {
            q.w = double.Parse(maskedTextBox1.Text);
            q.x = double.Parse(maskedTextBox2.Text);
            q.y = double.Parse(maskedTextBox3.Text);
            q.z = double.Parse(maskedTextBox4.Text);
            for (int i=0; i<45; i++)
            {
                offset[i] = int.Parse(ftext[i].Text);
                homeframe[i] = uint.Parse(ftext2[i].Text);
                min[i] = uint.Parse(ftext3[i].Text);
                Max[i] = uint.Parse(ftext4[i].Text);
                p_gain[i] = double.Parse(ftext5[i].Text);
            }
        }

        public void create_panel(int low, int high, int start_pos)
        {
            for (int i = low; i < high; i++, start_pos++)
            {
                fpanel[i] = new Panel();
                flabel[i] = new Label();
                flabel2[i] = new Label();
                fbox[i] = new ComboBox();
                fbox2[i] = new ComboBox();
                ftext[i] = new MaskedTextBox();
                ftext2[i] = new MaskedTextBox();
                ftext3[i] = new MaskedTextBox();
                ftext4[i] = new MaskedTextBox();
                ftext5[i] = new MaskedTextBox();
                fcheck[i] = new CheckBox();
                fbar_off[i] = new HScrollBar();
                fbar_home[i] = new HScrollBar();

                fpanel[i].Size = new Size(660, 50);
                fpanel[i].Top += 3 + start_pos * 50;

                flabel[i].Size = new Size(65, 18);
                flabel[i].Top += 5;
                flabel[i].Left += 5;

                flabel2[i].Size = new Size(2, 47);
                flabel2[i].Left += 534;
                flabel2[i].BorderStyle = BorderStyle.FixedSingle;

                fbox[i].DropDownStyle = ComboBoxStyle.DropDownList;
                fbox[i].Size = new Size(135, 22);
                fbox[i].Left += 70;

                fbox2[i].DropDownStyle = ComboBoxStyle.DropDownList;
                fbox2[i].Size = new Size(50, 22);
                fbox2[i].Left += 555;

                fcheck[i].Top += 24;
                fcheck[i].Left += 125;
                fcheck[i].Size = new Size(75, 22);
                fcheck[i].Text = NewMotion_lang_dic["NewMotion_fcheckText"];
                fcheck[i].Name = i.ToString();
                fcheck[i].Checked = false;
                fcheck[i].Enabled = false;
                ttp.SetToolTip(fcheck[i], NewMotion_lang_dic["NewMotion_fcheck_ToolTip"]);

                ftext[i].Name = i.ToString();
                ftext[i].Text = offset[i].ToString();
                ftext[i].TextAlign = HorizontalAlignment.Right;
                ftext[i].KeyPress += new KeyPressEventHandler(numbercheck_offset);
                ftext[i].TextChanged += new EventHandler(check_offset);
                ftext[i].Size = new Size(90, 22);
                ftext[i].Left += 210;

                ftext2[i].Name = i.ToString();
                ftext2[i].Text = homeframe[i].ToString();
                ftext2[i].TextAlign = HorizontalAlignment.Right;
                ftext2[i].KeyPress += new KeyPressEventHandler(numbercheck);
                ftext2[i].TextChanged += new EventHandler(check_homeframe);
                ftext2[i].Size = new Size(120, 22);
                ftext2[i].Left += 305;

                ftext3[i].Name = i.ToString();
                ftext3[i].Text = min[i].ToString();
                ftext3[i].TextAlign = HorizontalAlignment.Right;
                ftext3[i].KeyPress += new KeyPressEventHandler(numbercheck);
                ftext3[i].TextChanged += new EventHandler(check_range);
                ftext3[i].Size = new Size(40, 22);
                ftext3[i].Left += 430;
                ftext3[i].Enabled = false;

                ftext4[i].Name = i.ToString();
                ftext4[i].Text = Max[i].ToString();
                ftext4[i].TextAlign = HorizontalAlignment.Right;
                ftext4[i].KeyPress += new KeyPressEventHandler(numbercheck);
                ftext4[i].TextChanged += new EventHandler(check_range);
                ftext4[i].Size = new Size(40, 22);
                ftext4[i].Left += 475;
                ftext4[i].Enabled = false;

                ftext5[i].Name = i.ToString();
                ftext5[i].Text = p_gain[i].ToString();
                ftext5[i].TextAlign = HorizontalAlignment.Right;
                ftext5[i].KeyPress += new KeyPressEventHandler(floatcheck);
                ftext5[i].TextChanged += new EventHandler(check_pgain);
                ftext5[i].Size = new Size(40, 22);
                ftext5[i].Left += 610;
                ftext5[i].Enabled = false;

                fbar_off[i].Name = i.ToString();
                fbar_off[i].Top += 24;
                fbar_off[i].Left += 210;
                fbar_off[i].Size = new Size(90, 22);
                fbar_off[i].Minimum = -256;
                fbar_off[i].Maximum = 255 + 9;
                fbar_off[i].Value = int.Parse(ftext[i].Text);
                fbar_off[i].Scroll += new ScrollEventHandler(scroll_off);

                fbar_home[i].Name = i.ToString();
                fbar_home[i].Top += 24;
                fbar_home[i].Left += 305;
                fbar_home[i].Size = new Size(120, 22);
                fbar_home[i].Minimum = int.Parse(ftext3[i].Text);
                fbar_home[i].Maximum = int.Parse(ftext4[i].Text) + 9;
                fbar_home[i].Value = int.Parse(ftext2[i].Text);
                fbar_home[i].Scroll += new ScrollEventHandler(scroll_home);

                fbox[i].Items.AddRange(new object[] { "---noServo---",
                                                      "KONDO_KRS786",       
                                                      "KONDO_KRS788",       
                                                      "KONDO_KRS78X",       
                                                      "KONDO_KRS4014",      
                                                      "KONDO_KRS4024",      
                                                      "HITEC_HSR8498",      
                                                      "FUTABA_S3003",       
                                                      "SHAYYE_SYS214050",   
                                                      "TOWERPRO_MG995",     
                                                      "TOWERPRO_MG996",
                                                      "TOWERPRO_SG90",
                                                      "DMP_RS0263",         
                                                      "DMP_RS1270",         
                                                      "GWS_S777",           
                                                      "GWS_S03T",           
                                                      "GWS_MICRO",
                                                      "OtherServos"});
                fbox[i].SelectedIndex = 0;
                fbox[i].Name = i.ToString();
                fbox[i].SelectedIndexChanged += new EventHandler(motors_SelectedIndexChanged);

                fbox2[i].Items.AddRange(new object[] { "roll", "pitch", "none" });
                fbox2[i].SelectedIndex = 2;
                fbox2[i].Name = i.ToString();
                fbox2[i].Enabled = false;

                if (i < 10)
                    flabel[i].Text = "SetServo " + i.ToString() + ":";
                else
                    flabel[i].Text = "SetServo" + i.ToString() + ":";

                fpanel[i].Controls.Add(flabel[i]);
                fpanel[i].Controls.Add(flabel2[i]);
                fpanel[i].Controls.Add(fbox[i]);
                fpanel[i].Controls.Add(fbox2[i]);
                fpanel[i].Controls.Add(ftext[i]);
                fpanel[i].Controls.Add(ftext2[i]);
                fpanel[i].Controls.Add(ftext3[i]);
                fpanel[i].Controls.Add(ftext4[i]);
                fpanel[i].Controls.Add(ftext5[i]);
                fpanel[i].Controls.Add(fcheck[i]);
                fpanel[i].Controls.Add(fbar_off[i]);
                fpanel[i].Controls.Add(fbar_home[i]);
                channelver.Controls.Add(fpanel[i]);
            }
        }

        public void scroll_off(object sender, ScrollEventArgs e)
        {
            this.ftext[int.Parse(((HScrollBar)sender).Name)].Text = ((HScrollBar)sender).Value.ToString();
        }

        public void scroll_home(object sender, ScrollEventArgs e)
        {
            this.ftext2[int.Parse(((HScrollBar)sender).Name)].Text = ((HScrollBar)sender).Value.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdPic = new OpenFileDialog();
            ofdPic.Filter = "JPG(*.JPG;*.JPEG);gif文件(*.GIF);PNG(*.png)|*.jpg;*.jpeg;*.gif;*.png";
            ofdPic.FilterIndex = 1;
            ofdPic.RestoreDirectory = true;
            ofdPic.FileName = "";
            if (ofdPic.ShowDialog() == DialogResult.OK)
            {
                picfilename = Path.GetFullPath(ofdPic.FileName);
                last_picfilename = pic_loaded.Text;
                string short_picfilename = Path.GetFileName(ofdPic.FileName);
                if (short_picfilename.Length < 25)
                    pic_loaded.Text = short_picfilename;
                else
                    pic_loaded.Text = short_picfilename.Substring(0, 22) + "...";
                newflag = true;
            }
            else
            {
                picfilename = null;
            }
        }

        private void motors_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < 45; i++)
            {
                if (sender.Equals(fbox[i]))
                {
                    switch(fbox[i].Text)
                    {
                        case "---noServo---":
                            fcheck[i].Enabled = false;
                            fcheck[i].Checked = false;
                            ftext3[i].Text = "600";
                            ftext4[i].Text = "2400";
                            break;
                        case "KONDO_KRS786":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "500";
                            ftext4[i].Text = "2500";
                            break;
                        case "KONDO_KRS788":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "500";
                            ftext4[i].Text = "2500";
                            break;
                        case "KONDO_KRS78X":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "500";
                            ftext4[i].Text = "2500";
                            break;
                        case "KONDO_KRS4014":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "450";
                            ftext4[i].Text = "2500";
                            break;
                        case "KONDO_KRS4024":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "630";
                            ftext4[i].Text = "2380";
                            break;
                        case "HITEC_HSR8498":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "550";
                            ftext4[i].Text = "2450";
                            break;
                        case "FUTABA_S3003":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "450";
                            ftext4[i].Text = "2350";
                            break;
                        case "SHAYYE_SYS214050":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "600";
                            ftext4[i].Text = "2350";
                            break;
                        case "TOWERPRO_MG995":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "800";
                            ftext4[i].Text = "2200";
                            break;
                        case "TOWERPRO_MG996":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "800";
                            ftext4[i].Text = "2200";
                            break;
                        case "TOWERPRO_SG90":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "500";
                            ftext4[i].Text = "2500";
                            break;
                        case "DMP_RS0263":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "700";
                            ftext4[i].Text = "2280";
                            break;
                        case "DMP_RS1270":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "670";
                            ftext4[i].Text = "2230";
                            break;
                        case "GWS_S777":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "600";
                            ftext4[i].Text = "2350";
                            break;
                        case "GWS_S03T":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "580";
                            ftext4[i].Text = "2540";
                            break;
                        case "GWS_MICRO":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "580";
                            ftext4[i].Text = "2540";
                            break;
                        case "OtherServos":
                            fcheck[i].Enabled = true;
                            ftext3[i].Text = "600";
                            ftext4[i].Text = "2400";
                            break;
                    }
                }
            }
        }

        private void check_offset(object sender, EventArgs e)
        {
            int n;
            int i = int.Parse(((MaskedTextBox)sender).Name);
            if (int.TryParse(((MaskedTextBox)sender).Text, out n))
            {
                if (n < -256)
                    ((MaskedTextBox)sender).Text = "-256";
                else if (n > 255)
                    ((MaskedTextBox)sender).Text = "255";
                else
                {
                    fbar_off[i].Value = n;
                    offset[i] = n;
                }
            }
        }

        private void check_homeframe(object sender, EventArgs e)
        {
            uint n;
            int i = int.Parse(((MaskedTextBox)sender).Name);

            if (uint.TryParse(((MaskedTextBox)sender).Text, out n))
            {
                if (n >= min[i] && n <= Max[i])
                {
                    fbar_home[i].Value = (int)n;
                    homeframe[i] = n;
                }
                else if (n > Max[i])
                {
                    ((MaskedTextBox)sender).Text = Max[i].ToString();
                    homeframe[i] = Max[i];
                    fbar_home[i].Value = (int)Max[i];
                }
            }
            else
            {
                ((MaskedTextBox)sender).Text = "1500";
                fbar_home[i].Value = 1500;
                homeframe[i] = 1500;
            }
        }

        private void check_range(object sender, EventArgs e)
        {
            int i = int.Parse(((MaskedTextBox)sender).Name);
            int _min = int.Parse(ftext3[i].Text);
            int _max = int.Parse(ftext4[i].Text);
            fbar_home[i].Minimum = _min;
            fbar_home[i].Maximum = _max + 9;
            min[i] = (uint)_min;
            Max[i] = (uint)_max;
        }

        private void check_pgain(object sender, EventArgs e)
        {
            double n;
            int i = int.Parse(((MaskedTextBox)sender).Name);

            if (double.TryParse(((MaskedTextBox)sender).Text, out n))
            {
                p_gain[i] = n;
            }
            else if (((MaskedTextBox)sender).Text == "-" || ((MaskedTextBox)sender).Text == "" ||
                     ((MaskedTextBox)sender).Text == "-." || ((MaskedTextBox)sender).Text == ".")
            {
                p_gain[i] = 0;
            }
            else
            {
                p_gain[i] = 0;
                ((MaskedTextBox)sender).Text = "0";
            }
        }

        private void init_imu_Click(object sender, EventArgs e)
        {
            Thread show_progress = new Thread(new ThreadStart(progress_thread));
            if (arduino != null)
            {
                try
                {
                    arduino.init_IMU(comboBox2.SelectedIndex);
                    show_progress.Start();
                }
                catch
                {
                    MessageBox.Show(NewMotion_lang_dic["errorMsg1"]);
                }
            }
        }

        private void getQ_Click(object sender, EventArgs e)
        {
            lock (serial_lock)
            {
                if (arduino != null)
                {
                    try
                    {
                        Quaternion detectQ = new Quaternion();
                        detectQ.w = 0;
                        int avg_times = 8;
                        for (int i = 0; i < avg_times; i++)
                        {
                            arduino.getQ();
                            DateTime time_start = DateTime.Now;
                            while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 100) ;
                            arduino.dataRecieved = false;
                            detectQ.w += arduino.quaternion[0];
                            detectQ.x += arduino.quaternion[1];
                            detectQ.y += arduino.quaternion[2];
                            detectQ.z += arduino.quaternion[3];
                            Thread.Sleep(33);
                        }
                        detectQ.w /= avg_times;
                        detectQ.x /= avg_times;
                        detectQ.y /= avg_times;
                        detectQ.z /= avg_times;
                        detectQ = detectQ.Normalized();
                        maskedTextBox1.Text = detectQ.w.ToString("0.####");
                        maskedTextBox2.Text = detectQ.x.ToString("0.####");
                        maskedTextBox3.Text = detectQ.y.ToString("0.####");
                        maskedTextBox4.Text = detectQ.z.ToString("0.####");
                    }
                    catch
                    {
                        MessageBox.Show(NewMotion_lang_dic["errorMsg1"]);
                    }
                }
            }
        }

        private void ShowProcessBar()
        {
            init_ProcessBar = new Progress();
            progress_Increase = new IncreaseHandle(init_ProcessBar.Increase);
            init_ProcessBar.ShowDialog();
            init_ProcessBar = null;
        }

        private void progress_thread()
        {
            lock (serial_lock)
            {
                MethodInvoker mi = new MethodInvoker(ShowProcessBar);
                this.BeginInvoke(mi);
                Thread.Sleep(100);
                bool blnIncreased = false;
                object objReturn = null;
                do
                {
                    if (comboBox2.SelectedIndex == 1)
                        Thread.Sleep(50);
                    else
                        Thread.Sleep(130);
                    objReturn = this.Invoke(this.progress_Increase, new object[] { 1 });
                    blnIncreased = (bool)objReturn;
                }
                while (blnIncreased);
                DateTime time_start = DateTime.Now;
                while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 500) ;
                if (arduino.dataRecieved)
                {
                    if (arduino.captured_data != 0)
                    {
                        getQ.Enabled = false;
                        MessageBox.Show(this, NewMotion_lang_dic["errorMsg21"]);
                    }
                    else
                        getQ.Enabled = true;
                }
                else
                {
                    getQ.Enabled = false;
                    MessageBox.Show(this, NewMotion_lang_dic["errorMsg1"]);
                }
                arduino.dataRecieved = false;
            }
        }
    }
}
