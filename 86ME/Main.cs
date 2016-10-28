//================================================================//
//     __      ____   ____                                        //
//   /'_ `\   /'___\ /\  _`\             __                       //
//  /\ \L\ \ /\ \__/ \ \ \/\ \   __  __ /\_\     ___      ___     //
//  \/_> _ <_\ \  _``\\ \ \ \ \ /\ \/\ \\/\ \  /' _ `\   / __`\   //
//    /\ \L\ \\ \ \L\ \\ \ \_\ \\ \ \_\ \\ \ \ /\ \/\ \ /\ \L\ \  //
//    \ \____/ \ \____/ \ \____/ \ \____/ \ \_\\ \_\ \_\\ \____/  //
//     \/___/   \/___/   \/___/   \/___/   \/_/ \/_/\/_/ \/___/   //
//                                                                //
//                                       http://www.86duino.com   //
//================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Collections;
using System.Threading;
using System.Media;
using System.Globalization;
using System.Text.RegularExpressions;

namespace _86ME_ver1
{
    public partial class Main : Form
    {
        Dictionary<string, string> Main_lang_dic;
        ulong servo_onOff = ~0UL;
        int opVar_num = 50;
        double[] operand_var = new double[100];
        GlobalSettings gs = new GlobalSettings();
        bool change_board = false;
        public string init_load_file = "";
        int offset_Max = 255;
        int offset_min = -256;
        List<int> mtest_flag_goto = new List<int>();
        int mtest_start_pos = 0;
        int motiontest_state;
        enum mtest_states { start, pause, stop };
        int default_delay = 1000;
        int current_motionlist_idx = -1;
        int last_motionlist_idx = -1;
        public string com_port;
        Arduino arduino;
        private Panel[] fpanel = new Panel[45];
        Label[] flabel = new Label[45];
        MaskedTextBox[] ftext = new MaskedTextBox[45];
        CheckBox[] fcheck = new CheckBox[45];
        CheckBox[] fonoff = new CheckBox[45];
        HScrollBar[] fbar = new HScrollBar[45];
        NewMotion Motion;
        public ArrayList ME_Motionlist;
        int framecount = 0;
        int homecount = 0;
        string load_filename = "";
        string picture_name;
        uint[] homeframe = new uint[45];
        uint[] Max = new uint[45];
        uint[] min = new uint[45];
        int[] autoframe = new int[45];
        int[] offset = new int[45];
        int board_ver86;
        int used_imu;
        int[] motor_info = new int[45];
        bool[] enable_gain = new bool[45];
        double[] p_gain = new double[45];
        double[] s_gain = new double[45];
        int[] gain_source = new int[45];
        int[] gain_source2 = new int[45];
        double[] init_quaternion = new double[4];
        int mdx, mdy;
        bool[] freshflag = new bool[2];
        bool picmode_move = false;
        bool[] captured = new bool[45];
        string[] motionevent = new string[8];
        char[] delimiterChars = { ' ', '\t', '\r', '\n' };
        public Main(Dictionary<string, string> lang_dic)
        {
            InitializeComponent();
            saveFrame.Visible = false;
            loadFrame.Visible = false;
            Action_groupBox.Enabled = false;
            Hint_groupBox.Enabled = false;
            Motion_groupBox.Enabled = false;
            Setting_groupBox.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            CheckForIllegalCrossThreadCalls = false;// dangerous
            accLXText.Name = "0";
            accHXText.Name = "1";
            accLYText.Name = "2";
            accHYText.Name = "3";
            accLZText.Name = "4";
            accHZText.Name = "5";
            accDurationText.Name = "6";
            Main_lang_dic = lang_dic;
            applyLang();
        }

        public Main(string filename, Dictionary<string, string> lang_dic)
        {
            InitializeComponent();
            saveFrame.Visible = false;
            loadFrame.Visible = false;
            Action_groupBox.Enabled = false;
            Hint_groupBox.Enabled = false;
            Motion_groupBox.Enabled = false;
            Setting_groupBox.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            CheckForIllegalCrossThreadCalls = false;// dangerous
            accLXText.Name = "0";
            accHXText.Name = "1";
            accLYText.Name = "2";
            accHYText.Name = "3";
            accLZText.Name = "4";
            accHZText.Name = "5";
            accDurationText.Text = "6";
            Main_lang_dic = lang_dic;
            applyLang();
            init_load_file = filename;
            Application.Idle += new EventHandler(init_load);
        }

        private void init_load(object sender, EventArgs e)
        {
            if (String.Compare(init_load_file, "") != 0 && File.Exists(init_load_file))
            {
                load_project(init_load_file);
                Application.Idle -= new EventHandler(init_load);
            }
        }

        private void Update_framelist()  //set framelist
        {
            if (ME_Motionlist.Count == 0)
                return;
            Framelist.Controls.Clear();

            int count = 0;
            int picmode_count = 1;
            if (picmode_move == true)
                Motion.newflag = false;
            for (int i = 0; i < 45; i++)
            {
                if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                {
                    fcheck[i] = new CheckBox();
                    fonoff[i] = new CheckBox();
                    fpanel[i] = new Panel();
                    flabel[i] = new Label();
                    ftext[i] = new MaskedTextBox();
                    fbar[i] = new HScrollBar();
                    fpanel[i].Size = new Size(267, 30);
                    fpanel[i].BackColor = Color.Transparent;
                    fpanel[i].BorderStyle = BorderStyle.FixedSingle;
                    if (Motion.picfilename == null || Motion.newflag == true)
                    {
                        fpanel[i].Top = count * 30;
                        Motion.channely[i] = count * 30;
                    }
                    else
                    {
                        if (Motion.channely[i] == 0 && count != 0)
                        {
                            fpanel[i].Top = picmode_count * 30;
                            picmode_count++;
                        }
                        else
                        {
                            fpanel[i].Top = Motion.channely[i];
                        }
                        fpanel[i].Left = Motion.channelx[i];
                    }
                    fcheck[i].Size = new Size(15, 15);
                    fcheck[i].Top += 3;
                    fcheck[i].Left += 5;
                    fcheck[i].Checked = ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).used_servos.Contains(i);
                    fcheck[i].CheckedChanged += new EventHandler(used_CheckedChanged);
                    fcheck[i].Name = i.ToString();
                    if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).moton_layer == 0)
                        fcheck[i].Visible = false;
                    flabel[i].Size = new Size(40, 18);
                    flabel[i].BackColor = Color.White;
                    flabel[i].Top += 3;
                    flabel[i].Left += 20;
                    ftext[i].Size = new Size(40, 22);
                    ftext[i].Left += 60;
                    ftext[i].TextAlign = HorizontalAlignment.Right;

                    flabel[i].Name = i.ToString();
                    ftext[i].Name = i.ToString();

                    if (Motion.picfilename != null)
                    {
                        flabel[i].MouseDown += new MouseEventHandler(flMouseDown);
                        flabel[i].MouseMove += new MouseEventHandler(flMouseMove);
                        flabel[i].MouseUp += new MouseEventHandler(flMouseUp);
                        using(var memoryStream = new MemoryStream(Properties.Resources.hand_open))
                        {
                            flabel[i].Cursor = new Cursor(memoryStream);
                        }
                    }
                    ftext[i].KeyPress += new KeyPressEventHandler(numbercheck);

                    fbar[i].Size = new Size(135, 22);
                    fbar[i].Left += 105;
                    fbar[i].Maximum = (int)(Max[i] + 9);
                    fbar[i].Minimum = (int)min[i];
                    fbar[i].Name = i.ToString();
                    fbar[i].Scroll += new ScrollEventHandler(scroll_event);

                    fonoff[i].Appearance = Appearance.Button;
                    fonoff[i].FlatStyle = FlatStyle.Flat;
                    fonoff[i].FlatAppearance.BorderSize = 0;
                    fonoff[i].Image = Properties.Resources.on;
                    fonoff[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    fonoff[i].Size = new Size(23, 23);
                    fonoff[i].Left += 241;
                    fonoff[i].Name = i.ToString();
                    fonoff[i].CheckedChanged += new EventHandler(onOff_CheckedChanged);
                    if ((servo_onOff & (1UL << i)) != 0)
                        fonoff[i].Checked = false;
                    else
                        fonoff[i].Checked = true;

                    if (Motionlist.SelectedItem != null)
                    {
                        string[] datas = Motionlist.SelectedItem.ToString().Split(' ');
                        if (String.Compare(datas[0], "[Frame]") == 0)
                        {
                            ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                            ME_Frame f =(ME_Frame)m.Events[Motionlist.SelectedIndex];
                            ftext[i].Text = f.frame[i].ToString();
                            if (int.Parse(ftext[i].Text) <= Max[i] && int.Parse(ftext[i].Text) >= min[i])
                                fbar[i].Value = int.Parse(ftext[i].Text);
                            else if(String.Compare( ftext[i].Text, "0") == 0 )
                            {
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] = (int)homeframe[i];
                                ftext[i].Text = homeframe[i].ToString();
                                fbar[i].Value = (int)homeframe[i];
                            }
                        }
                        else if (String.Compare(datas[0], "[Home]") == 0)
                        {
                            ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                            ME_Frame f = (ME_Frame)m.Events[Motionlist.SelectedIndex];
                            ftext[i].Text = homeframe[i].ToString();
                            fbar[i].Value = (int)homeframe[i];
                        }
                    }

                    ftext[i].TextChanged += new EventHandler(Text_Changed);

                    if (i < 10)
                        flabel[i].Text = "CH " + i.ToString() + ":";
                    else
                        flabel[i].Text = "CH" + i.ToString() + ":";

                    if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).moton_layer != 0 && fcheck[i].Checked == false)
                    {
                        ftext[i].Enabled = false;
                        fbar[i].Enabled = false;
                    }

                    ttp.SetToolTip(fonoff[i], Main_lang_dic["fonoff_ToolTip"]);
                    ttp.SetToolTip(fcheck[i], Main_lang_dic["fcheck_ToolTip"]);
                    fpanel[i].Controls.Add(fcheck[i]);
                    fpanel[i].Controls.Add(fonoff[i]);
                    fpanel[i].Controls.Add(flabel[i]);
                    fpanel[i].Controls.Add(ftext[i]);
                    fpanel[i].Controls.Add(fbar[i]);
                    Framelist.Controls.Add(fpanel[i]);
                    
                    count++;
                }
            }
        }

        public void onOff_CheckedChanged(object sender, EventArgs e) //sender -> fonoff[i]
        {
            ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
            int index = int.Parse(((CheckBox)sender).Name);
            if (((CheckBox)sender).Checked == true)
            {
                ((CheckBox)sender).Image = Properties.Resources.off;
                servo_onOff &= ~(1UL << index);
                if (string.Compare(com_port, "OFF") != 0)
                {
                    try
                    {
                        arduino.motor_release(index);
                    }
                    catch
                    {
                        com_port = "OFF";
                        MessageBox.Show(Main_lang_dic["errorMsg1"]);
                    }
                }
            }
            else if (((CheckBox)sender).Checked == false)
            {
                ((CheckBox)sender).Image = Properties.Resources.on;
                servo_onOff |= 1UL << index;
                if (autocheck.Checked == true)
                {
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        {
                            autoframe[i] = (int.Parse(ftext[i].Text) + offset[i]);
                            if (m.moton_layer != 0 && !m.used_servos.Contains(i))
                                autoframe[i] = 0;
                        }
                        else
                            autoframe[i] = 0;
                    }
                    if (string.Compare(com_port, "OFF") != 0)
                    {
                        try
                        {
                            arduino.frameWrite(0x6F, autoframe, 0, servo_onOff);
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show(Main_lang_dic["errorMsg1"]);
                        }
                    }
                }
            }
        }

        public void used_CheckedChanged(object sender, EventArgs e) //sender -> fcheck[i]
        {
            int index = int.Parse(((CheckBox)sender).Name);
            ME_Motion m = ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]);
            if (((CheckBox)sender).Checked == true)
            {
                m.used_servos.Add(index);
                ftext[index].Enabled = true;
                fbar[index].Enabled = true;
            }
            else if (((CheckBox)sender).Checked == false)
            {
                m.used_servos.Remove(index);
                ftext[index].Enabled = false;
                fbar[index].Enabled = false;
            }
        }

        public void flMouseDown(object sender, MouseEventArgs e)
        {
            using (var memoryStream = new MemoryStream(Properties.Resources.hand_close))
            {
                ((Label)sender).Cursor = new Cursor(memoryStream);
            }
            mdx = e.X;
            mdy = e.Y;
        }

        public void flMouseUp(object sender, MouseEventArgs e)
        {
            using (var memoryStream = new MemoryStream(Properties.Resources.hand_open))
            {
                ((Label)sender).Cursor = new Cursor(memoryStream);
            }
        }

        public void flMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y - mdy + 30 < 550)
                    fpanel[int.Parse(((Label)sender).Name)].Top += e.Y - mdy;
                if (e.X - mdx + 260 < 700)
                    fpanel[int.Parse(((Label)sender).Name)].Left += e.X-mdx;
                Motion.channely[int.Parse(((Label)sender).Name)] = fpanel[int.Parse(((Label)sender).Name)].Top;
                Motion.channelx[int.Parse(((Label)sender).Name)] = fpanel[int.Parse(((Label)sender).Name)].Left;
                picmode_move = true;
            }
        }

        public void scroll_event(object sender, ScrollEventArgs e) //Scroll event
        {
            this.ftext[int.Parse(((HScrollBar)sender).Name)].Text = ((HScrollBar)sender).Value.ToString();            
        }

        public void SyncSpeed(object sender, EventArgs e)
        {
            if (string.Compare(com_port, "OFF") != 0)
            {
                try
                {
                    if (sync_speed.Value == 5)
                        arduino.setSyncSpeed(0);
                    else
                        arduino.setSyncSpeed(400 + sync_speed.Value * 400);
                }
                catch
                {
                    com_port = "OFF";
                    MessageBox.Show(Main_lang_dic["errorMsg1"]);
                }
            }
        }

        public void loops_TextChanged(object sender, EventArgs e)
        {
            ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).loops = ((MaskedTextBox)sender).Text;
            if (String.Compare(((MaskedTextBox)sender).Text, "") == 0)
            {
                ((MaskedTextBox)sender).Text = "0";
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).loops = "0";
            }
            else if (int.Parse(((MaskedTextBox)sender).Text) > 10000000)
                ((MaskedTextBox)sender).Text = "10000000";
            int loop_num = int.Parse(((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).loops);
            ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).current_loop = loop_num;
        }

        public void Text_Changed(object sender, EventArgs e) //Text event
        {
            int n;
            if ((((MaskedTextBox)sender).Text) == "")
            {
                (((MaskedTextBox)sender).Text) = "0";
            }
            else if (int.Parse(((MaskedTextBox)sender).Text) <= Max[int.Parse(((MaskedTextBox)sender).Name)] && int.Parse(((MaskedTextBox)sender).Text) >= min[int.Parse(((MaskedTextBox)sender).Name)])
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                this.fbar[int.Parse(((MaskedTextBox)sender).Name)].Value = int.Parse(((MaskedTextBox)sender).Text);
                if (autocheck.Checked == true)
                {
                    if (!freshflag[0])
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                autoframe[i] = (int.Parse(ftext[i].Text) + offset[i]);
                                if (m.moton_layer != 0 && !m.used_servos.Contains(i))
                                    autoframe[i] = 0;
                            }
                            else
                                autoframe[i] = 0;
                        }
                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, 0, servo_onOff);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show(Main_lang_dic["errorMsg1"]);
                            }
                        }
                    }
                }
            }

            if (int.TryParse(((MaskedTextBox)sender).Text,out n))
            {
                if (Motionlist.SelectedItem != null)
                {
                    string[] datas = Motionlist.SelectedItem.ToString().Split(' ');
                    if (String.Compare(datas[0], "[Frame]") == 0)
                    {
                        ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                        ME_Frame f = (ME_Frame)m.Events[Motionlist.SelectedIndex];
                        f.frame[int.Parse(((MaskedTextBox)sender).Name)] = int.Parse(((MaskedTextBox)sender).Text);
                    }
                }
            }
            else
                ((MaskedTextBox)sender).Text = "";
        }

        public void numbercheck(object sender, KeyPressEventArgs e) //Text number check
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
            {
                e.Handled = true;
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

        private void accXYZText_Changed(object sender, EventArgs e)
        {
            double output;
            if (double.TryParse(((MaskedTextBox)sender).Text, out output))
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).acc_Settings[int.Parse(((MaskedTextBox)sender).Name)] = output;
            else if (((MaskedTextBox)sender).Text == "-" || ((MaskedTextBox)sender).Text == "" ||
                     ((MaskedTextBox)sender).Text == "-." || ((MaskedTextBox)sender).Text == ".")
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).acc_Settings[int.Parse(((MaskedTextBox)sender).Name)] = 0;
            else
            {
                MessageBox.Show(Main_lang_dic["errorMsg19"]);
                ((MaskedTextBox)sender).SelectAll();
            }
        }

        private void accDurationText_Changed(object sender, EventArgs e)
        {
            int output;
            if (ME_Motionlist != null)
            {
                if (int.TryParse(((MaskedTextBox)sender).Text, out output))
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).acc_Settings[6] = output;
                else
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).acc_Settings[6] = 0;
            }
        }

        public void analogValueText_Changed(object sender, EventArgs e)
        {
            int output;
            if (ME_Motionlist != null)
            {
                if (int.TryParse(((MaskedTextBox)sender).Text, out output))
                {
                    if (output >= 1024)
                    {
                        output = 1023;
                        ((MaskedTextBox)sender).Text = "1023";
                        ((MaskedTextBox)sender).SelectionStart = ((MaskedTextBox)sender).Text.Length;
                    }
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).analog_value = output;
                }
                else
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).analog_value = 0;
            }
        }

        private bool needToSave()
        {
            bool need_to_save = false;
            if (File.Exists(load_filename) && Motion != null)
            {
                string tmp_file = DateTime.Now.ToString("yyyyMMddhhmmss") + "_86ME_tmpGeneratedFile.rbm";
                save_project(tmp_file);
                StreamReader checker = new StreamReader(tmp_file);
                StreamReader prev = new StreamReader(load_filename);
                string[] datas_c = checker.ReadToEnd().Split(delimiterChars);
                string[] datas_p = prev.ReadToEnd().Split(delimiterChars);
                if (datas_c.Length == datas_p.Length)
                {
                    for (int i = 0; i < datas_c.Length; i++)
                    {
                        if (String.Compare(datas_c[i], datas_p[i]) != 0)
                        {
                            need_to_save = true;
                            break;
                        }
                    }
                }
                else
                    need_to_save = true;
                checker.Dispose();
                checker.Close();
                prev.Dispose();
                prev.Close();
                File.Delete(tmp_file);
            }
            else if(Motion != null)
                return true;
            return need_to_save;
        }

        private void initAnalog()
        {
            if (board_ver86 == 0) //one
            {
                analogPinCombo.Items.Clear();
                for (int i = 0; i < 7; i++)
                    analogPinCombo.Items.Add("A" + i.ToString());
            }
            else if (board_ver86 == 1) //zero
            {
                analogPinCombo.Items.Clear();
                for (int i = 0; i < 6; i++)
                    analogPinCombo.Items.Add("A" + i.ToString());
            }
            else if (board_ver86 == 2) //edu
            {
                analogPinCombo.Items.Clear();
                for (int i = 0; i < 6; i++)
                    analogPinCombo.Items.Add("A" + i.ToString());
            }
            else if (board_ver86 == 3) //ai
            {
                analogPinCombo.Items.Clear();
                for (int i = 0; i < 2; i++)
                    analogPinCombo.Items.Add("A" + i.ToString());
            }
        }

        private void clearPs2()
        {
            ps2DATCombo.Items.Clear();
            ps2CMDCombo.Items.Clear();
            ps2ATTCombo.Items.Clear();
            ps2CLKCombo.Items.Clear();
        }

        private void createPs2(int i)
        {
            ps2DATCombo.Items.Add(i.ToString());
            ps2CMDCombo.Items.Add(i.ToString());
            ps2ATTCombo.Items.Add(i.ToString());
            ps2CLKCombo.Items.Add(i.ToString());
        }

        private void initPs2()
        {
            if (board_ver86 == 0) //one
            {
                clearPs2();
                for (int i = 0; i < 45; i++)
                    createPs2(i);
            }
            else if (board_ver86 == 1) //zero
            {
                clearPs2();
                for (int i = 0; i < 14; i++)
                    createPs2(i);
                for (int i = 42; i < 45; i++)
                    createPs2(i);
            }
            else if (board_ver86 == 2) //edu
            {
                clearPs2();
                for (int i = 0; i < 21; i++)
                    createPs2(i);
                for (int i = 31; i < 33; i++)
                    createPs2(i);
                for (int i = 42; i < 45; i++)
                    createPs2(i);
            }
            else if (board_ver86 == 3) //ai
            {
                clearPs2();
                for (int i = 0; i < 36; i++)
                    createPs2(i);
            }
        }

        private void update_newMotionParams(NewMotion nMotion)
        {
            board_ver86 = nMotion.comboBox1.SelectedIndex;
            used_imu = nMotion.comboBox2.SelectedIndex;
            if (nMotion.maskedTextBox1.Text == "" || nMotion.maskedTextBox1.Text == "." ||
                nMotion.maskedTextBox1.Text == "-." || nMotion.maskedTextBox1.Text == "-")
                nMotion.maskedTextBox1.Text = "0";
            if (nMotion.maskedTextBox2.Text == "" || nMotion.maskedTextBox2.Text == "." ||
                nMotion.maskedTextBox2.Text == "-." || nMotion.maskedTextBox2.Text == "-")
                nMotion.maskedTextBox2.Text = "0";
            if (nMotion.maskedTextBox3.Text == "" || nMotion.maskedTextBox3.Text == "." ||
                nMotion.maskedTextBox3.Text == "-." || nMotion.maskedTextBox3.Text == "-")
                nMotion.maskedTextBox3.Text = "0";
            if (nMotion.maskedTextBox4.Text == "" || nMotion.maskedTextBox4.Text == "." ||
                nMotion.maskedTextBox4.Text == "-." || nMotion.maskedTextBox4.Text == "-")
                nMotion.maskedTextBox4.Text = "0";
            init_quaternion[0] = double.Parse(nMotion.maskedTextBox1.Text);
            init_quaternion[1] = double.Parse(nMotion.maskedTextBox2.Text);
            init_quaternion[2] = double.Parse(nMotion.maskedTextBox3.Text);
            init_quaternion[3] = double.Parse(nMotion.maskedTextBox4.Text);
            for (int i = 0; i < 45; i++)
            {
                if (nMotion.ftext[i].Text == "")
                    nMotion.ftext[i].Text = "0";
                if (nMotion.ftext2[i].Text == "")
                    nMotion.ftext2[i].Text = "1500";
                if (nMotion.ftext3[i].Text == "")
                    nMotion.ftext3[i].Text = "600";
                if (nMotion.ftext4[i].Text == "")
                    nMotion.ftext4[i].Text = "2400";
                if (nMotion.ftext5[i].Text == "" || nMotion.ftext5[i].Text == "." ||
                    nMotion.ftext5[i].Text == "-." || nMotion.ftext5[i].Text == "-")
                    nMotion.ftext5[i].Text = "0";
                motor_info[i] = nMotion.fbox[i].SelectedIndex;
                homeframe[i] = uint.Parse(nMotion.ftext2[i].Text);
                min[i] = uint.Parse(nMotion.ftext3[i].Text);
                Max[i] = uint.Parse(nMotion.ftext4[i].Text);
                p_gain[i] = double.Parse(nMotion.ftext5[i].Text);
                gain_source[i] = nMotion.fbox2[i].SelectedIndex;
                s_gain[i] = double.Parse(nMotion.ftext6[i].Text);
                gain_source2[i] = nMotion.fbox3[i].SelectedIndex;
                if (homeframe[i] > Max[i] || homeframe[i] < min[i])
                {
                    homeframe[i] = 1500;
                    nMotion.ftext2[i].Text = "1500";
                }
                try
                {
                    offset[i] = int.Parse(nMotion.ftext[i].Text);
                    if (offset[i] > offset_Max || offset[i] < offset_min)
                    {
                        offset[i] = 0;
                        nMotion.ftext[i].Text = "0";
                    }
                }
                catch
                {
                    offset[i] = 0;
                    nMotion.ftext[i].Text = "0";
                    string error_msg = "The offset " + i.ToString() + " is illegal, set to 0";
                    MessageBox.Show(error_msg);
                }
            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e) //new project
        {
            if (needToSave() && File.Exists(load_filename))
            {
                DialogResult dialogResult = MessageBox.Show(Main_lang_dic["saveMsg"], "", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                    save_project(load_filename);
                else if (dialogResult == DialogResult.Cancel)
                    return ;
            }
            else if (needToSave())
            {
                DialogResult dialogResult = MessageBox.Show(Main_lang_dic["saveMsg2"], "", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                    saveFileToolStripMenuItem_Click(sender, e);
                else if (dialogResult == DialogResult.Cancel)
                    return ;
            }

            NewMotion nMotion = new NewMotion(Main_lang_dic);
            if (string.Compare(com_port, "OFF") != 0)
            {
                nMotion.arduino = arduino;
                nMotion.start_synchronizer();
            }
            nMotion.ShowDialog();
            if (nMotion.DialogResult == DialogResult.OK)
            {
                load_filename = "";
                Hint_groupBox.Enabled = true;
                Motion_groupBox.Enabled = true;
                saveFileToolStripMenuItem.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                ME_Motionlist = new ArrayList();
                MotionCombo.Items.Clear();
                MotionCombo.Text = "";
                Motionlist.Items.Clear();
                Framelist.Controls.Clear();
                delaytext.Text = default_delay.ToString();
                delaytext.Enabled = false;
                current_motionlist_idx = -1;
                last_motionlist_idx = -1;
                servo_onOff = ~0UL;
                autocheck.Checked = false;

                update_newMotionParams(nMotion);

                initAnalog();
                initPs2();
                gs.ps2pins[0] = "0";
                gs.ps2pins[1] = "0";
                gs.ps2pins[2] = "0";
                gs.ps2pins[3] = "0";

                Motion = nMotion;
                if(Robot_pictureBox.Image != null)
                    Robot_pictureBox.Image = null;
                if (nMotion.picfilename != null)
                {
                    picture_name = nMotion.picfilename;
                    draw_background();
                }
                this.MotionConfig.SelectedIndex = 0;
                this.hint_richTextBox.Text = Main_lang_dic["hint1"];
                this.MotionConfig.Enabled = false;
            }
        }

        public void connect_comport()
        {
            if (arduino == null && (string.Compare(com_port, "OFF") != 0))
            {
                try
                {
                    arduino = new Arduino(com_port);
                    arduino.setSyncSpeed(0);
                }
                catch
                {
                    com_port = "OFF";
                    MessageBox.Show(Main_lang_dic["errorMsg2"], "",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)  //option
        {
            Motion.NewMotion_lang_dic = Main_lang_dic;
            Motion.applyLang();
            for (int i = 0; i < 45; i++)
                Motion.fcheck[i].Checked = false;
            autocheck.Checked = false;
            if (string.Compare(com_port, "OFF") != 0)
            {
                Motion.arduino = arduino;
                Motion.start_synchronizer();
            }
            Motion.ShowDialog();
            if (Motion.DialogResult == DialogResult.OK)
            {
                if (Motion.picfilename != null)
                {
                    picture_name = Motion.picfilename;
                    try
                    {
                        Robot_pictureBox.Image = Image.FromFile(Motion.picfilename);
                    }
                    catch
                    {
                        Motion.picfilename = null;
                        MessageBox.Show(Main_lang_dic["errorMsg3"]);
                    }
                }

                if (board_ver86 != Motion.comboBox1.SelectedIndex)
                    change_board = true;

                update_newMotionParams(Motion);
                last_motionlist_idx = -1;
                current_motionlist_idx = -1;
                freshflag[1] = false;
                update_motionlist();
                draw_background();
            }
            else if (Motion.DialogResult == DialogResult.Cancel)
            {
                if (board_ver86 != Motion.comboBox1.SelectedIndex)
                {
                    Motion.clear_Channels();
                    if (board_ver86 == 0)
                        Motion.create_panel(0, 45, 0);
                    else if (board_ver86 == 1)
                    {
                        Motion.create_panel(0, 14, 0);
                        Motion.create_panel(42, 45, 14);
                    }
                    else if (board_ver86 == 2)
                    {
                        Motion.create_panel(0, 21, 0);
                        Motion.create_panel(31, 33, 21);
                        Motion.create_panel(42, 45, 23);
                    }
                    else if (board_ver86 == 3)
                    {
                        Motion.create_panel(0, 26, 0);
                        Motion.create_panel(34, 36, 26);
                    }
                }
                if (used_imu != Motion.comboBox2.SelectedIndex)
                {
                    Motion.SetIMUUI(used_imu);
                }
                Motion.picfilename = picture_name;
                Motion.comboBox1.SelectedIndex = board_ver86;
                Motion.comboBox2.SelectedIndex = used_imu;
                Motion.maskedTextBox1.Text = init_quaternion[0].ToString();
                Motion.maskedTextBox2.Text = init_quaternion[1].ToString();
                Motion.maskedTextBox3.Text = init_quaternion[2].ToString();
                Motion.maskedTextBox4.Text = init_quaternion[3].ToString();
                for (int i = 0; i < 45; i++)
                {
                    Motion.fbox[i].SelectedIndex = motor_info[i];
                    Motion.fbox2[i].SelectedIndex = gain_source[i];
                    Motion.fbox3[i].SelectedIndex = gain_source2[i];
                    Motion.ftext[i].Text = offset[i].ToString();
                    Motion.ftext2[i].Text = homeframe[i].ToString();
                    Motion.ftext3[i].Text = min[i].ToString();
                    Motion.ftext4[i].Text = Max[i].ToString();
                    Motion.ftext5[i].Text = p_gain[i].ToString();
                    Motion.ftext6[i].Text = s_gain[i].ToString();
                }
            }

            MotionConfig.SelectedIndex = 0;
            if (ME_Motionlist.Count > 0)
                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count > 0)
                    Motionlist.SelectedIndex = 0;
            initAnalog();
            initPs2();
            if (change_board)
            {
                gs.ps2pins[0] = "0";
                gs.ps2pins[1] = "0";
                gs.ps2pins[2] = "0";
                gs.ps2pins[3] = "0";
                change_board = false;
            }
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)    //save project
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "rbm files (*.rbm)|*.rbm";
            dialog.Title = "Save File";
            dialog.FileName = Path.GetFileName(load_filename);
            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != null)
            {
                load_filename = dialog.FileName.ToString();
                save_project(load_filename);
            }
        }

        private void save_project(string filename)
        {
            TextWriter writer = new StreamWriter(filename);

            writer.Write("BoardVer ");
            writer.Write(Motion.comboBox1.SelectedItem.ToString());
            writer.Write(" " + gs.ps2pins[0] + " " + gs.ps2pins[1] + " " + gs.ps2pins[2] + " " + gs.ps2pins[3] + " " +
                         gs.bt_baud + " " + gs.bt_port + " " + gs.wifi602_port);
            writer.Write("\n");
            writer.Write("Servo ");
            for (int i = 0; i < 45; i++)
            {
                ComboBox cb = Motion.fbox[i];
                writer.Write(cb.Text);
                if (i != 44)
                    writer.Write(" ");
            }
            writer.Write("\n");
            writer.Write("Offset ");
            for (int j = 0; j < 45; j++)
            {
                if (string.Compare(Motion.ftext[j].Text, "") == 0)
                    Motion.ftext[j].Text = "0";
                writer.Write(Motion.ftext[j].Text + " ");
            }
            writer.Write("\n");
            writer.Write("Homeframe ");
            for (int j = 0; j < 45; j++)
            {
                if (string.Compare(Motion.ftext[j].Text, "") == 0)
                    Motion.ftext2[j].Text = "1500";
                writer.Write(Motion.ftext2[j].Text + " ");
            }
            writer.Write("\n");
            writer.Write("Range ");
            for (int j = 0; j < 45; j++)
            {
                if (string.Compare(Motion.ftext3[j].Text, "") == 0)
                    Motion.ftext3[j].Text = "600";
                writer.Write(Motion.ftext3[j].Text + " ");
            }
            for (int j = 0; j < 45; j++)
            {
                if (string.Compare(Motion.ftext4[j].Text, "") == 0)
                    Motion.ftext4[j].Text = "2400";
                writer.Write(Motion.ftext4[j].Text + " ");
            }
            writer.Write("\n");
            if (Motion.picfilename != null)
            {
                writer.Write("picmode ");
                writer.Write(Motion.picfilename + " ");
                for (int i = 0; i < 45; i++)
                    writer.Write(Motion.channelx[i] + " ");
                for (int i = 0; i < 45; i++)
                    writer.Write(Motion.channely[i] + " ");
                writer.Write("\n");
            }
            // save sync_speed
            writer.WriteLine("Sync " + sync_speed.Value.ToString());
            // save IMU
            writer.WriteLine("IMU " + Motion.comboBox2.SelectedIndex + " " + Motion.q.w + " " + Motion.q.x +
                             " " + Motion.q.y + " " + Motion.q.z);
            writer.Write("PGain ");
            for (int j = 0; j < 45; j++)
            {
                if (string.Compare(Motion.ftext5[j].Text, "") == 0)
                    Motion.ftext5[j].Text = "0";
                writer.Write(Motion.ftext5[j].Text + " ");
            }
            writer.WriteLine();
            writer.Write("SGain ");
            for (int j = 0; j < 45; j++)
            {
                if (string.Compare(Motion.ftext6[j].Text, "") == 0)
                    Motion.ftext6[j].Text = "0";
                writer.Write(Motion.ftext6[j].Text + " ");
            }
            writer.WriteLine();
            writer.Write("Source ");
            for (int j = 0; j < 45; j++)
                writer.Write(Motion.fbox2[j].Text + " ");
            writer.WriteLine();
            writer.Write("Source2 ");
            for (int j = 0; j < 45; j++)
                writer.Write(Motion.fbox3[j].Text + " ");
            writer.WriteLine();
            for (int i = 0; i < ME_Motionlist.Count; i++) // save existing motions 
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                saveMotion(m, writer);
                if (i != ME_Motionlist.Count - 1)
                    writer.Write("\n");
            }

            writer.Dispose();
            writer.Close();
        }

        private void saveMotion(ME_Motion m, TextWriter writer)
        {
            string bt_key = (m.bt_key == "" ? "---noBtKey---" : m.bt_key);
            writer.Write("Motion " + m.name + " " + m.trigger_method + " " + m.auto_method + " " +
                         m.trigger_key + " " + m.trigger_keyType + " " + bt_key + " " + m.ps2_key +
                         " " + m.ps2_type + " " + m.bt_mode + " " + m.acc_Settings[0] + " " + m.acc_Settings[1] +
                         " " + m.acc_Settings[2] + " " + m.acc_Settings[3] + " " + m.acc_Settings[4] +
                         " " + m.acc_Settings[5] + " " + m.acc_Settings[6] + " " + m.wifi602_key +
                         " " + m.analog_pin + " " + m.analog_cond + " " + m.analog_value + "\n");
            writer.Write("Layer " + m.moton_layer + " ");
            for (int j = 0; j < m.used_servos.Count; j++)
            {
                writer.Write(m.used_servos[j]);
                if (j != m.used_servos.Count - 1)
                    writer.Write(" ");
            }
            writer.WriteLine();
            for (int j = 0; j < m.Events.Count; j++)
            {
                if (m.Events[j] is ME_Frame)
                {
                    ME_Frame f = (ME_Frame)m.Events[j];
                    if (f.type == 1)
                        writer.Write("frame " + f.delay.ToString() + " ");
                    else if (f.type == 0)
                        writer.Write("home " + f.delay.ToString() + " ");
                    int count = 0;
                    for (int k = 0; k < 45; k++)
                    {
                        if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                        {
                            count++;
                        }
                    }
                    for (int k = 0; k < 45; k++)
                    {
                        if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                        {
                            count--;
                            writer.Write(f.frame[k].ToString());
                            if (count != 0)
                                writer.Write(" ");
                        }
                    }
                    writer.Write("\n");
                }
                else if (m.Events[j] is ME_Delay)
                {
                    ME_Delay d = (ME_Delay)m.Events[j];
                    writer.Write("delay " + d.delay.ToString() + "\n");
                }
                else if (m.Events[j] is ME_Goto)
                {
                    ME_Goto g = (ME_Goto)m.Events[j];
                    writer.Write("goto " + g.name + " " + g.is_goto.ToString() + " " + g.loops + " " + g.infinite + "\n");
                }
                else if (m.Events[j] is ME_Flag)
                {
                    ME_Flag fl = (ME_Flag)m.Events[j];
                    writer.Write("flag " + fl.name + "\n");
                }
                else if (m.Events[j] is ME_Trigger)
                {
                    ME_Trigger t = (ME_Trigger)m.Events[j];
                    writer.Write("trigger " + t.name + " " + t.method + "\n");
                }
                else if (m.Events[j] is ME_Release)
                {
                    writer.Write("release\n");
                }
                else if (m.Events[j] is ME_Compute)
                {
                    ME_Compute op = (ME_Compute)m.Events[j];
                    writer.Write("compute " + op.left_var + " " + op.form + " " + op.f1_var1 + " " + op.f1_op + " " + op.f1_var2 +
                                 " " + op.f2_op + " " + op.f2_var + " " + op.f3_var + " " + op.f4_const + "\n");
                }
                else if (m.Events[j] is ME_If)
                {
                    ME_If mif = (ME_If)m.Events[j];
                    writer.Write("if " + mif.left_var + " " + mif.method + " " + mif.right_var + " " + mif.name + "\n");
                }
            }
            writer.Write("MotionEnd " + m.property + " " + m.comp_range + " " + m.control_method + " " + m.name);
        }

        private void actionToolStripMenuItem_Click(object sender, EventArgs e)//load project
        {
            bool _needToSave = needToSave();
            if (_needToSave && File.Exists(load_filename))
            {
                DialogResult dialogResult = MessageBox.Show(Main_lang_dic["saveMsg"], "", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                    save_project(load_filename);
                else if (dialogResult == DialogResult.Cancel)
                    return ;
            }
            else if (_needToSave)
            {
                DialogResult dialogResult = MessageBox.Show(Main_lang_dic["saveMsg2"], "", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                    saveFileToolStripMenuItem_Click(sender, e);
                else if (dialogResult == DialogResult.Cancel)
                    return ;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "rbm files (*.rbm)|*.rbm";
            dialog.Title = "Open File";
            String filename = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : null;
            if (filename == null)
                return;
            if( String.Compare(Path.GetExtension(filename), ".rbm") != 0 )
            {
                MessageBox.Show(Main_lang_dic["errorMsg4"]);
                return;
            }
            current_motionlist_idx = -1;
            last_motionlist_idx = -1;
            servo_onOff = ~0UL;
            autocheck.Checked = false;
            load_project(filename);
            MotionConfig.SelectedIndex = 0;
            if (ME_Motionlist.Count > 0)
                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count > 0)
                    Motionlist.SelectedIndex = 0;
        }

        private void load_project(string filename)
        {
            picture_name = null;
            bool picmode = false;
            NewMotion nMotion = new NewMotion(Main_lang_dic);
            string[] rbver = new string[] { "---unset---",
                                            "RB_100b1",
                                            "RB_100b2",
                                            "RB_100b3",
                                            "RB_100",
                                            "RB_100RD",
                                            "RB_110",
                                            "86Duino_One",
                                            "86Duino_Zero",
                                            "86Duino_EduCake",
                                            "86Duino_AI",
                                            "unknow"};
            string[] servo = new string[] { "---noServo---",
                                            "EMAX_ES08AII",
                                            "EMAX_ES3104",
                                            "KONDO_KRS786",
                                            "KONDO_KRS788",
                                            "KONDO_KRS78X",
                                            "KONDO_KRS4014",
                                            "KONDO_KRS4024",
                                            "HITEC_HSR8498",
                                            "FUTABA_S3003",
                                            "SHAYYE_SYS214050",
                                            "TOWERPRO_MG90S",
                                            "TOWERPRO_MG995",
                                            "TOWERPRO_MG996",
                                            "TOWERPRO_SG90",
                                            "DMP_RS0263",
                                            "DMP_RS1270",
                                            "GWS_S777",
                                            "GWS_S03T",
                                            "GWS_MICRO",
                                            "OtherServos"};

            using (StreamReader reader = new StreamReader(filename))
            {
                load_filename = filename;

                string[] datas = reader.ReadToEnd().Split(delimiterChars);
                if (datas.Length < 239)
                {
                    MessageBox.Show(Main_lang_dic["errorMsg5"]);
                    return;
                }
                if (datas[0] != "BoardVer")
                {
                    MessageBox.Show(Main_lang_dic["errorMsg5"]);
                    return;
                }

                ME_Motionlist = new ArrayList();
                ME_Motion motiontag = null;
                MotionCombo.Items.Clear();
                MotionCombo.Text = "";
                Motionlist.Items.Clear();
                delaytext.Text = default_delay.ToString();

                for (int i = 0; i < datas.Length; i++)
                {
                    if (String.Compare(datas[i], "BoardVer") == 0)
                    {
                        i++;
                        for (int j = 0; j < rbver.Length; j++)
                        {
                            if (String.Compare(datas[i], rbver[j]) == 0)
                            {
                                //***fix bug after remove rb
                                nMotion.comboBox1.SelectedIndex = j - 7;
                                board_ver86 = j - 7;
                                if (string.Compare(rbver[j], "86Duino_Zero") == 0)
                                {
                                    nMotion.clear_Channels();
                                    nMotion.create_panel(0, 14, 0);
                                    nMotion.create_panel(42, 45, 14);
                                }
                                else if (string.Compare(rbver[j], "86Duino_EduCake") == 0)
                                {
                                    nMotion.clear_Channels();
                                    nMotion.create_panel(0, 21, 0);
                                    nMotion.create_panel(31, 33, 21);
                                    nMotion.create_panel(42, 45, 23);
                                }
                                else if (string.Compare(rbver[j], "86Duino_AI") == 0)
                                {
                                    nMotion.clear_Channels();
                                    nMotion.create_panel(0, 36, 0);
                                }
                            }
                        }
                        if (String.Compare(datas[i + 1], "Servo") != 0)
                        {
                            gs.ps2pins[0] = datas[++i];
                            gs.ps2pins[1] = datas[++i];
                            gs.ps2pins[2] = datas[++i];
                            gs.ps2pins[3] = datas[++i];
                        }
                        if (String.Compare(datas[i + 1], "Servo") != 0)
                        {
                            gs.bt_baud = datas[++i];
                            gs.bt_port = datas[++i];
                        }
                        if (String.Compare(datas[i + 1], "Servo") != 0)
                        {
                            gs.wifi602_port = datas[++i];
                        }
                    }
                    else if (String.Compare(datas[i], "Offset") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.ftext[k].Text = datas[i];
                            try
                            {
                                offset[k] = int.Parse(datas[i]);
                            }
                            catch
                            {
                                nMotion.ftext[k].Text = "0";
                                offset[k] = 0;
                                MessageBox.Show(Main_lang_dic["errorMsg6"]);
                            }
                        }
                    }
                    else if (String.Compare(datas[i], "Homeframe") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.ftext2[k].Text = datas[i];
                            try
                            {
                                homeframe[k] = uint.Parse(datas[i]);
                            }
                            catch
                            {
                                nMotion.ftext2[k].Text = "1500";
                                homeframe[k] = 1500;
                                MessageBox.Show(Main_lang_dic["errorMsg7"]);
                            }
                        }
                    }
                    else if (String.Compare(datas[i], "Range") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.ftext3[k].Text = datas[i];
                            try
                            {
                                min[k] = uint.Parse(datas[i]);
                            }
                            catch
                            {
                                nMotion.ftext3[k].Text = "600";
                                min[k] = 600;
                                MessageBox.Show(Main_lang_dic["errorMsg8"]);
                            }
                        }
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.ftext4[k].Text = datas[i];
                            try
                            {
                                Max[k] = uint.Parse(datas[i]);
                            }
                            catch
                            {
                                nMotion.ftext4[k].Text = "2400";
                                Max[k] = 2400;
                                MessageBox.Show(Main_lang_dic["errorMsg8"]);
                            }
                        }
                    }
                    else if (String.Compare(datas[i], "Sync") == 0)
                    {
                        sync_speed.Value = int.Parse(datas[++i]);
                    }
                    else if (String.Compare(datas[i], "IMU") == 0)
                    {
                        used_imu = int.Parse(datas[++i]);
                        if (used_imu != 0)
                        {
                            if (string.Compare(com_port, "OFF") != 0)
                                nMotion.init_imu.Enabled = true;
                            nMotion.maskedTextBox1.Enabled = true;
                            nMotion.maskedTextBox2.Enabled = true;
                            nMotion.maskedTextBox3.Enabled = true;
                            nMotion.maskedTextBox4.Enabled = true;
                            nMotion.label10.Enabled = true;
                            nMotion.label11.Enabled = true;
                            for (int k = 0; k < 45; k++)
                            {
                                nMotion.fbox2[k].Enabled = true;
                                nMotion.ftext5[k].Enabled = true;
                                nMotion.fbox3[k].Enabled = true;
                                nMotion.ftext6[k].Enabled = true;
                                nMotion.fcheck_ps[k].Enabled = true;
                            }
                        }
                        nMotion.comboBox2.SelectedIndex = used_imu;
                        nMotion.maskedTextBox1.Text = datas[++i];
                        nMotion.maskedTextBox2.Text = datas[++i];
                        nMotion.maskedTextBox3.Text = datas[++i];
                        nMotion.maskedTextBox4.Text = datas[++i];
                        init_quaternion[0] = double.Parse(nMotion.maskedTextBox1.Text);
                        init_quaternion[1] = double.Parse(nMotion.maskedTextBox2.Text);
                        init_quaternion[2] = double.Parse(nMotion.maskedTextBox3.Text);
                        init_quaternion[3] = double.Parse(nMotion.maskedTextBox4.Text);
                    }
                    else if (String.Compare(datas[i], "PGain") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.ftext5[k].Text = datas[i];
                            p_gain[k] = double.Parse(datas[i]);
                        }
                    }
                    else if (String.Compare(datas[i], "SGain") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.ftext6[k].Text = datas[i];
                            s_gain[k] = double.Parse(datas[i]);
                        }
                    }
                    else if (String.Compare(datas[i], "Source") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            nMotion.fbox2[k].Text = datas[++i];
                            gain_source[k] = nMotion.fbox2[k].SelectedIndex;
                        }
                    }
                    else if (String.Compare(datas[i], "Source2") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            nMotion.fbox3[k].Text = datas[++i];
                            gain_source2[k] = nMotion.fbox3[k].SelectedIndex;
                        }
                    }
                    else if (String.Compare(datas[i], "picmode") == 0)
                    {
                        picmode = true;
                        i++;
                        nMotion.picfilename = datas[i];
                        while (String.Compare(Path.GetExtension(datas[i]), "") == 0 || !File.Exists(nMotion.picfilename))
                        {
                            i++;
                            int value;
                            bool success = int.TryParse(datas[i], out value);
                            if (!success)
                                nMotion.picfilename += " " + datas[i];
                            else
                            {
                                i--;
                                break;
                            }
                        }
                        string short_picfilename = Path.GetFileName(nMotion.picfilename);
                        if (short_picfilename.Length < 25)
                            nMotion.pic_loaded.Text = short_picfilename;
                        else
                            nMotion.pic_loaded.Text = short_picfilename.Substring(0, 22) + "...";
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.channelx[k] = int.Parse(datas[i]);
                        }
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            nMotion.channely[k] = int.Parse(datas[i]);
                        }
                    }
                    else if (String.Compare(datas[i], "Servo") == 0)
                    {
                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            bool servo_fine = false;
                            for (int j = 0; j < servo.Length; j++)
                            {
                                if (String.Compare(datas[i], servo[j]) == 0)
                                {
                                    servo_fine = true;
                                    nMotion.fbox[k].SelectedIndex = j;
                                    motor_info[k] = j;
                                }
                            }
                            if (servo_fine == false)
                            {
                                nMotion.fbox[k].SelectedIndex = 0;
                                motor_info[k] = 0;
                                MessageBox.Show(Main_lang_dic["errorMsg9"]);
                                i--;
                                break;
                            }
                        }
                    }

                    else if (String.Compare(datas[i], "Motion") == 0)
                    {
                        i++;
                        for (int j = 0; j < ME_Motionlist.Count; j++)
                        {
                            motiontag = (ME_Motion)ME_Motionlist[j];
                            if (String.Compare(datas[i], motiontag.name) != 0)
                                motiontag = null;
                            else
                                break;
                        }
                        if (motiontag == null)
                        {
                            motiontag = new ME_Motion();
                            motiontag.name = datas[i];
                            int try_out;
                            double try_out_d;
                            if (String.Compare("frame", datas[i + 1]) != 0 && String.Compare("home", datas[i + 1]) != 0 &&
                                String.Compare("delay", datas[i + 1]) != 0 && String.Compare("flag", datas[i + 1]) != 0 &&
                                String.Compare("goto", datas[i + 1]) != 0 && String.Compare("MotionEnd", datas[i + 1]) != 0 &&
                                int.TryParse(datas[i + 1], out try_out))
                            { // triggers
                                motiontag.trigger_method = int.Parse(datas[++i]);
                                motiontag.auto_method = int.Parse(datas[++i]);
                                motiontag.trigger_key = int.Parse(datas[++i]);
                                motiontag.trigger_keyType = int.Parse(datas[++i]);
                                i++;
                                if (String.Compare("---noBtKey---", datas[i]) == 0)
                                    motiontag.bt_key = "";
                                else
                                    motiontag.bt_key = datas[i];
                                if (int.TryParse(datas[++i], out try_out) == false)
                                    i--;
                                motiontag.ps2_key = datas[++i];
                                motiontag.ps2_type = int.Parse(datas[++i]);
                                if (String.Compare("frame", datas[i + 1]) != 0 && String.Compare("home", datas[i + 1]) != 0 &&
                                    String.Compare("delay", datas[i + 1]) != 0 && String.Compare("flag", datas[i + 1]) != 0 &&
                                    String.Compare("goto", datas[i + 1]) != 0 && String.Compare("MotionEnd", datas[i + 1]) != 0 &&
                                    String.Compare("Layer", datas[i + 1]) != 0)
                                    motiontag.bt_mode = datas[++i];
                                if (double.TryParse(datas[i + 1], out try_out_d))
                                {
                                    motiontag.acc_Settings[0] = double.Parse(datas[++i]);
                                    motiontag.acc_Settings[1] = double.Parse(datas[++i]);
                                    motiontag.acc_Settings[2] = double.Parse(datas[++i]);
                                    motiontag.acc_Settings[3] = double.Parse(datas[++i]);
                                    motiontag.acc_Settings[4] = double.Parse(datas[++i]);
                                    motiontag.acc_Settings[5] = double.Parse(datas[++i]);
                                    motiontag.acc_Settings[6] = int.Parse(datas[++i]);
                                }
                                if (int.TryParse(datas[i + 1], out try_out))
                                {
                                    motiontag.wifi602_key = int.Parse(datas[++i]);
                                }
                                if (int.TryParse(datas[i + 1], out try_out))
                                {
                                    motiontag.analog_pin = int.Parse(datas[++i]);
                                    motiontag.analog_cond = int.Parse(datas[++i]);
                                    motiontag.analog_value = int.Parse(datas[++i]);
                                }
                            }
                            ME_Motionlist.Add(motiontag);
                        }
                    }
                    else if (String.Compare(datas[i], "Layer") == 0)
                    {
                        i++;
                        int try_out;
                        if (int.TryParse(datas[i], out try_out) == true)
                            motiontag.moton_layer = try_out;
                        while (String.Compare("frame", datas[i + 1]) != 0 && String.Compare("home", datas[i + 1]) != 0 &&
                            String.Compare("delay", datas[i + 1]) != 0 && String.Compare("flag", datas[i + 1]) != 0 &&
                            String.Compare("goto", datas[i + 1]) != 0 && String.Compare("MotionEnd", datas[i + 1]) != 0 &&
                            int.TryParse(datas[i + 1], out try_out))
                        {
                            i++;
                            motiontag.used_servos.Add(try_out);
                        }
                    }
                    else if (String.Compare(datas[i], "MotionEnd") == 0)
                    {
                        int try_out;
                        i++;
                        if (i < datas.Length && int.TryParse(datas[i], out try_out) == true)
                            motiontag.property = try_out;
                        else
                            i--;
                        i++;
                        if (i < datas.Length && int.TryParse(datas[i], out try_out) == true)
                            motiontag.comp_range = try_out;
                        else
                            i--;
                        i++;
                        if (i < datas.Length && int.TryParse(datas[i], out try_out) == true)
                            motiontag.control_method = try_out;
                        else
                            i--;
                        if (motiontag != null)
                            if (String.Compare(datas[++i], motiontag.name) == 0)
                                motiontag = null;
                    }
                    else if (String.Compare(datas[i], "frame") == 0)
                    {
                        ME_Frame nframe = new ME_Frame();
                        nframe.type = 1;
                        i++;
                        try
                        {
                            nframe.delay = int.Parse(datas[i]);
                        }
                        catch
                        {
                            nframe.delay = default_delay;
                            MessageBox.Show(Main_lang_dic["errorMsg10"]);
                        }
                        int j = 0;
                        while (j < 45)
                        {
                            if (String.Compare(nMotion.fbox[j].SelectedItem.ToString(), "---noServo---") != 0)
                            {
                                i++;
                                try
                                {
                                    nframe.frame[j] = int.Parse(datas[i]);
                                }
                                catch
                                {
                                    nframe.frame[j] = 0;
                                    MessageBox.Show(Main_lang_dic["errorMsg10"]);
                                }
                            }
                            else
                            {
                                nframe.frame[j] = 0;
                            }
                            j++;
                        }
                        motiontag.Events.Add(nframe);
                    }
                    else if (String.Compare(datas[i], "home") == 0)
                    {
                        ME_Frame nframe = new ME_Frame();
                        nframe.type = 0;
                        i++;
                        try
                        {
                            nframe.delay = int.Parse(datas[i]);
                        }
                        catch
                        {
                            nframe.delay = default_delay;
                            MessageBox.Show(Main_lang_dic["errorMsg10"]);
                        }
                        int j = 0;
                        while (j < 45)
                        {
                            if (String.Compare(nMotion.fbox[j].SelectedItem.ToString(), "---noServo---") != 0)
                            {
                                i++;
                                try
                                {
                                    nframe.frame[j] = int.Parse(datas[i]);
                                }
                                catch
                                {
                                    nframe.frame[j] = 0;
                                    MessageBox.Show(Main_lang_dic["errorMsg10"]);
                                }
                            }
                            else
                            {
                                nframe.frame[j] = 0;
                            }
                            j++;
                        }
                        motiontag.Events.Add(nframe);
                    }
                    else if (String.Compare(datas[i], "delay") == 0)
                    {
                        ME_Delay ndelay = new ME_Delay();
                        i++;
                        try
                        {
                            ndelay.delay = int.Parse(datas[i]);
                        }
                        catch
                        {
                            ndelay.delay = default_delay;
                            MessageBox.Show(Main_lang_dic["errorMsg11"]);
                        }
                        motiontag.Events.Add(ndelay);
                    }
                    else if (String.Compare(datas[i], "flag") == 0)
                    {
                        ME_Flag nflag = new ME_Flag();
                        i++;
                        nflag.name = datas[i];
                        motiontag.Events.Add(nflag);
                    }
                    else if (String.Compare(datas[i], "goto") == 0)
                    {
                        ME_Goto ngoto = new ME_Goto();
                        i++;
                        ngoto.name = datas[i];
                        i++;
                        if (String.Compare(datas[i], "True") == 0)
                            ngoto.is_goto = true;
                        else
                            ngoto.is_goto = false;
                        i++;
                        int value;
                        bool success = int.TryParse(datas[i], out value);
                        if (!success)
                        {
                            i--;
                            motiontag.Events.Add(ngoto);
                            continue;
                        }
                        ngoto.loops = datas[i];
                        ngoto.current_loop = value;
                        i++;
                        if (String.Compare(datas[i], "True") == 0)
                            ngoto.infinite = true;
                        else if (String.Compare(datas[i], "False") == 0)
                            ngoto.infinite = false;
                        else
                            i--;

                        motiontag.Events.Add(ngoto);
                    }
                    else if (String.Compare(datas[i], "trigger") == 0)
                    {
                        ME_Trigger ntr = new ME_Trigger();
                        ntr.name = datas[++i];
                        ntr.method = int.Parse(datas[++i]);
                        motiontag.Events.Add(ntr);
                    }
                    else if (String.Compare(datas[i], "release") == 0)
                    {
                        ME_Release nr = new ME_Release();
                        motiontag.Events.Add(nr);
                    }
                    else if (String.Compare(datas[i], "compute") == 0)
                    {
                        ME_Compute op = new ME_Compute();
                        op.left_var = int.Parse(datas[++i]);
                        op.form = int.Parse(datas[++i]);
                        op.f1_var1 = int.Parse(datas[++i]);
                        op.f1_op = int.Parse(datas[++i]);
                        op.f1_var2 = int.Parse(datas[++i]);
                        op.f2_op = int.Parse(datas[++i]);
                        op.f2_var = int.Parse(datas[++i]);
                        op.f3_var = int.Parse(datas[++i]);
                        op.f4_const = double.Parse(datas[++i]);
                        motiontag.Events.Add(op);
                    }
                    else if (String.Compare(datas[i], "if") == 0)
                    {
                        ME_If mif = new ME_If();
                        mif.left_var = int.Parse(datas[++i]);
                        mif.method = int.Parse(datas[++i]);
                        mif.right_var = int.Parse(datas[++i]);
                        mif.name = datas[++i];
                        motiontag.Events.Add(mif);
                    }
                }
            }

            nMotion.write_back();

            if (nMotion.picfilename != null && picmode == true)
            {
                picture_name = nMotion.picfilename;
                try
                {
                    Robot_pictureBox.Image = Image.FromFile(nMotion.picfilename);
                }
                catch
                {
                    nMotion.picfilename = null;
                    MessageBox.Show(Main_lang_dic["errorMsg3"]);
                }
            }
            else
            {
                nMotion.picfilename = null;
                Robot_pictureBox.Image = null;
            }

            initAnalog();
            initPs2();

            Hint_groupBox.Enabled = true;
            Motion_groupBox.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            saveFileToolStripMenuItem.Enabled = true;
            Motion = nMotion;

            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                MotionCombo.Items.Add(m.name);
            }

            if (MotionCombo.Items.Count > 0)
                MotionCombo.SelectedIndex = 0;
            this.hint_richTextBox.Text =
                            "   ___   __   ____        _\n" +
                            "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                            "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                            " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                            "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
            MessageBox.Show(filename + Main_lang_dic["loadedText"]);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            MotionTest.Enabled = false;
            motion_pause.Enabled = false;
            motion_stop.Enabled = false;
        }

        private void MotionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            MotionConfig.Enabled = true;
            update_motionlist();
            move_up.Enabled = false;
            move_down.Enabled = false;
            current_motionlist_idx = -1;
            // Motion Trigger part
            Always_radioButton.Enabled = true;
            Keyboard_radioButton.Enabled = true;
            bt_radioButton.Enabled = true;
            ps2_radioButton.Enabled = true;
            acc_radioButton.Enabled = true;
            analog_radioButton.Enabled = true;
            ME_Motion m = ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]);
            if (m.trigger_method == (int)mtest_method.always)
            {
                Always_radioButton.Checked = true;
            }
            else if(m.trigger_method == (int)mtest_method.keyboard)
            {
                Keyboard_radioButton.Checked = true;
            }
            else if (m.trigger_method == (int)mtest_method.bluetooth)
            {
                bt_radioButton.Checked = true;
            }
            else if (m.trigger_method == (int)mtest_method.ps2)
            {
                ps2_radioButton.Checked = true;
            }
            else if (m.trigger_method == (int)mtest_method.acc)
            {
                acc_radioButton.Checked = true;
            }
            else if (m.trigger_method == (int)mtest_method.wifi602)
            {
                wifi602_radioButton.Checked = true;
            }
            else if (m.trigger_method == (int)mtest_method.analog)
            {
                analog_radioButton.Checked = true;
            }
            if (m.auto_method == (int)auto_method.on)
                AlwaysOn.Checked = true;
            else if (m.auto_method == (int)auto_method.off)
                AlwaysOff.Checked = true;
            else if (m.auto_method == (int)auto_method.title)
                TitleMotion.Checked = true;
            KeyboardCombo.SelectedIndex = m.trigger_key;
            KeyboardTypeCombo.SelectedIndex = m.trigger_keyType;
            btKeyText.Text = m.bt_key;
            btModeCombo.Text = m.bt_mode;
            btPortCombo.Text = gs.bt_port;
            btBaudCombo.Text = gs.bt_baud;
            wifi602PortCombo.Text = gs.wifi602_port;
            wifi602KeyCombo.SelectedIndex = m.wifi602_key;
            ps2DATCombo.Text = gs.ps2pins[0];
            ps2CMDCombo.Text = gs.ps2pins[1];
            ps2ATTCombo.Text = gs.ps2pins[2];
            ps2CLKCombo.Text = gs.ps2pins[3];
            ps2KeyCombo.Text = m.ps2_key;
            ps2TypeCombo.SelectedIndex = m.ps2_type;
            accLXText.Text = m.acc_Settings[0].ToString();
            accHXText.Text = m.acc_Settings[1].ToString();
            accLYText.Text = m.acc_Settings[2].ToString();
            accHYText.Text = m.acc_Settings[3].ToString();
            accLZText.Text = m.acc_Settings[4].ToString();
            accHZText.Text = m.acc_Settings[5].ToString();
            accDurationText.Text = m.acc_Settings[6].ToString();
            analogCondCombo.SelectedIndex = m.analog_cond;
            analogPinCombo.SelectedIndex = m.analog_pin;
            analogValueText.Text = m.analog_value.ToString();
            // Motion Property Part
            Blocking.Enabled = true;
            NonBlocking.Enabled = true;
            if (m.property == (int)motion_property.blocking)
                Blocking.Checked = true;
            else if (m.property == (int)motion_property.nonblocking)
                NonBlocking.Checked = true;
            MotionLayerCombo.SelectedIndex = m.moton_layer;
            CompRangeText.Text = m.comp_range.ToString();
            MotionControlCombo.SelectedIndex = m.control_method;
            Framelist.Controls.Clear();
            current_motionlist_idx = -1;
            last_motionlist_idx = -1;
            freshflag[1] = false;
            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count > 0 && MotionConfig.SelectedIndex == 0)
            {
                Motionlist.SelectedIndex = 0;
            }
            if (MotionConfig.SelectedIndex == 0)
                this.hint_richTextBox.Text =
                            "   ___   __   ____        _\n" +
                            "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                            "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                            " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                            "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
                else
                    this.hint_richTextBox.Text = Main_lang_dic["hint9"];
        }

        private void gototext(object sender, EventArgs e)// set names of Goto & Flag
        {
            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Flag)
            {
                ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = ((MaskedTextBox)sender).Text;
                Motionlist.Items[current_motionlist_idx] = "[Flag] " + ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
            }
            else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Goto)
            {
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = ((MaskedTextBox)sender).Text;
                Motionlist.Items[current_motionlist_idx] = "[Goto] " + ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
            }
        }

        private void iftext(object sender, EventArgs e)
        {
            ME_If mif = (ME_If)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            mif.name = ((MaskedTextBox)sender).Text;
            Motionlist.Items[current_motionlist_idx] = "[If] " + convertIndex2Str(mif.left_var, 0) + convertIndex2Str(mif.method, 3) +
                                                       convertIndex2Str(mif.right_var, 0) + " goto " + mif.name;
        }

        private void triggerMotion_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((ME_Trigger)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = ((ComboBox)sender).Text;
            Motionlist.Items[current_motionlist_idx] = "[GotoMotion] " + ((ME_Trigger)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
        }

        private void callRadio_CheckedChanged(object sender, EventArgs e)
        {
            if(((RadioButton)sender).Checked == true )
                ((ME_Trigger)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).method = (int)internal_trigger.call;
        }

        private void jumpRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked == true)
                ((ME_Trigger)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).method = (int)internal_trigger.jump;
        }

        private void enable_goto(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).is_goto = true;
                ((CheckBox)Framelist.Controls.Find("loop_inf", true)[0]).Enabled = true;
                ((MaskedTextBox)Framelist.Controls.Find("loop_num", true)[0]).Enabled = true;
                ((Label)Framelist.Controls.Find("loop_inf_l", true)[0]).Enabled = true;
            }
            else
            {
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).is_goto = false;
                ((CheckBox)Framelist.Controls.Find("loop_inf", true)[0]).Enabled = false;
                ((MaskedTextBox)Framelist.Controls.Find("loop_num", true)[0]).Enabled = false;
                ((Label)Framelist.Controls.Find("loop_inf_l", true)[0]).Enabled = false;
            }
        }

        private void enable_infinite(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).infinite = true;
                ((MaskedTextBox)Framelist.Controls.Find("loop_num", true)[0]).Enabled = false;
            }
            else
            {
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).infinite = false;
                ((MaskedTextBox)Framelist.Controls.Find("loop_num", true)[0]).Enabled = true;
            }
        }

        private void operandRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            ME_Compute op = (ME_Compute)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            op.form = int.Parse(((RadioButton)sender).Name);
            Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
        }

        private double opVal(int index)
        {
            if (index < opVar_num)
            {
                return operand_var[index];
            }
            else if (index < opVar_num + 8 && index >= opVar_num)
            {
                if (string.Compare(com_port, "OFF") != 0)
                {
                    try
                    {
                        arduino.pin_capture(index - opVar_num);
                        DateTime time_start = DateTime.Now;
                        while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 500) ;
                        arduino.dataRecieved = false;
                        return arduino.captured_data;
                    }
                    catch
                    {
                        com_port = "OFF";
                        MessageBox.Show(Main_lang_dic["errorMsg1"]);
                    }
                }
            }
            else if (index < opVar_num + 11 && index >= opVar_num + 8)
            {
                if (string.Compare(com_port, "OFF") != 0 && Motion.getQ.Enabled == true)
                {
                    try
                    {
                        arduino.pin_capture(index - opVar_num);
                        DateTime time_start = DateTime.Now;
                        while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 500) ;
                        arduino.dataRecieved = false;
                        return arduino.captured_float;
                    }
                    catch
                    {
                        com_port = "OFF";
                        MessageBox.Show(Main_lang_dic["errorMsg1"]);
                    }
                }
            }
            else if (index < opVar_num + 13 && index >= opVar_num + 11)
            {
                try
                {
                    Quaternion RcvQ = new Quaternion();
                    arduino.getQ();
                    DateTime time_start = DateTime.Now;
                    while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 100) ;
                    arduino.dataRecieved = false;
                    RcvQ.w = arduino.quaternion[0];
                    RcvQ.x = arduino.quaternion[1];
                    RcvQ.y = arduino.quaternion[2];
                    RcvQ.z = arduino.quaternion[3];
                    RollPitchYaw rpy = (RcvQ.Normalized() * Motion.q.Normalized().Inverse()).toRPY();
                    if (index == opVar_num + 11)
                        return (float)rpy.rpy[0];
                    else
                        return (float)rpy.rpy[1];
                }
                catch
                {
                    com_port = "OFF";
                    MessageBox.Show(Main_lang_dic["errorMsg1"]);
                }
            }
            return 0;
        }

        private bool ifResult(int ind1, int ind2, int method)
        {
            switch (method)
            {
                case 0:
                    return float.Equals(opVal(ind1), opVal(ind2));
                case 1:
                    return !float.Equals(opVal(ind1), opVal(ind2));
                case 2:
                    return opVal(ind1) >= opVal(ind2);
                case 3:
                    return opVal(ind1) <= opVal(ind2);
                case 4:
                    return opVal(ind1) > opVal(ind2);
                case 5:
                    return opVal(ind1) < opVal(ind2);
                default:
                    break;
            }
            return false;
        }

        private double opOperate(double val1, double val2, int form, int method)
        {
            switch (form)
            {
                case 0:
                    if (method == 0)
                        return val1 + val2;
                    else if (method == 1)
                        return val1 - val2;
                    else if (method == 2)
                        return val1 * val2;
                    else if (method == 3 && val2 != 0)
                        return val1 / val2;
                    else if (method == 4)
                        return (float)Math.Pow(val1, val2);
                    else if (method == 5 && val2 != 0)
                        return val1 % val2;
                    else if (method == 6)
                        return (int)val1 & (int)val2;
                    else if (method == 7)
                        return (int)val1 | (int)val2;
                    break;
                case 1:
                    if (method == 0)
                        return ~((int)val2);
                    else if (method == 1)
                        return (float)Math.Sqrt(val2);
                    else if (method == 2)
                        return (float)Math.Exp(val2);
                    else if (method == 3 && val2 != 0)
                        return (float)Math.Log(val2, Math.E);
                    else if (method == 4 && val2 != 0)
                        return (float)Math.Log10(val2);
                    else if (method == 5)
                        return Math.Abs(val2);
                    else if (method == 6)
                        return -val2;
                    else if (method == 7)
                        return (float)Math.Cos(val2);
                    else if (method == 8)
                        return (float)Math.Sin(val2);
                    break;
                default:
                    break;
            }
            return 0;
        }

        private void opVar_Handle(object sender, EventArgs e)
        {
            ME_Compute op = new ME_Compute();
            ME_If mif = new ME_If();
            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Compute)
                op = (ME_Compute)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            else
                mif = (ME_If)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            switch(((ComboBox)sender).Name)
            {
                case "0":
                    op.left_var = ((ComboBox)sender).SelectedIndex;
                    Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "1":
                    op.f1_var1 = ((ComboBox)sender).SelectedIndex;
                    if (op.form == 0)
                        Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "2":
                    op.f1_var2 = ((ComboBox)sender).SelectedIndex;
                    if (op.form == 0)
                        Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "3":
                    op.f2_var = ((ComboBox)sender).SelectedIndex;
                    if (op.form == 1)
                        Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "4":
                    op.f3_var = ((ComboBox)sender).SelectedIndex;
                    if (op.form == 2)
                        Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "5":
                    op.f1_op = ((ComboBox)sender).SelectedIndex;
                    if (op.form == 0)
                        Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "6":
                    op.f2_op = ((ComboBox)sender).SelectedIndex;
                    if (op.form == 1)
                        Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
                    break;
                case "i0":
                    mif.left_var = ((ComboBox)sender).SelectedIndex;
                    Motionlist.Items[current_motionlist_idx] = "[If] " + convertIndex2Str(mif.left_var, 0) + convertIndex2Str(mif.method, 3) +
                                                                convertIndex2Str(mif.right_var, 0) + " goto " + mif.name;
                    break;
                case "i1":
                    mif.method = ((ComboBox)sender).SelectedIndex;
                    Motionlist.Items[current_motionlist_idx] = "[If] " + convertIndex2Str(mif.left_var, 0) + convertIndex2Str(mif.method, 3) +
                                                                convertIndex2Str(mif.right_var, 0) + " goto " + mif.name;
                    break;
                case "i2":
                    mif.right_var = ((ComboBox)sender).SelectedIndex;
                    Motionlist.Items[current_motionlist_idx] = "[If] " + convertIndex2Str(mif.left_var, 0) + convertIndex2Str(mif.method, 3) +
                                                                convertIndex2Str(mif.right_var, 0) + " goto " + mif.name;
                    break;
                default:
                    break;
            }
        }

        private void setOpComboBox(ComboBox cb, int top, int left, string name)
        {
            ME_Compute op = new ME_Compute();
            ME_If mif = new ME_If();
            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Compute)
                op = (ME_Compute)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            else
                mif = (ME_If)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            cb.Name = name;
            cb.Size = new Size(55, 22);
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Top += top;
            cb.Left += left;
            switch (name)
            {
                case "5":
                    cb.Items.Add("+");
                    cb.Items.Add("-");
                    cb.Items.Add("×");
                    cb.Items.Add("÷");
                    cb.Items.Add("^");
                    cb.Items.Add("%");
                    cb.Items.Add("AND");
                    cb.Items.Add("OR");
                    cb.SelectedIndex = op.f1_op;
                    break;
                case "6":
                    cb.Items.Add("NOT");
                    cb.Items.Add("√");
                    cb.Items.Add("exp");
                    cb.Items.Add("㏑");
                    cb.Items.Add("㏒");
                    cb.Items.Add("abs");
                    cb.Items.Add("-");
                    cb.Items.Add("cos");
                    cb.Items.Add("sin");
                    cb.SelectedIndex = op.f2_op;
                    break;
                case "i1":
                    cb.Items.Add("=");
                    cb.Items.Add("≠");
                    cb.Items.Add("≧");
                    cb.Items.Add("≦");
                    cb.Items.Add(">");
                    cb.Items.Add("<");
                    cb.SelectedIndex = mif.method;
                    break;
                default:
                    break;
            }
        }

        private void setOpVComboBox(ComboBox cb, int top, int left, string name, bool isLeft)
        {
            ME_Compute op = new ME_Compute();
            ME_If mif = new ME_If();
            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Compute)
                op = (ME_Compute)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            else
                mif = (ME_If)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            cb.Name = name;
            cb.Size = new Size(100, 22);
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            cb.Top += top;
            cb.Left += left;
            for (int i = 0; i < opVar_num; i++)
                cb.Items.Add("V" + i);
            if (!isLeft)
            {
                cb.Items.Add("NowTime");
                cb.Items.Add("Random");
                for (int i = 0; i < 6; i++)
                    cb.Items.Add("Analog" + i);
                cb.Items.Add("AccX");
                cb.Items.Add("AccY");
                cb.Items.Add("AccZ");
                cb.Items.Add("GyroRoll");
                cb.Items.Add("GyroPitch");
            }
            switch (name)
            {
                case "0":
                    cb.SelectedIndex = op.left_var;
                    break;
                case "1":
                    cb.SelectedIndex = op.f1_var1;
                    break;
                case "2":
                    cb.SelectedIndex = op.f1_var2;
                    break;
                case "3":
                    cb.SelectedIndex = op.f2_var;
                    break;
                case "4":
                    cb.SelectedIndex = op.f3_var;
                    break;
                case "i0":
                    cb.SelectedIndex = mif.left_var;
                    break;
                case "i2":
                    cb.SelectedIndex = mif.right_var;
                    break;
                default:
                    break;
            }
        }

        private void OpConst_TextChanged(object sender, EventArgs e)
        {
            ME_Compute op = (ME_Compute)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];
            double output;
            if (double.TryParse(((MaskedTextBox)sender).Text, out output))
            {
                op.f4_const = output;
                if (op.form == 3)
                    Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
            }
            else if (((MaskedTextBox)sender).Text == "-" || ((MaskedTextBox)sender).Text == "" ||
                     ((MaskedTextBox)sender).Text == "-." || ((MaskedTextBox)sender).Text == ".")
            {
                op.f4_const = 0;
                if (op.form == 3)
                    Motionlist.Items[current_motionlist_idx] = "[Compute] " + Operand2Text(op);
            }
            else
            {
                MessageBox.Show(Main_lang_dic["errorMsg19"]);
                ((MaskedTextBox)sender).SelectAll();
            }
        }

        private string convertIndex2Str(int n, int type)
        {
            if (type == 0)
            {
                if (n < opVar_num)
                    return "V" + n;
                else if (n == opVar_num)
                    return "NowTime";
                else if (n == opVar_num + 1)
                    return "Random";
                else if (n < opVar_num + 8 && n >= opVar_num + 2)
                    return "A" + (n - opVar_num - 2);
                else if (n == opVar_num + 8)
                    return "AccX";
                else if (n == opVar_num + 9)
                    return "AccY";
                else if (n == opVar_num + 10)
                    return "AccZ";
                else if (n == opVar_num + 11)
                    return "Roll";
                else if (n == opVar_num + 12)
                    return "Pitch";
            }
            else if (type == 1)
            {
                switch(n)
                {
                    case 0: return "+";
                    case 1: return "-";
                    case 2: return "×";
                    case 3: return "÷";
                    case 4: return "^";
                    case 5: return "%";
                    case 6: return "AND";
                    case 7: return "OR";
                    default: break;
                }
            }
            else if (type == 2)
            {
                switch(n)
                {
                    case 0: return "NOT";
                    case 1: return "√";
                    case 2: return "exp";
                    case 3: return "㏑";
                    case 4: return "㏒";
                    case 5: return "abs";
                    case 6: return "-";
                    case 7: return "cos";
                    case 8: return "sin";
                    default: break;
                }
            }
            else if (type == 3)
            {
                switch(n)
                {
                    case 0: return "=";
                    case 1: return "≠";
                    case 2: return "≧";
                    case 3: return "≦";
                    case 4: return ">";
                    case 5: return "<";
                    default: break;
                }
            }
            return "";
        }

        private string Operand2Text(ME_Compute op)
        {
            switch(op.form)
            {
                case 0:
                    return convertIndex2Str(op.left_var, 0) + "=" + convertIndex2Str(op.f1_var1, 0) +
                           convertIndex2Str(op.f1_op, 1) + convertIndex2Str(op.f1_var2, 0);
                case 1:
                    return convertIndex2Str(op.left_var, 0) + "=" + convertIndex2Str(op.f2_op, 2) + convertIndex2Str(op.f2_var, 0);
                case 2:
                    return convertIndex2Str(op.left_var, 0) + "=" + convertIndex2Str(op.f3_var, 0);
                case 3:
                    return convertIndex2Str(op.left_var, 0) + "=" + op.f4_const.ToString();
                default:
                    break;
            }
            return "";
        }

        private void Motionlist_SelectedIndexChanged(object sender, EventArgs e) // select motionlist
        {
            if (Motionlist.SelectedIndex == -1)
            {
                DelayLabel.Visible = false;
                delaytext.Visible = false;
                delayUnitLabel.Visible = false;
                current_motionlist_idx = Motionlist.SelectedIndex;
                saveFrame.Visible = false;
                loadFrame.Visible = false;
                move_up.Enabled = false;
                move_down.Enabled = false;
                freshflag[1] = false;
                this.DelayLabel.Text = Main_lang_dic["Label2TextDelay"];
            }
            if (Motionlist.SelectedItem != null && (MotionTest.Enabled))
            {
                move_up.Enabled = true;
                move_down.Enabled = true;
                current_motionlist_idx = Motionlist.SelectedIndex;
                Action_groupBox.Enabled = true;
                Setting_groupBox.Enabled = true;
                string[] datas = Motionlist.SelectedItem.ToString().Split(' ');
                if (String.Compare(datas[0], "[Frame]") == 0)
                {
                    delayUnitLabel.Visible = true;
                    DelayLabel.Visible = true;
                    delaytext.Visible = true;
                    saveFrame.Visible = true;
                    loadFrame.Visible = true;
                    autocheck.Enabled = true;
                    this.DelayLabel.Text = Main_lang_dic["Label2TextPlayTime"];
                    ME_Frame f = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]);
                    delaytext.Text = f.delay.ToString();
                    if (!freshflag[1])
                    {
                        Update_framelist();
                        freshflag[1] = true;
                    }
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                autoframe[i] = (int)(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] + offset[i]);
                                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).moton_layer != 0 && !((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).used_servos.Contains(i))
                                    autoframe[i] = 0;
                            }
                            else
                                autoframe[i] = 0;
                        }
                        autocheck.Enabled = false;
                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, (int)(f.delay), servo_onOff);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show(Main_lang_dic["errorMsg1"]);
                            }
                        }
                        autocheck.Enabled = true;
                    }
                    freshflag[0] = false;
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && fpanel[i] != null)
                        {
                            freshflag[0] = true;
                            uint frame_value = ((uint)((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i]);
                            if(frame_value <= Max[i] && frame_value >= min[i])
                                ftext[i].Text = frame_value.ToString();
                            else
                            {
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] = (int)homeframe[i];
                                ftext[i].Text = homeframe[i].ToString();
                            }
                        }
                    }
                    freshflag[0] = false;
                    Framelist.Enabled = true;
                    delaytext.Enabled = true;
                    capturebutton.Enabled = true;
                    draw_background();
                    if (Motion.picfilename == null)
                        this.hint_richTextBox.Text = Main_lang_dic["hint2"];
                    else
                        this.hint_richTextBox.Text = Main_lang_dic["hint3"];
                }
                else if (String.Compare(datas[0], "[Home]") == 0)
                {
                    delayUnitLabel.Visible = true;
                    DelayLabel.Visible = true;
                    delaytext.Visible = true;
                    saveFrame.Visible = false;
                    loadFrame.Visible = false;
                    autocheck.Enabled = true;
                    this.DelayLabel.Text = Main_lang_dic["Label2TextPlayTime"];
                    ME_Frame h = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]);
                    delaytext.Text = h.delay.ToString();
                    if (!freshflag[1])
                    {
                        Update_framelist();
                        freshflag[1] = true;
                    }
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                                autoframe[i] = (int)homeframe[i] + offset[i];
                            else
                                autoframe[i] = 0;
                        }
                        autocheck.Enabled = false;
                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, (int)(h.delay), servo_onOff);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show(Main_lang_dic["errorMsg1"]);
                            }
                        }
                        autocheck.Enabled = true;
                    }
                    freshflag[0] = false;
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && fpanel[i] != null)
                        {
                            freshflag[0] = true;
                            uint frame_value = homeframe[i];
                            if (frame_value <= Max[i] && frame_value >= min[i])
                                ftext[i].Text = frame_value.ToString();
                            else
                            {
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] = (int)homeframe[i];
                                ftext[i].Text = homeframe[i].ToString();
                            }
                        }
                    }
                    freshflag[0] = false;
                    Framelist.Enabled = false;
                    delaytext.Enabled = true;
                    capturebutton.Enabled = false;
                    draw_background();
                    this.hint_richTextBox.Text = Main_lang_dic["hint4"];
                }
                else if (String.Compare(datas[0], "[Delay]") == 0)
                {
                    delayUnitLabel.Visible = true;
                    DelayLabel.Visible = true;
                    delaytext.Visible = true;
                    saveFrame.Visible = false;
                    loadFrame.Visible = false;
                    this.DelayLabel.Text = Main_lang_dic["Label2TextDelay"];
                    delaytext.Text = ((ME_Delay)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                    delaytext.Enabled = true;
                    Framelist.Enabled = false;
                    Framelist.Controls.Clear();
                    capturebutton.Enabled = false;
                    autocheck.Enabled = false;
                    freshflag[1] = false;
                    this.hint_richTextBox.Text = Main_lang_dic["hint5"];
                }
                else if (String.Compare(datas[0], "[Flag]") == 0)
                {
                    delayUnitLabel.Visible = false;
                    saveFrame.Visible = false;
                    loadFrame.Visible = false;
                    DelayLabel.Visible = false;
                    delaytext.Visible = false;
                    Framelist.Enabled = true;
                    capturebutton.Enabled = false;
                    autocheck.Enabled = false;
                    freshflag[1] = false;

                    if (current_motionlist_idx == last_motionlist_idx)
                        return;

                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = Main_lang_dic["flag_xlabel"];
                    xlabel.Size = new Size(45, 22);

                    MaskedTextBox xtext = new MaskedTextBox();
                    
                    xtext.Text = ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    xtext.TextChanged += new EventHandler(gototext);
                    xtext.Size = new Size(160, 22);
                    xtext.Left += 45;
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xtext);
                    Framelist.Enabled = true;
                    this.hint_richTextBox.Text = Main_lang_dic["hint6"];
                    xtext.SelectionStart = xtext.Text.Length;
                    xtext.Focus();
                }
                else if (String.Compare(datas[0], "[Goto]") == 0)
                {
                    delayUnitLabel.Visible = false;
                    DelayLabel.Visible = false;
                    delaytext.Visible = false;
                    Framelist.Enabled = true;
                    capturebutton.Enabled = false;
                    autocheck.Enabled = false;
                    freshflag[1] = false;
                    saveFrame.Visible = false;
                    loadFrame.Visible = false;

                    if (current_motionlist_idx == last_motionlist_idx)
                        return;

                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = Main_lang_dic["goto_xlabel"];
                    xlabel.Size = new Size(95, 22);
                    MaskedTextBox xtext = new MaskedTextBox();
                    xtext.Text = ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    xtext.TextChanged += new EventHandler(gototext);
                    xtext.Size = new Size(160, 22);
                    xtext.Left += 100;
                    Label xlabel2 = new Label();
                    xlabel2.Text = Main_lang_dic["goto_xlabel2"];
                    xlabel2.Size = new Size(65, 22);
                    xlabel2.Top += 32;
                    CheckBox xcheckbox = new CheckBox();
                    xcheckbox.Checked = ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).is_goto;
                    xcheckbox.CheckedChanged += new EventHandler(enable_goto);
                    xcheckbox.Size = new Size(15, 15);
                    xcheckbox.Top += 32;
                    xcheckbox.Left += 65;
                    Label xlabel4 = new Label();
                    xlabel4.Name = "loop_inf_l";
                    xlabel4.Enabled = xcheckbox.Checked;
                    xlabel4.Text = Main_lang_dic["goto_xlabel4"];
                    xlabel4.Size = new Size(80, 22);
                    xlabel4.Top += 32;
                    xlabel4.Left += 85;
                    CheckBox xcheckbox2 = new CheckBox();
                    xcheckbox2.Enabled = xcheckbox.Checked;
                    xcheckbox2.Name = "loop_inf";
                    xcheckbox2.Size = new Size(15, 15);
                    xcheckbox2.Checked = ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).infinite;
                    xcheckbox2.CheckedChanged += new EventHandler(enable_infinite);
                    xcheckbox2.Top += 32;
                    xcheckbox2.Left += 165;
                    Label xlabel3 = new Label();
                    xlabel3.Text = Main_lang_dic["goto_xlabel3"];
                    xlabel3.Size = new Size(95, 22);
                    xlabel3.Top += 62;
                    MaskedTextBox xtext2 = new MaskedTextBox();
                    if (xcheckbox.Checked == false)
                        xtext2.Enabled = false;
                    else if (xcheckbox2.Checked == true)
                        xtext2.Enabled = false;
                    xtext2.Name = "loop_num";
                    xtext2.Text = ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).loops;
                    xtext2.KeyPress += new KeyPressEventHandler(numbercheck);
                    xtext2.TextChanged += new EventHandler(loops_TextChanged);
                    xtext2.Size = new Size(160, 22);
                    xtext2.Left += 100;
                    xtext2.Top += 62;
                    this.hint_richTextBox.Text = Main_lang_dic["hint10"];
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xtext);
                    Framelist.Controls.Add(xlabel2);
                    Framelist.Controls.Add(xcheckbox);
                    Framelist.Controls.Add(xlabel3);
                    Framelist.Controls.Add(xtext2);
                    Framelist.Controls.Add(xlabel4);
                    Framelist.Controls.Add(xcheckbox2);
                    Framelist.Enabled = true;
                    this.hint_richTextBox.Text = Main_lang_dic["hint7"];
                    xtext.SelectionStart = xtext.Text.Length;
                    xtext.Focus();
                }
                else if (String.Compare(datas[0], "[GotoMotion]") == 0)
                {
                    delayUnitLabel.Visible = false;
                    DelayLabel.Visible = false;
                    delaytext.Visible = false;
                    Framelist.Enabled = true;
                    capturebutton.Enabled = false;
                    autocheck.Enabled = false;
                    freshflag[1] = false;

                    saveFrame.Visible = false;
                    loadFrame.Visible = false;
                    this.DelayLabel.Text = Main_lang_dic["Label2TextDelay"];
                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = Main_lang_dic["gotoMotion_xlabel"];
                    xlabel.Size = new Size(85, 20);
                    Label xlabel2 = new Label();
                    xlabel2.Text = Main_lang_dic["gotoMotion_xlabel2"];
                    xlabel2.Size = new Size(80, 20);
                    Label xlabel3 = new Label();
                    xlabel3.Text = Main_lang_dic["gotoMotion_xlabel3"];
                    xlabel3.Size = new Size(450, 20);
                    Label xlabel4 = new Label();
                    xlabel4.Text = Main_lang_dic["gotoMotion_xlabel4"];
                    xlabel4.Size = new Size(450, 20);
                    Label xlabel5 = new Label();
                    xlabel5.Text = Main_lang_dic["gotoMotion_xlabel5"];
                    xlabel5.Size = new Size(650, 20);

                    ComboBox xcombo = new ComboBox();
                    xcombo.Size = new Size(160, 22);
                    RadioButton call_radio = new RadioButton();
                    RadioButton jump_radio = new RadioButton();
                    call_radio.Size = new Size(300, 20);
                    jump_radio.Size = new Size(300, 20);
                    
                    for (int i = 0; i < MotionCombo.Items.Count; i++)
                        xcombo.Items.Add(((ME_Motion)ME_Motionlist[i]).name);
                    call_radio.Text = Main_lang_dic["call_radioText"];
                    jump_radio.Text = Main_lang_dic["jump_radioText"];

                    xcombo.Text = ((ME_Trigger)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    if (((ME_Trigger)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).method == (int)internal_trigger.call)
                        call_radio.Checked = true;
                    else
                        jump_radio.Checked = true;

                    xcombo.DropDownStyle = ComboBoxStyle.DropDownList;

                    xcombo.SelectedIndexChanged += new EventHandler(triggerMotion_SelectedIndexChanged);
                    call_radio.CheckedChanged += new EventHandler(callRadio_CheckedChanged);
                    jump_radio.CheckedChanged += new EventHandler(jumpRadio_CheckedChanged);

                    xlabel.Top += 3;
                    xlabel2.Top += 25;
                    xlabel3.Top += 105;
                    xlabel3.Left += 26;
                    xlabel4.Top += 65;
                    xlabel4.Left += 26;
                    xlabel5.Top += 135;
                    xcombo.Left += 85;
                    call_radio.Top += 85;
                    call_radio.Left += 10;
                    jump_radio.Top += 45;
                    jump_radio.Left += 10;
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xcombo);
                    Framelist.Controls.Add(xlabel2);
                    Framelist.Controls.Add(xlabel3);
                    Framelist.Controls.Add(xlabel4);
                    Framelist.Controls.Add(xlabel5);
                    Framelist.Controls.Add(jump_radio);
                    Framelist.Controls.Add(call_radio);
                    Framelist.Enabled = true;

                    this.hint_richTextBox.Text = Main_lang_dic["hint8"];
                }
                else if (String.Compare(datas[0], "[Release]") == 0)
                {
                    delayUnitLabel.Visible = false;
                    DelayLabel.Visible = false;
                    delaytext.Visible = false;
                    Framelist.Enabled = true;
                    capturebutton.Enabled = false;
                    freshflag[1] = false;

                    saveFrame.Visible = false;
                    loadFrame.Visible = false;
                    Framelist.Controls.Clear();

                    if (autocheck.Checked == true)
                        if (string.Compare(com_port, "OFF") != 0)
                            arduino.motor_release();

                    this.hint_richTextBox.Text = Main_lang_dic["hint14"];
                }
                else if (String.Compare(datas[0], "[Compute]") == 0)
                {
                    delayUnitLabel.Visible = false;
                    DelayLabel.Visible = false;
                    delaytext.Visible = false;
                    
                    ME_Compute op = (ME_Compute)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];

                    Framelist.Enabled = true;
                    capturebutton.Enabled = false;
                    autocheck.Enabled = false;
                    freshflag[1] = false;

                    saveFrame.Visible = false;
                    loadFrame.Visible = false;

                    if (current_motionlist_idx == last_motionlist_idx)
                        return;

                    Framelist.Controls.Clear();

                    ComboBox left_var = new ComboBox();
                    setOpVComboBox(left_var, 5, 5, "0", true);

                    Label equal = new Label();
                    equal.Text = "=";
                    equal.Top += 5;
                    equal.Left += 105;
                    equal.Font = new Font("Arial", 14);
                    equal.Size = new Size(22,22);

                    RadioButton[] method = new RadioButton[4];
                    for (int i = 0; i < 4; i++)
                    {
                        method[i] = new RadioButton();
                        method[i].Name = i.ToString();
                        method[i].Top += 5 + i * 25;
                        method[i].Left += 128;
                        method[i].Size = new Size(22, 22);
                        Framelist.Controls.Add(method[i]);
                    }
                    method[op.form].Checked = true;

                    ComboBox f1_var1 = new ComboBox();
                    setOpVComboBox(f1_var1, 5, 150, "1", false);
                    ComboBox f1_var2 = new ComboBox();
                    setOpVComboBox(f1_var2, 5, 311, "2", false);
                    ComboBox f2_var = new ComboBox();
                    setOpVComboBox(f2_var, 30, 208, "3", false);
                    ComboBox f3_var = new ComboBox();
                    setOpVComboBox(f3_var, 55, 150, "4", false);
                    ComboBox f1_op = new ComboBox();
                    setOpComboBox(f1_op, 5, 253, "5");
                    ComboBox f2_op = new ComboBox();
                    setOpComboBox(f2_op, 30, 150, "6");

                    EventHandler op_var_handle = new EventHandler(opVar_Handle);
                    left_var.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    f1_var1.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    f1_var2.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    f2_var.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    f3_var.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    f1_op.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    f2_op.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    for (int i = 0; i < 4; i++)
                        method[i].CheckedChanged += new EventHandler(operandRadioButton_CheckedChanged);

                    MaskedTextBox f4_const = new MaskedTextBox();
                    f4_const.Size = new Size(100, 22);
                    f4_const.Top += 80;
                    f4_const.Left += 150;
                    f4_const.Text = op.f4_const.ToString(".0#######");
                    f4_const.KeyPress += new KeyPressEventHandler(floatcheck);
                    f4_const.TextChanged += new EventHandler(OpConst_TextChanged);

                    Framelist.Controls.Add(left_var);
                    Framelist.Controls.Add(equal);
                    Framelist.Controls.Add(f1_var1);
                    Framelist.Controls.Add(f1_var2);
                    Framelist.Controls.Add(f2_var);
                    Framelist.Controls.Add(f3_var);
                    Framelist.Controls.Add(f1_op);
                    Framelist.Controls.Add(f2_op);
                    Framelist.Controls.Add(f4_const);

                    this.hint_richTextBox.Text = Main_lang_dic["hint15"];
                    f4_const.SelectionStart = f4_const.Text.Length;
                    f4_const.Focus();
                }
                else if (String.Compare(datas[0], "[If]") == 0)
                {
                    ME_If mif = (ME_If)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex];

                    delayUnitLabel.Visible = false;
                    DelayLabel.Visible = false;
                    delaytext.Visible = false;
                    Framelist.Enabled = true;
                    capturebutton.Enabled = false;
                    freshflag[1] = false;

                    saveFrame.Visible = false;
                    loadFrame.Visible = false;

                    if (current_motionlist_idx == last_motionlist_idx)
                        return;

                    Framelist.Controls.Clear();

                    Label lstmt = new Label();
                    lstmt.Text = "if (";
                    lstmt.Top += 3;
                    lstmt.Left += 5;
                    lstmt.Font = new Font("Arial", 14);
                    lstmt.Size = new Size(35, 22);

                    Label rstmt = new Label();
                    rstmt.Text = ")";
                    rstmt.Top += 3;
                    rstmt.Left += 330;
                    rstmt.Font = new Font("Arial", 14);
                    rstmt.Size = new Size(22, 22);

                    Label gstmt = new Label();
                    gstmt.Text = "goto ";
                    gstmt.Top += 30;
                    gstmt.Left += 5;
                    gstmt.Font = new Font("Arial", 14);
                    gstmt.Size = new Size(50, 22);

                    ComboBox left_var = new ComboBox();
                    setOpVComboBox(left_var, 5, 50, "i0", false);
                    ComboBox op = new ComboBox();
                    setOpComboBox(op, 5, 155, "i1");
                    ComboBox right_var = new ComboBox();
                    setOpVComboBox(right_var, 5, 215, "i2", false);

                    MaskedTextBox xtextbox = new MaskedTextBox();
                    xtextbox.Size = new Size(80, 22);
                    xtextbox.Top += 33;
                    xtextbox.Left += 60;
                    xtextbox.Text = mif.name;
                    xtextbox.TextChanged += new EventHandler(iftext);

                    left_var.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    op.SelectedIndexChanged += new EventHandler(opVar_Handle);
                    right_var.SelectedIndexChanged += new EventHandler(opVar_Handle);

                    Framelist.Controls.Add(lstmt);
                    Framelist.Controls.Add(rstmt);
                    Framelist.Controls.Add(gstmt);
                    Framelist.Controls.Add(left_var);
                    Framelist.Controls.Add(op);
                    Framelist.Controls.Add(right_var);
                    Framelist.Controls.Add(xtextbox);

                    this.hint_richTextBox.Text = Main_lang_dic["hint16"];
                    xtextbox.SelectionStart = xtextbox.Text.Length;
                    xtextbox.Focus();
                }
                last_motionlist_idx = Motionlist.SelectedIndex;
            }
        }

        private void delaytext_TextChanged(object sender, EventArgs e)
        {
            if (Motionlist.SelectedItem != null && String.Compare(delaytext.Text,"") != 0)
            {
                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Frame)
                {
                    ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay = int.Parse(((MaskedTextBox)sender).Text);
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Delay)
                {
                    ((ME_Delay)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay = int.Parse(((MaskedTextBox)sender).Text);
                }
            }
        }

        private void delaytext_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
            {
                e.Handled = true;
            }
        }

        private void Motionlist_MouseDown(object sender, MouseEventArgs e) // right-click for editing motionlist
        {
            if (e.Button == MouseButtons.Right && MotionCombo.SelectedItem != null)
            {
                Motionlist.SelectedIndex = Motionlist.IndexFromPoint(e.X, e.Y);
                if (Motionlist.SelectedItem == null)
                {
                    last_motionlist_idx = -1;
                    motionToolStripMenuItem.Text = Main_lang_dic["AddNewAction_F"];
                    Motionlist_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { motionToolStripMenuItem });
                    Motionlist_contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    Motionlist_contextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    Motionlist_contextMenuStrip.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                    Framelist.Enabled = false;
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Frame)
                {
                    motionToolStripMenuItem.Text = Main_lang_dic["AddNewAction_N"];
                    Motionlist_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { motionToolStripMenuItem });

                    if ((((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count - 1) > Motionlist.SelectedIndex)
                        if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1] is ME_Frame)
                            Motionlist_contextMenuStrip.Items.Add(Main_lang_dic["InsertIntermediateFrame"]);

                    for (int i = 2; i < motionevent.Length - 2; i++)
                        Motionlist_contextMenuStrip.Items.Add(motionevent[i]);
                    Motionlist_contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    Motionlist_contextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    Motionlist_contextMenuStrip.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                }
                else if (Motionlist.SelectedItem != null)
                {
                    motionToolStripMenuItem.Text = Main_lang_dic["AddNewAction_N"];
                    Motionlist_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { motionToolStripMenuItem });
                    for (int i = 2; i < motionevent.Length - 3; i++)
                        Motionlist_contextMenuStrip.Items.Add(motionevent[i]);
                    Motionlist_contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    Motionlist_contextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    Motionlist_contextMenuStrip.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                }
            }
        }

        private void Motionlist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (current_motionlist_idx != -1)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(Motionlist.SelectedIndex);
                    Motionlist.Items.Remove(Motionlist.SelectedItem);
                    delaytext.Enabled = false;
                    delaytext.Text = "";
                    capturebutton.Enabled = false;
                    autocheck.Enabled = false;
                    Framelist.Controls.Clear();
                    Framelist.Enabled = false;
                    update_motionlist();
                    draw_background();
                    last_motionlist_idx = -1;
                }
            }
        }

        private void Motionlistevent(object sender, ToolStripItemClickedEventArgs e)
        {
            int n;
            for (int i = 0; i < motionevent.Length; i++)
            {
                if (String.Compare(e.ClickedItem.Text, motionevent[i]) == 0)
                {
                    switch (i)
                    {
                        case 0:
                            break;
                        case 1:
                            ME_Frame h = new ME_Frame();
                            h.type = 0;
                            for (int j = 0; j < 45; j++)
                                h.frame[j] = (int)homeframe[j];
                            h.delay = default_delay;
                            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Home] " + homecount++);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, h);
                            Motionlist.SelectedIndex++;
                            break;
                        case 2:
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(Motionlist.SelectedIndex);
                            Motionlist.Items.Remove(Motionlist.SelectedItem);
                            delaytext.Enabled = false;
                            delaytext.Text = "";
                            capturebutton.Enabled = false;
                            autocheck.Enabled = false;
                            Framelist.Controls.Clear();
                            Framelist.Enabled = false;
                            update_motionlist();
                            draw_background();
                            last_motionlist_idx = -1;
                            break;
                        case 3:
                            Framelist.Enabled = false;
                            n = Motionlist.SelectedIndex;
                            if (n == 0)
                                break;
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n - 1, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n + 1);
                            Motionlist.Items.Insert(n - 1, Motionlist.SelectedItem);
                            Motionlist.Items.RemoveAt(n + 1);
                            Motionlist.SelectedIndex = n - 1;
                            break;
                        case 4:
                            Framelist.Enabled = false;
                            n = Motionlist.SelectedIndex;
                            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count <= n + 1)
                                break;
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n + 2, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n);
                            Motionlist.Items.Insert(n + 2, Motionlist.SelectedItem);
                            Motionlist.Items.RemoveAt(n);
                            Motionlist.SelectedIndex = n + 1;
                            break;
                        case 5:
                            Framelist.Enabled = false;
                            if (((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).type == 1)
                            {
                                Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Frame] " + MotionCombo.SelectedItem.ToString() + "-" + framecount++.ToString());
                                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());
                                Array.Copy(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame, ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame, 45);
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay;
                                Motionlist.SelectedIndex++;
                            }
                            else if (((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).type == 0)
                            {
                                Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Home] " + homecount++.ToString());
                                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).type = 0;
                                Array.Copy(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame, ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame, 45);
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay;
                                Motionlist.SelectedIndex++;
                            }
                            break;
                        case 6:
                            break;
                        case 7:
                            Framelist.Enabled = false;
                            ME_Frame f1 = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]);
                            ME_Frame f2 = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]);
                            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Frame] " + MotionCombo.SelectedItem.ToString() + "-" + framecount++.ToString());
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());
                            for (int j = 0; j < 45; j++)
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame[j] = (f1.frame[j] + f2.frame[j]) / 2;
                            ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = default_delay;
                            Motionlist.SelectedIndex++;
                            break;
                    }
                }
            }
        }

        private void Motionlistcloseevent(object sender, EventArgs e)
        {
            Motionlist_contextMenuStrip.Items.Clear();
            Motionlist_contextMenuStrip.ItemClicked -= Motionlistevent;
            Motionlist_contextMenuStrip.Closed -= Motionlistcloseevent;
        }

        private void ifToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_If mif = new ME_If();
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[If] v0=v0 goto");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, mif);
            Motionlist.SelectedIndex++;
        }

        private void operandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Compute op = new ME_Compute();
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Compute] v0=v0+v0");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, op);
            Motionlist.SelectedIndex++;
        }

        private void releaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Release r = new ME_Release();
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Release]");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, r);
            Motionlist.SelectedIndex++;
        }

        private void frameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Frame f = new ME_Frame();
            f.type = 1;
            for (int j = 0; j < 45; j++)
                f.frame[j] = (int)homeframe[j];
            f.delay = default_delay;
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Frame] " + ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).name + "-" + framecount++);
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, f);
            Motionlist.SelectedIndex++;
        }

        private void flagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Flag f = new ME_Flag();
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Flag]");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, f);
            Motionlist.SelectedIndex++;
        }

        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Goto g = new ME_Goto();
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Goto]");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, g);
            Motionlist.SelectedIndex++;
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Frame h = new ME_Frame();
            h.type = 0;
            for (int j = 0; j < 45; j++)
                h.frame[j] = (int)homeframe[j];
            h.delay = default_delay;
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Home] " + homecount++);
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, h);
            Motionlist.SelectedIndex++;
        }

        private void delayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Delay d = new ME_Delay();
            d.delay = default_delay;
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Delay]");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, d);
            Motionlist.SelectedIndex++;
        }

        private void triggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ME_Trigger t = new ME_Trigger();
            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[GotoMotion]");
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, t);
            Motionlist.SelectedIndex++;
        }

        private void NewMotion_Click(object sender, EventArgs e)
        {
            MotionName motionName = new MotionName(Main_lang_dic, "", "Create a New Motion", MotionCombo);
            motionName.ShowDialog();
            if (motionName.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                MotionCombo.Items.Add(motionName.name);
                ME_Motion m = new ME_Motion();
                m.name = motionName.name;
                ME_Motionlist.Add(m);
                MotionCombo.SelectedIndex = MotionCombo.Items.Count - 1;
                Motionlist.Controls.Clear();
                current_motionlist_idx = -1;
                move_up.Enabled = false;
                move_down.Enabled = false;
                MotionConfig.Enabled = true;
                draw_background();
                MotionConfig.SelectedIndex = 0;
                Motionlist.Focus();
                this.hint_richTextBox.Text = Main_lang_dic["hint11"];
            }
            else
                MotionCombo.Focus();
        }

        private void EditMotion_Click(object sender, EventArgs e)
        {
            EditMotion_contextMenuStrip.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
            MotionCombo.Focus();
        }

        private void ImportMotionToolStripMenuItem_Click(object sender, EventArgs e) // load motion
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "rbm files (*.mot)|*.mot";
            dialog.Title = "Open File";
            String filename = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : null;
            if (filename == null)
                return;
            using (StreamReader reader = new StreamReader(filename))
            {
                string[] datas = reader.ReadToEnd().Split(delimiterChars);
                ME_Motion motiontag = new ME_Motion();
                for (int i = 0; i < datas.Length; i++)
                {
                    if (String.Compare(datas[i], "Motion") == 0)
                    {
                        i++;
                        MotionName motionName = new MotionName(Main_lang_dic, datas[i], "Enter the Imported Motion Name", MotionCombo);
                        motionName.ShowDialog();
                        if (motionName.DialogResult == System.Windows.Forms.DialogResult.OK)
                            motiontag.name = motionName.name;
                        else
                            return;
                        int try_out;
                        double try_out_d;
                        if (String.Compare("frame", datas[i + 1]) != 0 && String.Compare("home", datas[i + 1]) != 0 &&
                            String.Compare("delay", datas[i + 1]) != 0 && String.Compare("flag", datas[i + 1]) != 0 &&
                            String.Compare("goto", datas[i + 1]) != 0 && String.Compare("MotionEnd", datas[i + 1]) != 0 &&
                            int.TryParse(datas[i + 1], out try_out))
                        { // triggers
                            motiontag.trigger_method = int.Parse(datas[++i]);
                            motiontag.auto_method = int.Parse(datas[++i]);
                            motiontag.trigger_key = int.Parse(datas[++i]);
                            motiontag.trigger_keyType = int.Parse(datas[++i]);
                            i++;
                            if (String.Compare("---noBtKey---", datas[i]) == 0)
                                motiontag.bt_key = "";
                            else
                                motiontag.bt_key = datas[i];
                            if (int.TryParse(datas[++i], out try_out) == false)
                                i--;
                            motiontag.ps2_key = datas[++i];
                            motiontag.ps2_type = int.Parse(datas[++i]);
                            if (String.Compare("frame", datas[i + 1]) != 0 && String.Compare("home", datas[i + 1]) != 0 &&
                                String.Compare("delay", datas[i + 1]) != 0 && String.Compare("flag", datas[i + 1]) != 0 &&
                                String.Compare("goto", datas[i + 1]) != 0 && String.Compare("MotionEnd", datas[i + 1]) != 0 &&
                                String.Compare("Layer", datas[i + 1]) != 0)
                                motiontag.bt_mode = datas[++i];
                            if (double.TryParse(datas[i + 1], out try_out_d))
                            {
                                motiontag.acc_Settings[0] = double.Parse(datas[++i]);
                                motiontag.acc_Settings[1] = double.Parse(datas[++i]);
                                motiontag.acc_Settings[2] = double.Parse(datas[++i]);
                                motiontag.acc_Settings[3] = double.Parse(datas[++i]);
                                motiontag.acc_Settings[4] = double.Parse(datas[++i]);
                                motiontag.acc_Settings[5] = double.Parse(datas[++i]);
                                motiontag.acc_Settings[6] = int.Parse(datas[++i]);
                            }
                            if (int.TryParse(datas[i + 1], out try_out))
                            {
                                motiontag.wifi602_key = int.Parse(datas[++i]);
                            }
                            if (int.TryParse(datas[i + 1], out try_out))
                            {
                                motiontag.analog_pin = int.Parse(datas[++i]);
                                motiontag.analog_cond = int.Parse(datas[++i]);
                                motiontag.analog_value = int.Parse(datas[++i]);
                            }
                        }
                        ME_Motionlist.Add(motiontag);
                    }
                    else if (String.Compare(datas[i], "Layer") == 0)
                    {
                        i++;
                        int try_out;
                        if (int.TryParse(datas[i], out try_out) == true)
                            motiontag.moton_layer = try_out;
                        while (String.Compare("frame", datas[i + 1]) != 0 && String.Compare("home", datas[i + 1]) != 0 &&
                            String.Compare("delay", datas[i + 1]) != 0 && String.Compare("flag", datas[i + 1]) != 0 &&
                            String.Compare("goto", datas[i + 1]) != 0 && String.Compare("MotionEnd", datas[i + 1]) != 0 &&
                            int.TryParse(datas[i + 1], out try_out))
                        {
                            i++;
                            motiontag.used_servos.Add(try_out);
                        }
                    }
                    else if (String.Compare(datas[i], "MotionEnd") == 0)
                    {
                        int try_out;
                        i++;
                        if (i < datas.Length && int.TryParse(datas[i], out try_out) == true)
                            motiontag.property = try_out;
                        else
                            i--;
                        i++;
                        if (i < datas.Length && int.TryParse(datas[i], out try_out) == true)
                            motiontag.comp_range = try_out;
                        else
                            i--;
                        i++;
                        if (i < datas.Length && int.TryParse(datas[i], out try_out) == true)
                            motiontag.control_method = try_out;
                        else
                            i--;
                        if (motiontag != null)
                        {
                            MotionCombo.Items.Add(motiontag.name);
                            MotionCombo.SelectedIndex = MotionCombo.Items.Count - 1;
                        }
                    }
                    else if (String.Compare(datas[i], "frame") == 0)
                    {
                        ME_Frame nframe = new ME_Frame();
                        nframe.type = 1;
                        i++;
                        try
                        {
                            nframe.delay = int.Parse(datas[i]);
                        }
                        catch
                        {
                            nframe.delay = default_delay;
                            MessageBox.Show(Main_lang_dic["errorMsg10"]);
                        }
                        int j = 0;
                        while (j < 45)
                        {
                            if (String.Compare(Motion.fbox[j].SelectedItem.ToString(), "---noServo---") != 0)
                            {
                                i++;
                                try
                                {
                                    nframe.frame[j] = int.Parse(datas[i]);
                                }
                                catch
                                {
                                    nframe.frame[j] = 0;
                                    MessageBox.Show(Main_lang_dic["errorMsg10"]);
                                }
                            }
                            else
                            {
                                nframe.frame[j] = 0;
                            }
                            j++;
                        }
                        motiontag.Events.Add(nframe);
                    }
                    else if (String.Compare(datas[i], "home") == 0)
                    {
                        ME_Frame nframe = new ME_Frame();
                        nframe.type = 0;
                        i++;
                        try
                        {
                            nframe.delay = int.Parse(datas[i]);
                        }
                        catch
                        {
                            nframe.delay = default_delay;
                            MessageBox.Show(Main_lang_dic["errorMsg10"]);
                        }
                        int j = 0;
                        while (j < 45)
                        {
                            if (String.Compare(Motion.fbox[j].SelectedItem.ToString(), "---noServo---") != 0)
                            {
                                i++;
                                try
                                {
                                    nframe.frame[j] = int.Parse(datas[i]);
                                }
                                catch
                                {
                                    nframe.frame[j] = 0;
                                    MessageBox.Show(Main_lang_dic["errorMsg10"]);
                                }
                            }
                            else
                            {
                                nframe.frame[j] = 0;
                            }
                            j++;
                        }
                        motiontag.Events.Add(nframe);
                    }
                    else if (String.Compare(datas[i], "delay") == 0)
                    {
                        ME_Delay ndelay = new ME_Delay();
                        i++;
                        try
                        {
                            ndelay.delay = int.Parse(datas[i]);
                        }
                        catch
                        {
                            ndelay.delay = default_delay;
                            MessageBox.Show(Main_lang_dic["errorMsg11"]);
                        }
                        motiontag.Events.Add(ndelay);
                    }
                    else if (String.Compare(datas[i], "flag") == 0)
                    {
                        ME_Flag nflag = new ME_Flag();
                        i++;
                        nflag.name = datas[i];
                        motiontag.Events.Add(nflag);
                    }
                    else if (String.Compare(datas[i], "goto") == 0)
                    {
                        ME_Goto ngoto = new ME_Goto();
                        i++;
                        ngoto.name = datas[i];
                        i++;
                        if (String.Compare(datas[i], "True") == 0)
                            ngoto.is_goto = true;
                        else
                            ngoto.is_goto = false;
                        i++;
                        int value;
                        bool success = int.TryParse(datas[i], out value);
                        if (!success)
                        {
                            i--;
                            motiontag.Events.Add(ngoto);
                            continue;
                        }
                        ngoto.loops = datas[i];
                        ngoto.current_loop = value;
                        i++;
                        if (String.Compare(datas[i], "True") == 0)
                            ngoto.infinite = true;
                        else if (String.Compare(datas[i], "False") == 0)
                            ngoto.infinite = false;
                        else
                            i--;

                        motiontag.Events.Add(ngoto);
                    }
                    else if (String.Compare(datas[i], "trigger") == 0)
                    {
                        ME_Trigger ntr = new ME_Trigger();
                        ntr.name = datas[++i];
                        ntr.method = int.Parse(datas[++i]);
                        motiontag.Events.Add(ntr);
                    }
                    else if (String.Compare(datas[i], "release") == 0)
                    {
                        ME_Release nr = new ME_Release();
                        motiontag.Events.Add(nr);
                    }
                    else if (String.Compare(datas[i], "compute") == 0)
                    {
                        ME_Compute op = new ME_Compute();
                        op.left_var = int.Parse(datas[++i]);
                        op.form = int.Parse(datas[++i]);
                        op.f1_var1 = int.Parse(datas[++i]);
                        op.f1_op = int.Parse(datas[++i]);
                        op.f1_var2 = int.Parse(datas[++i]);
                        op.f2_op = int.Parse(datas[++i]);
                        op.f2_var = int.Parse(datas[++i]);
                        op.f3_var = int.Parse(datas[++i]);
                        op.f4_const = double.Parse(datas[++i]);
                        motiontag.Events.Add(op);
                    }
                    else if (String.Compare(datas[i], "if") == 0)
                    {
                        ME_If mif = new ME_If();
                        mif.left_var = int.Parse(datas[++i]);
                        mif.method = int.Parse(datas[++i]);
                        mif.right_var = int.Parse(datas[++i]);
                        mif.name = datas[++i];
                        motiontag.Events.Add(mif);
                    }
                }
            }
        }

        private void ExportMotionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MotionCombo.Items.Count > 0)
            {
                if (MotionCombo.SelectedItem != null)
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.Filter = "mot files (*.mot)|*.mot";
                    dialog.Title = "Save File";
                    dialog.FileName = ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).name;
                    if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != null)
                    {
                        TextWriter writer = new StreamWriter(dialog.FileName);
                        saveMotion(((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]), writer);
                        writer.Dispose();
                        writer.Close();
                    }
                }
            }
        }

        private void RenameMotionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MotionCombo.Items.Count > 0)
            {
                if (MotionCombo.SelectedItem != null)
                {
                    MotionName motionName = new MotionName(Main_lang_dic, MotionCombo.Text, "Rename the Motion", MotionCombo);
                    motionName.ShowDialog();
                    if (motionName.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).name = motionName.name;
                        MotionCombo.Items[MotionCombo.SelectedIndex] = motionName.name;
                        RenewGotoMotion();
                        update_motionlist();
                        if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count > 0)
                            Motionlist.SelectedIndex = 0;
                    }
                }
            }
        }

        private void DeleteMotionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MotionCombo.Items.Count > 0)
            {
                if (MotionCombo.SelectedItem != null)
                {
                    DialogResult res = MessageBox.Show(Main_lang_dic["warning1"], "Confirm", MessageBoxButtons.YesNo);
                    if (res == System.Windows.Forms.DialogResult.Yes)
                    {
                        Motionlist.Items.Clear();
                        Motionlist.Controls.Clear();
                        ME_Motionlist.Remove(ME_Motionlist[MotionCombo.SelectedIndex]);
                        MotionCombo.Items.Remove(MotionCombo.SelectedItem);
                        RenewGotoMotion();
                        if (MotionCombo.Items.Count > 0)
                            MotionCombo.SelectedIndex = 0;
                        else
                            Framelist.Controls.Clear();
                    }
                }
            }
        }

        private void DuplicateMotionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MotionCombo.Items.Count > 0)
            {
                if (MotionCombo.SelectedItem != null)
                {
                    MotionName motionName = new MotionName(Main_lang_dic, MotionCombo.Text + "_new", "Duplicate the Motion", MotionCombo);
                    motionName.ShowDialog();
                    if (motionName.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        ME_Motion m = ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Copy();
                        m.name = motionName.name;
                        MotionCombo.Items.Add(motionName.name);
                        ME_Motionlist.Add(m);
                        MotionCombo.SelectedIndex = MotionCombo.Items.Count - 1;
                    }
                }
            }
        }

        private void RenewGotoMotion()
        {
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_Trigger)
                    {
                        ME_Trigger t = (ME_Trigger)m.Events[j];
                        bool has_target = false;
                        for (int k = 0; k < ME_Motionlist.Count; k++)
                            if (((ME_Motion)ME_Motionlist[k]).name == t.name)
                                has_target = true;
                        if (has_target == false)
                            t.name = "";
                    }
                }
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool close = true;
            if (needToSave() && File.Exists(load_filename))
            {
                DialogResult dialogResult = MessageBox.Show(Main_lang_dic["saveMsg"], "Exit", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                {
                    save_project(load_filename);
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    close = false;
                    e.Cancel = true;
                }
            }
            else if (needToSave())
            {
                DialogResult dialogResult = MessageBox.Show(Main_lang_dic["saveMsg2"], "Exit", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                {
                    saveFileToolStripMenuItem_Click(sender, e);
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    close = false;
                    e.Cancel = true;
                }
            }
            if (close)
            {
                if (arduino != null)
                    arduino.Close();
            }
        }

        private void capturebutton_Click(object sender, EventArgs e)
        {
            if (string.Compare(com_port, "OFF") != 0)
            {
                capturebutton.Enabled = false;
                autocheck.Checked = false;
                uint[] frame = new uint[45];
                if (servo_captured() == false)
                {
                    this.hint_richTextBox.Text = Main_lang_dic["hint12"];
                }
                else
                {
                    arduino.motor_release();
                    Thread.Sleep(100);
                    for (int i = 0; i < 45; i++)
                    {
                        if (captured[i])
                        {
                            arduino.frame_capture(i);
                            Thread.Sleep(100);
                            ftext[i].Text = arduino.captured_data.ToString();
                        }
                    }
                }
                capturebutton.Enabled = true;
            }
        }

        private bool servo_captured()
        {
            bool can_cap = false;
            for (int i = 0; i < 45; i++)
            {
                if (String.Compare(Motion.fbox[i].Text, "KONDO_KRS786") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "KONDO_KRS788") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "KONDO_KRS78X") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "KONDO_KRS4014") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "KONDO_KRS4024") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "HITEC_HSR8498") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "FUTABA_S3003") == 0)
                    captured[i] = true;
                else if (String.Compare(Motion.fbox[i].Text, "SHAYYE_SYS214050") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "TOWERPRO_MG90S") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "TOWERPRO_MG995") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "TOWERPRO_MG996") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "TOWERPRO_SG90") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "DMP_RS0263") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "DMP_RS1270") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "GWS_S777") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "GWS_S03T") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "GWS_MICRO") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "EMAX_ES08AII") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "EMAX_ES3104") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "OtherServos") == 0)
                    captured[i] = false;

                if (captured[i] == true)
                    can_cap = true;
            }
            return can_cap;
        }

        public void MotionOnTest(ME_Motion m)
        {
            optionsToolStripMenuItem.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            fileToolStripMenuItem.Enabled = false;
            actionToolStripMenuItem.Enabled = false;

            saveFrame.Enabled = false;
            loadFrame.Enabled = false;
            move_down.Enabled = false;
            move_up.Enabled = false;
            MotionConfig.Enabled = false;
            MotionCombo.Enabled = false;
            MotionTest.Enabled = false;
            motion_pause.Enabled = true;
            motion_stop.Enabled = true;
            autocheck.Enabled = false;
            capturebutton.Enabled = false;
            Framelist.Enabled = false;
            NewMotion.Enabled = false;
            EditMotion.Enabled = false;
            if (ME_Motionlist == null)
                return;

            for (int j = mtest_start_pos; j < m.Events.Count; j++)
            {
                if (MotionConfig.SelectedIndex == 0)
                    Motionlist.SelectedIndex = j;
                if (motiontest_state == (int)mtest_states.stop)
                    break;
                if (motiontest_state == (int)mtest_states.pause)
                {
                    mtest_start_pos = j;
                    MotionTest.Enabled = true;
                    motion_pause.Enabled = false;
                    motion_stop.Enabled = true;
                    this.hint_richTextBox.Text =
                            "   ___   __   ____        _\n" +
                            "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                            "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                            " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                            "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
                    return;
                }
                if (m.Events[j] is ME_Frame)
                {
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        {
                            autoframe[i] = (((ME_Frame)m.Events[j]).frame[i] + offset[i]);
                            if (m.moton_layer != 0 && !m.used_servos.Contains(i))
                                autoframe[i] = 0;
                        }
                    }
                    if (string.Compare(com_port, "OFF") != 0)
                    {
                        try
                        {
                            arduino.frameWrite(0x6F, autoframe, (int)((ME_Frame)m.Events[j]).delay);
                            Thread.Sleep((int)((ME_Frame)m.Events[j]).delay);
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show(Main_lang_dic["errorMsg1"]);
                        }
                    }
                }
                else if (m.Events[j] is ME_Delay)
                {
                    Thread.Sleep((int)((ME_Delay)m.Events[j]).delay);
                }
                else if (m.Events[j] is ME_Goto)
                {
                    if (((ME_Goto)m.Events[j]).is_goto &&
                        (((ME_Goto)m.Events[j]).current_loop > 0 ||
                        ((ME_Goto)m.Events[j]).infinite))
                    {
                        if (((ME_Goto)m.Events[j]).infinite == false)
                            ((ME_Goto)m.Events[j]).current_loop--;
                        for (int k = 0; k < m.Events.Count; k++)
                        {
                            if (m.Events[k] is ME_Flag)
                            {
                                if (String.Compare(((ME_Goto)m.Events[j]).name, ((ME_Flag)m.Events[k]).name) == 0)
                                {
                                    j = k;
                                    break;
                                }
                            }
                        }
                    }
                    else if (((ME_Goto)m.Events[j]).current_loop == 0)
                    {
                        int loop_num = int.Parse(((ME_Goto)m.Events[j]).loops);
                        ((ME_Goto)m.Events[j]).current_loop = loop_num;
                    }
                }
                else if (m.Events[j] is ME_Release)
                {
                    if (string.Compare(com_port, "OFF") != 0)
                        arduino.motor_release();
                }
                else if (m.Events[j] is ME_Compute)
                {
                    ME_Compute op = (ME_Compute)m.Events[j];
                    switch(op.form)
                    {
                        case 0:
                            operand_var[op.left_var] = opOperate(opVal(op.f1_var1), opVal(op.f1_var2), 0, op.f1_op);
                            break;
                        case 1:
                            operand_var[op.left_var] = opOperate(0, opVal(op.f2_var), 1, op.f2_op);
                            break;
                        case 2:
                            operand_var[op.left_var] = opVal(op.f3_var);
                            break;
                        case 3:
                            operand_var[op.left_var] = (float)op.f4_const;
                            break;
                        default:
                            break;
                    }
                }
                else if (m.Events[j] is ME_If)
                {
                    ME_If mif = (ME_If)m.Events[j];
                    if (ifResult(mif.left_var, mif.right_var, mif.method))
                    {
                        for (int k = 0; k < m.Events.Count; k++)
                        {
                            if (m.Events[k] is ME_Flag)
                            {
                                if (String.Compare(((ME_If)m.Events[j]).name, ((ME_Flag)m.Events[k]).name) == 0)
                                {
                                    j = k;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            
            mtest_start_pos = 0;

            optionsToolStripMenuItem.Enabled = true;
            saveFileToolStripMenuItem.Enabled = true;
            fileToolStripMenuItem.Enabled = true;
            actionToolStripMenuItem.Enabled = true;

            saveFrame.Enabled = true;
            loadFrame.Enabled = true;
            motion_pause.Enabled = false;
            motion_stop.Enabled = false;
            MotionConfig.Enabled = true;
            MotionTest.Enabled = true;
            MotionCombo.Enabled = true;
            NewMotion.Enabled = true;
            EditMotion.Enabled = true;

            for (int j = 0; j < m.Events.Count; j++)
            {
                if (m.Events[j] is ME_Goto)
                {
                    int loops = int.Parse(((ME_Goto)m.Events[j]).loops);
                    ((ME_Goto)m.Events[j]).current_loop = loops;
                }
            }
            hint_richTextBox.Text =
                    "   ___   __   ____        _\n" +
                    "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                    "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                    " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                    "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
        }

        private void MotionTest_Click(object sender, EventArgs e)
        {
            delayUnitLabel.Visible = false;
            DelayLabel.Visible = false;
            delaytext.Visible = false;
            saveFrame.Visible = false;
            loadFrame.Visible = false;
            MotionConfig.SelectedIndex = 0;
            Motionlist.Focus();
            Framelist.Controls.Clear();
            freshflag[1] = false;
            last_motionlist_idx = -1;
            motiontest_state = (int)mtest_states.start;
            Thread t = new Thread(() => MotionOnTest(((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex])));
            t.IsBackground = true;
            t.Start();
            draw_background();
        }

        private void motion_pause_Click(object sender, EventArgs e)
        {
            motiontest_state = (int)mtest_states.pause;
        }

        private void motion_stop_Click(object sender, EventArgs e)
        {
            if (motiontest_state == (int)mtest_states.pause)
            {
                mtest_start_pos = 0;
                optionsToolStripMenuItem.Enabled = true;
                saveFileToolStripMenuItem.Enabled = true;
                fileToolStripMenuItem.Enabled = true;
                actionToolStripMenuItem.Enabled = true;
                saveFrame.Enabled = true;
                loadFrame.Enabled = true;
                MotionTest.Enabled = true;
                motion_pause.Enabled = false;
                motion_stop.Enabled = false;
                Framelist.Enabled = false;
                MotionCombo.Enabled = true;
                MotionConfig.Enabled = true;
                NewMotion.Enabled = true;
                EditMotion.Enabled = true;
                this.hint_richTextBox.Text =
                        "   ___   __   ____        _\n" +
                        "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                        "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                        " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                        "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
            }
            else
                motiontest_state = (int)mtest_states.stop;
        }

        private void autocheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist.Count == 0)
                return;
            ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
            bool active_autocheck = false;
            if (MotionCombo.SelectedItem != null)
                if (ME_Motionlist != null)
                    if (m.Events.Capacity - 1 >= Motionlist.SelectedIndex && Motionlist.SelectedIndex >= 0)
                        if (m.Events[Motionlist.SelectedIndex] is ME_Frame || m.Events[Motionlist.SelectedIndex] is ME_Release)
                            active_autocheck = true;

            if (String.Compare(delaytext.Text, "") == 0)
                delaytext.Text = default_delay.ToString();
            if (active_autocheck)
            {
                if (autocheck.Checked == true && int.Parse(delaytext.Text) < 0)
                {
                    MessageBox.Show(Main_lang_dic["errorMsg15"]);
                    autocheck.Checked = false;
                }
                else if (autocheck.Checked == true)
                {
                    if (m.Events[Motionlist.SelectedIndex] is ME_Release)
                    {
                        if (string.Compare(com_port, "OFF") != 0)
                            arduino.motor_release();
                    }
                    else
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                autoframe[i] = (int.Parse(ftext[i].Text) + offset[i]);
                                if (m.moton_layer != 0 && !m.used_servos.Contains(i))
                                    autoframe[i] = 0;
                            }
                            else
                                autoframe[i] = 0;
                        }

                        autocheck.Enabled = false;

                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, int.Parse(delaytext.Text), servo_onOff);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show(Main_lang_dic["errorMsg1"]);
                            }
                        }
                        autocheck.Enabled = true;
                    }
                }
            }
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.86duino.com/index.php?p=11544&lang=TW");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about a = new about(Main_lang_dic);
            a.ShowDialog();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (ME_Motionlist == null)
                return;
            if (ME_Motionlist.Count == 0)
            {
                MessageBox.Show(Main_lang_dic["errorMsg16"]);
                return;
            }
            FSMGen g = new FSMGen(Motion, offset, ME_Motionlist, gs);
            g.generate_withFiles();
        }

        private void GenerateAllInOne_Click(object sender, EventArgs e)
        {
            if (ME_Motionlist == null)
                return;
            if (ME_Motionlist.Count == 0)
            {
                MessageBox.Show(Main_lang_dic["errorMsg16"]);
                return;
            }
            FSMGen g = new FSMGen(Motion, offset, ME_Motionlist, gs);
            g.generate_AllinOne();
        }

        private void draw_background()
        {
            if(Motion.picfilename != null)
            {
                try
                {
                    Robot_pictureBox.Image = Image.FromFile(Motion.picfilename);
                }
                catch
                {
                    MessageBox.Show(Main_lang_dic["errorMsg17"]);
                }
            }
            Framelist.Controls.Add(Robot_pictureBox);
        }

        private void update_motionlist()
        {
            Action_groupBox.Enabled = false;
            Setting_groupBox.Enabled = false;
            Motionlist.Items.Clear();
            framecount = 0;
            homecount = 0;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (MotionCombo.SelectedItem == null)
                    break;
                if (String.Compare(MotionCombo.SelectedItem.ToString(), m.name.ToString()) == 0)
                {
                    for (int j = 0; j < m.Events.Count; j++)
                    {
                        if (m.Events[j] is ME_Frame)
                        {
                            if (((ME_Frame)m.Events[j]).type == 1)
                            {
                                Motionlist.Items.Add("[Frame] " + ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).name + "-" + framecount);
                                framecount++;
                            }
                            else if (((ME_Frame)m.Events[j]).type == 0)
                            {
                                Motionlist.Items.Add("[Home] " + homecount);
                                homecount++;
                                for (int k = 0; k < 45; k++)
                                {
                                    if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                                    {
                                        ((ME_Frame)m.Events[j]).frame[k] = (int)homeframe[k];
                                    }
                                }
                            }
                        }
                        else if (m.Events[j] is ME_Delay)
                        {
                            Motionlist.Items.Add("[Delay]");
                        }
                        else if (m.Events[j] is ME_Goto)
                        {
                            ME_Goto g = (ME_Goto)m.Events[j];
                            Motionlist.Items.Add("[Goto] " + g.name);
                        }
                        else if (m.Events[j] is ME_Flag)
                        {
                            ME_Flag fl = (ME_Flag)m.Events[j];
                            Motionlist.Items.Add("[Flag] " + fl.name);
                        }
                        else if (m.Events[j] is ME_Trigger)
                        {
                            ME_Trigger t = (ME_Trigger)m.Events[j];
                            Motionlist.Items.Add("[GotoMotion] " + t.name);
                        }
                        else if (m.Events[j] is ME_Release)
                        {
                            Motionlist.Items.Add("[Release]");
                        }
                        else if (m.Events[j] is ME_If)
                        {
                            ME_If mif = (ME_If)m.Events[j];
                            Motionlist.Items.Add("[If] " + convertIndex2Str(mif.left_var, 0) + convertIndex2Str(mif.method, 3) +
                                                 convertIndex2Str(mif.right_var, 0) + " goto " + mif.name);
                        }
                        else if (m.Events[j] is ME_Compute)
                        {
                            ME_Compute op = (ME_Compute)m.Events[j];
                            Motionlist.Items.Add("[Compute] " + Operand2Text(op));
                        }
                    }
                    break;
                }
            }
            MotionTest.Enabled = true;
            motion_pause.Enabled = false;
            motion_stop.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void motorRelease_Click(object sender, EventArgs e)
        {
            if (string.Compare(com_port, "OFF") != 0)
            {
                arduino.motor_release();
                autocheck.Checked = false;
                Thread.Sleep(100);
            }
        }

        private void motionlist_up(object sender, EventArgs e)
        {
            int n = current_motionlist_idx;
            Motionlist.SelectedIndex = n;
            if (n == 0 || n == -1)
                return;
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n - 1, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n + 1);
            Motionlist.Items.Insert(n - 1, Motionlist.SelectedItem);
            Motionlist.Items.RemoveAt(n + 1);
            freshflag[1] = true;
            Motionlist.SelectedIndex = n - 1;
        }

        private void motionlist_down(object sender, EventArgs e)
        {
            int n = current_motionlist_idx;
            Motionlist.SelectedIndex = n;
            if ((((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count <= n + 1) || n == -1)
                return;
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n + 2, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n);
            Motionlist.Items.Insert(n + 2, Motionlist.SelectedItem);
            Motionlist.Items.RemoveAt(n);
            freshflag[1] = true;
            Motionlist.SelectedIndex = n + 1;
        }

        private void enableTriggerGroup(GroupBox gb)
        {
            Always_groupBox.Enabled = false;
            Keyboard_groupBox.Enabled = false;
            bt_groupBox.Enabled = false;
            ps2_groupBox.Enabled = false;
            acc_groupBox.Enabled = false;
            wifi602_groupBox.Enabled = false;
            analog_groupBox.Enabled = false;
            gb.Enabled = true;
        }

        private void MotionConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(MotionConfig.SelectedIndex == 0)
            {
                if (Motionlist.SelectedItem != null && (MotionTest.Enabled))
                {
                    string[] datas = Motionlist.SelectedItem.ToString().Split(' ');
                    if (String.Compare(datas[0], "[Frame]") == 0)
                    {
                        loadFrame.Visible = true;
                        saveFrame.Visible = true;
                    }
                }
                autocheck.Enabled = true;
                capturebutton.Enabled = true;
                delaytext.Enabled = true;
                last_motionlist_idx = -1;
                if (current_motionlist_idx != -1)
                {
                    Framelist.Enabled = true;
                    int tmp_motionlist_idx = current_motionlist_idx;
                    Motionlist.SelectedIndex = -1;
                    Motionlist.SelectedIndex = tmp_motionlist_idx;
                }
                else
                {
                    Framelist.Enabled = false;
                }
                this.hint_richTextBox.Text =
                    "   ___   __   ____        _\n" +
                    "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                    "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                    " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                    "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
            }
            else if(MotionConfig.SelectedIndex == 1)
            {
                saveFrame.Visible = false;
                loadFrame.Visible = false;
                move_down.Enabled = false;
                move_up.Enabled = false;
                Framelist.Enabled = false;
                autocheck.Enabled = false;
                capturebutton.Enabled = false;
                delaytext.Enabled = false;
                if (ME_Motionlist == null || MotionCombo.SelectedItem == null)
                {
                    Always_radioButton.Enabled = false;
                    Keyboard_radioButton.Enabled = false;
                    bt_radioButton.Enabled = false;
                    ps2_radioButton.Enabled = false;
                    acc_groupBox.Enabled = false;
                    Always_groupBox.Enabled = false;
                    Keyboard_groupBox.Enabled = false;
                    bt_groupBox.Enabled = false;
                    ps2_groupBox.Enabled = false;
                    acc_groupBox.Enabled = false;
                    wifi602_groupBox.Enabled = false;
                    wifi602_radioButton.Enabled = false;
                    analog_groupBox.Enabled = false;
                    analog_radioButton.Enabled = false;
                }
                else
                {
                    if (Always_radioButton.Checked == true)
                    {
                        enableTriggerGroup(Always_groupBox);
                    }
                    else if (Keyboard_radioButton.Checked == true)
                    {
                        enableTriggerGroup(Keyboard_groupBox);
                    }
                    else if (bt_radioButton.Checked == true)
                    {
                        enableTriggerGroup(bt_groupBox);
                    }
                    else if (ps2_radioButton.Checked == true)
                    {
                        enableTriggerGroup(ps2_groupBox);
                    }
                    else if (acc_radioButton.Checked == true)
                    {
                        enableTriggerGroup(acc_groupBox);
                    }
                    else if (wifi602_radioButton.Checked == true)
                    {
                        enableTriggerGroup(wifi602_groupBox);
                    }
                    else if (analog_radioButton.Checked == true)
                    {
                        enableTriggerGroup(analog_groupBox);
                    }
                    ps2DATCombo.Text = gs.ps2pins[0];
                    ps2CMDCombo.Text = gs.ps2pins[1];
                    ps2ATTCombo.Text = gs.ps2pins[2];
                    ps2CLKCombo.Text = gs.ps2pins[3];
                }
                this.hint_richTextBox.Text = Main_lang_dic["hint9"];
            }
            else if(MotionConfig.SelectedIndex == 2)
            {
                saveFrame.Visible = false;
                loadFrame.Visible = false;
                move_down.Enabled = false;
                move_up.Enabled = false;
                Framelist.Enabled = false;
                autocheck.Enabled = false;
                capturebutton.Enabled = false;
                delaytext.Enabled = false;
                if (ME_Motionlist == null || MotionCombo.SelectedItem == null)
                {
                    Blocking.Enabled = false;
                    NonBlocking.Enabled = false;
                }
                this.hint_richTextBox.Text = Main_lang_dic["hint13"];
            }
        }

        private void NonBlocking_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                if (NonBlocking.Checked == true)
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).property = (int)motion_property.nonblocking;
        }

        private void Blocking_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                if (Blocking.Checked == true)
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).property = (int)motion_property.blocking;
        }

        private void Always_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (Always_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.always;
                    enableTriggerGroup(Always_groupBox);
                }
            }
        }

        private void Keyboard_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (Keyboard_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.keyboard;
                    enableTriggerGroup(Keyboard_groupBox);
                }
            }
        }

        private void bt_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (bt_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.bluetooth;
                    enableTriggerGroup(bt_groupBox);
                }
            }
        }

        private void ps2_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (ps2_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.ps2;
                    enableTriggerGroup(ps2_groupBox);
                }
            }
        }

        private void acc_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (acc_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.acc;
                    enableTriggerGroup(acc_groupBox);
                }
            }
        }

        private void wifi602_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (wifi602_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.wifi602;
                    enableTriggerGroup(wifi602_groupBox);
                }
            }
        }

        private void analog_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                if (analog_radioButton.Checked == true)
                {
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_method = (int)mtest_method.analog;
                    enableTriggerGroup(analog_groupBox);
                }
            }
        }

        private void AlwaysOn_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                if(AlwaysOn.Checked == true)
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).auto_method = (int)auto_method.on;
        }

        private void AlwaysOff_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                if (AlwaysOff.Checked == true)
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).auto_method = (int)auto_method.off;
        }

        private void TitleMotion_CheckedChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                if (TitleMotion.Checked == true)
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).auto_method = (int)auto_method.title;
        }

        private void KeyboardCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_key = KeyboardCombo.SelectedIndex;
        }

        private void KeyboardTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).trigger_keyType = KeyboardTypeCombo.SelectedIndex;
        }

        private void btKeyText_TextChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).bt_key = btKeyText.Text;
                btKeyLabel.Text = "Key: " + btKeyText.Text;
            }
        }

        private void btTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).bt_mode = btModeCombo.Text;
        }

        private void btPortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.bt_port = btPortCombo.Text;
        }

        private void btBaudCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.bt_baud = btBaudCombo.Text;
        }

        private void wifi602PortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.wifi602_port = wifi602PortCombo.Text;
        }

        private void wifi602KeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).wifi602_key = wifi602KeyCombo.SelectedIndex;
        }

        private void analogPinCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).analog_pin = analogPinCombo.SelectedIndex;
        }

        private void analogCondCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).analog_cond = analogCondCombo.SelectedIndex;
        }

        private void ps2DATCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[0] = ps2DATCombo.Text;
        }

        private void ps2CMDCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[1] = ps2CMDCombo.Text;
        }

        private void ps2ATTCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[2] = ps2ATTCombo.Text;
        }

        private void ps2CLKCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[3] = ps2CLKCombo.Text;
        }

        private void ps2KeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).ps2_key = ps2KeyCombo.Text;
        }

        private void ps2TypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).ps2_type = ps2TypeCombo.SelectedIndex;
        }

        private void MotionLayerCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
            {
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).moton_layer = MotionLayerCombo.SelectedIndex;
                freshflag[1] = false;
            }
        }

        private void CompRangeText_TextChanged(object sender, EventArgs e)
        {
            int try_out;
            if (int.TryParse(CompRangeText.Text, out try_out))
            {
                if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                {
                    if (try_out <= 180)
                        ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).comp_range = try_out;
                    else
                    {
                        ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).comp_range = 180;
                        CompRangeText.Text = "180";
                    }
                }
            }
        }

        private void MotionControlCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ME_Motionlist != null && MotionCombo.SelectedItem != null)
                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).control_method = MotionControlCombo.SelectedIndex;
        }

        private void saveFrame_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "86Duino frame file (*.txt)|*.txt";
            dialog.Title = "Save Frame";
            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != null)
            {
                int ch_count = 0;
                TextWriter writer = new StreamWriter(dialog.FileName);
                ME_Frame f = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]);
                for (int k = 0; k < 45; k++)
                {
                    if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                    {
                        writer.Write("channel");
                        writer.Write(ch_count.ToString() + "=");
                        writer.WriteLine(f.frame[k].ToString());
                        ch_count++;
                    }
                }
                writer.Dispose();
                writer.Close();
            }
        }

        private void loadFrame_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "86Duino frame file (*.txt)|*.txt";
            dialog.Title = "Load Frame";
            String filename = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : null;
            if (filename == null)
                return;
            using (StreamReader reader = new StreamReader(filename))
            {
                ME_Frame f = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]);
                int ch_count = 0;
                List<int> ch_mapping = new List<int>();
                for (int k = 0; k < 45; k++)
                {
                    if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                    {
                        ch_mapping.Add(k);
                        ch_count++;
                    }
                }

                while (reader.Peek() >= 0)
                {
                    string[] datas = reader.ReadLine().Split('=', '\n');
                    int channel = 50;
                    for (int i = 0; i < datas.Length; i++)
                    {
                        if (i == 0) //channelX
                        {
                            if (datas[i].Length < 8)
                                break;
                            else if (String.Compare(datas[i].Substring(0, 7), "channel") == 0)
                                if (int.TryParse(datas[i].Substring(7), out channel) == false)
                                    break;
                        }
                        else if (i == 1) //val
                        {
                            int val;
                            if (int.TryParse(datas[i], out val))
                                if (channel != 50 && ch_count > channel)
                                    f.frame[ch_mapping[channel]] = val;
                        }
                    }
                }
            }
            Update_framelist();
        }

        private void applyLang()
        {
            hint_richTextBox.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            nonblockingExplanation.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            blockingExplaination.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            CompRangeExplanation.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            motionLayerExplanation.LanguageOption = RichTextBoxLanguageOptions.DualFont;
            MotionControlExplanation.LanguageOption = RichTextBoxLanguageOptions.DualFont;

            motionevent[0] = Main_lang_dic["AddNewAction_N"];
            motionevent[1] = Main_lang_dic["AddHomeframe"];
            motionevent[2] = Main_lang_dic["DeleteAction"];
            motionevent[3] = Main_lang_dic["MoveActionUP"];
            motionevent[4] = Main_lang_dic["MoveActionDOWN"];
            motionevent[5] = Main_lang_dic["DuplicateFrame"];
            motionevent[6] = Main_lang_dic["AddNewAction_F"];
            motionevent[7] = Main_lang_dic["InsertIntermediateFrame"];

            aboutToolStripMenuItem.Text = Main_lang_dic["aboutToolStripMenuItem_Text"];
            acc_groupBox.Text = Main_lang_dic["acc_groupBox_Text"];
            Action_groupBox.Text = Main_lang_dic["Action_groupBox_Text"];
            ActionList.Text = Main_lang_dic["ActionList_Text"];
            actionToolStripMenuItem.Text = Main_lang_dic["actionToolStripMenuItem_Text"];
            autocheck.Text = Main_lang_dic["autocheck_Text"];         
            blockingExplaination.Text = Main_lang_dic["blockingExplaination_Text"];
            bt_groupBox.Text = Main_lang_dic["bt_groupBox_Text"];
            capturebutton.Text = Main_lang_dic["capturebutton_Text"];
            CompRangeExplanation.Text = Main_lang_dic["CompRangeExplanation_Text"];
            MotionControlExplanation.Text = Main_lang_dic["MotionControlExplanation_Text"];
            editToolStripMenuItem.Text = Main_lang_dic["editToolStripMenuItem_Text"];
            exitToolStripMenuItem.Text = Main_lang_dic["exitToolStripMenuItem_Text"];
            fast.Text = Main_lang_dic["fast_Text"];
            fileToolStripMenuItem.Text = Main_lang_dic["fileToolStripMenuItem_Text"];
            Generate.Text = Main_lang_dic["Generate_Text"];
            GenerateAllInOne.Text = Main_lang_dic["GenerateAllInOne_Text"];
            getAccData.Text = Main_lang_dic["getAccData_Text"];
            helpToolStripMenuItem.Text = Main_lang_dic["helpToolStripMenuItem_Text"];
            Hint_groupBox.Text = Main_lang_dic["Hint_groupBox_Text"];
            howToUseToolStripMenuItem.Text = Main_lang_dic["howToUseToolStripMenuItem_Text"];
            Keyboard_groupBox.Text = Main_lang_dic["Keyboard_groupBox_Text"];
            DelayLabel.Text = Main_lang_dic["Main_label2_Text"];
            languageToolStripMenuItem.Text = Main_lang_dic["languageToolStripMenuItem_Text"];
            Motion_groupBox.Text = Main_lang_dic["Motion_groupBox_Text"];
            motionLayerExplanation.Text = Main_lang_dic["motionLayerExplanation_Text"];
            MotionNameLabel.Text = Main_lang_dic["MotionNameLabel_Text"];
            MotionProperty.Text = Main_lang_dic["MotionProperty_Text"];
            MotionPropertyLabel.Text = Main_lang_dic["MotionPropertyLabel_Text"];
            MotionTrigger.Text = Main_lang_dic["MotionTrigger_Text"];
            motorRelease.Text = Main_lang_dic["motorRelease_Text"];
            newToolStripMenuItem.Text = Main_lang_dic["newToolStripMenuItem_Text"];
            nonblockingExplanation.Text = Main_lang_dic["nonblockinExplanation_Text"];
            optionsToolStripMenuItem.Text = Main_lang_dic["optionsToolStripMenuItem_Text"];
            preferenceToolStripMenuItem.Text = Main_lang_dic["preferenceToolStripMenuItem_Text"];
            ps2_groupBox.Text = Main_lang_dic["ps2_groupBox_Text"];
            saveFileToolStripMenuItem.Text = Main_lang_dic["saveFileToolStripMenuItem_Text"];
            Setting_groupBox.Text = Main_lang_dic["Setting_groupBox_Text"];
            slow.Text = Main_lang_dic["slow_Text"];
            DuplicateMotionToolStripMenuItem.Text = Main_lang_dic["DuplicateMotion"];
            DeleteMotionToolStripMenuItem.Text = Main_lang_dic["DeleteMotion"];
            RenameMotionToolStripMenuItem.Text = Main_lang_dic["RenameMotion"];
            ImportMotionToolStripMenuItem.Text = Main_lang_dic["ImportMotion"];
            ExportMotionToolStripMenuItem.Text = Main_lang_dic["ExportMotion"];

            ttp.SetToolTip(AlwaysOff, Main_lang_dic["AlwaysOff_ToolTip"]);
            ttp.SetToolTip(AlwaysOn, Main_lang_dic["AlwaysOn_ToolTip"]);
            ttp.SetToolTip(autocheck, Main_lang_dic["autocheck_ToolTip"]);
            ttp.SetToolTip(btBaudCombo, Main_lang_dic["btBaudCombo_ToolTip"]);
            ttp.SetToolTip(btKeyText, Main_lang_dic["btKeyText_ToolTip"]);
            ttp.SetToolTip(btModeCombo, Main_lang_dic["btModeCombo_ToolTip"]);
            ttp.SetToolTip(btPortCombo, Main_lang_dic["btPortCombo_ToolTip"]);
            ttp.SetToolTip(capturebutton, Main_lang_dic["capturebutton_ToolTip"]);
            ttp.SetToolTip(EditMotion, Main_lang_dic["EditMotion_ToolTip"]);
            ttp.SetToolTip(Generate, Main_lang_dic["Generate_ToolTip"]);
            ttp.SetToolTip(GenerateAllInOne, Main_lang_dic["GenerateAllInOne_ToolTip"]);
            ttp.SetToolTip(KeyboardTypeCombo, Main_lang_dic["KeyboardTypeCombo_ToolTip"]);
            ttp.SetToolTip(loadFrame, Main_lang_dic["loadFrame_ToolTip"]);
            ttp.SetToolTip(motion_pause, Main_lang_dic["motion_pause_ToolTip"]);
            ttp.SetToolTip(motion_stop, Main_lang_dic["motion_stop_ToolTip"]);
            ttp.SetToolTip(MotionTest, Main_lang_dic["MotionTest_ToolTip"]);
            ttp.SetToolTip(motorRelease, Main_lang_dic["motorRelease_ToolTip"]);
            ttp.SetToolTip(move_down, Main_lang_dic["move_down_ToolTip"]);
            ttp.SetToolTip(move_up, Main_lang_dic["move_up_ToolTip"]);
            ttp.SetToolTip(NewMotion, Main_lang_dic["NewMotion_ToolTip"]);
            ttp.SetToolTip(ps2ATTCombo, Main_lang_dic["ps2ATTCombo_Text"]);
            ttp.SetToolTip(ps2CLKCombo, Main_lang_dic["ps2CLKCombo_Text"]);
            ttp.SetToolTip(ps2CMDCombo, Main_lang_dic["ps2CMDCombo_Text"]);
            ttp.SetToolTip(ps2DATCombo, Main_lang_dic["ps2DATCombo_Text"]);
            ttp.SetToolTip(ps2TypeCombo, Main_lang_dic["ps2TypeCombo_ToolTip"]);
            ttp.SetToolTip(saveFrame, Main_lang_dic["saveFrame_ToolTip"]);
            ttp.SetToolTip(sync_speed, Main_lang_dic["sync_speed_ToolTip"]);
            ttp.SetToolTip(TitleMotion, Main_lang_dic["TitleMotion_ToolTip"]);
            ttp.SetToolTip(wifi602PortCombo, Main_lang_dic["wifi602PortCombo_ToolTip"]);

            if (Motionlist != null)
            {
                Motionlist.SelectedIndex = -1;
                Framelist.Controls.Clear();
            }
            last_motionlist_idx = -1;
            this.hint_richTextBox.Text =
                "   ___   __   ____        _\n" +
                "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _86ME_path = System.Windows.Forms.Application.StartupPath;
            if (File.Exists(_86ME_path + "\\locales\\en.ini"))
            {
                TextWriter writer = new StreamWriter(_86ME_path + "\\locale.ini");
                writer.Write("en");
                writer.Dispose();
                writer.Close();
                SetLanguage sl = new SetLanguage(_86ME_path + "\\locales\\en.ini");
                Main_lang_dic = sl.lang_dic;
                applyLang();
            }
        }

        private void zhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _86ME_path = System.Windows.Forms.Application.StartupPath;
            if (File.Exists(_86ME_path + "\\locales\\zh-TW.ini"))
            {
                TextWriter writer = new StreamWriter(_86ME_path + "\\locale.ini");
                writer.Write("zh-TW");
                writer.Dispose();
                writer.Close();
                SetLanguage sl = new SetLanguage(_86ME_path + "\\locales\\zh-TW.ini");
                Main_lang_dic = sl.lang_dic;
                applyLang();
            }
        }

        private void zhHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _86ME_path = System.Windows.Forms.Application.StartupPath;
            if (File.Exists(_86ME_path + "\\locales\\zh-Hans.ini"))
            {
                TextWriter writer = new StreamWriter(_86ME_path + "\\locale.ini");
                writer.Write("zh-Hans");
                writer.Dispose();
                writer.Close();
                SetLanguage sl = new SetLanguage(_86ME_path + "\\locales\\zh-Hans.ini");
                Main_lang_dic = sl.lang_dic;
                applyLang();
            }
        }

        private void jaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string _86ME_path = System.Windows.Forms.Application.StartupPath;
            if (File.Exists(_86ME_path + "\\locales\\ja.ini"))
            {
                TextWriter writer = new StreamWriter(_86ME_path + "\\locale.ini");
                writer.Write("ja");
                writer.Dispose();
                writer.Close();
                SetLanguage sl = new SetLanguage(_86ME_path + "\\locales\\ja.ini");
                Main_lang_dic = sl.lang_dic;
                applyLang();
            }
        }

        private void getAccData_Click(object sender, EventArgs e)
        {
            if (string.Compare(com_port, "OFF") != 0 && Motion.getQ.Enabled == true)
            {
                double tolerance = 2.25;
                float x, y, z;
                arduino.pin_capture(8);
                DateTime time_start = DateTime.Now;
                while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 100) ;
                arduino.dataRecieved = false;
                x = arduino.captured_float;
                arduino.pin_capture(9);
                time_start = DateTime.Now;
                while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 100) ;
                arduino.dataRecieved = false;
                y = arduino.captured_float;
                arduino.pin_capture(10);
                time_start = DateTime.Now;
                while (!arduino.dataRecieved && (DateTime.Now - time_start).TotalMilliseconds < 100) ;
                arduino.dataRecieved = false;
                z = arduino.captured_float;
                accLXText.Text = (x - tolerance).ToString();
                accHXText.Text = (x + tolerance).ToString();
                accLYText.Text = (y - tolerance).ToString();
                accHYText.Text = (y + tolerance).ToString();
                accLZText.Text = (z - tolerance).ToString();
                accHZText.Text = (z + tolerance).ToString();
            }
            else
            {
                MessageBox.Show(Main_lang_dic["errorMsg20"]);
            }
        }
    }
}