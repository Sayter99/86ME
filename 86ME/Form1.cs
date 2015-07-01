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
using System.Management;

namespace _86ME_ver1
{
    public partial class Form1 : Form
    {
        string init_load_file = "";
        int last_sync_frame;
        int offset_Max = 255;
        int offset_min = -256;
        List<int> mtest_flag_goto = new List<int>();
        int mtest_start_pos = 0;
        int motiontest_state;
        enum mtest_states { start, pause, stop };
        int default_delay = 1000;
        int current_motionlist_idx = -1;
        public string com_port;
        Arduino arduino;
        private Panel[] fpanel = new Panel[45];
        Label[] flabel = new Label[45];
        MaskedTextBox[] ftext = new MaskedTextBox[45];
        CheckBox[] fcheck = new CheckBox[45];
        private bool[] sync_list = new bool[45];
        HScrollBar[] fbar = new HScrollBar[45];
        NewMotion Motion;
        public ArrayList ME_Motionlist;
        int framecount = 0;
        int homecount = 0;
        Boolean new_obj = false;
        String nfilename = "";
        string picture_name;
        uint[] homeframe = new uint[45];
        uint[] Max = new uint[45];
        uint[] min = new uint[45];
        int[] autoframe = new int[45];
        int[] offset = new int[45];
        int board_ver86;
        int[] motor_info = new int[45];
        int mdx, mdy;
        bool freshflag;
        bool picmode_move = false;
        bool[] captured = new bool[45];
        string[] motionevent = {"Add new action at the next field",
                                "Add homeframe",
                                "Delete action",
                                "Move action UP",
                                "Move action DOWN",
                                "Duplicate frame",
                                "Add new action at the first field"};
        char[] delimiterChars = { ' ', '\t', '\r', '\n' };
        public Form1()
        {
            InitializeComponent();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
        }

        public Form1(string filename)
        {
            InitializeComponent();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            groupBox4.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            init_load_file = filename;
            Application.Idle += new EventHandler(init_load);
        }

        private void init_load(object sender, EventArgs e)
        {
            if (String.Compare(init_load_file, "") != 0 && File.Exists(init_load_file))
            {
                MessageBox.Show("Loding Complete");
                load_project(init_load_file);
                Application.Idle -= new EventHandler(init_load);
            }
        }

        private void Update_framelist()  //set framelist
        {
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
                    fpanel[i] = new Panel();
                    flabel[i] = new Label();
                    ftext[i] = new MaskedTextBox();
                    fbar[i] = new HScrollBar();
                    fpanel[i].Size = new Size(275, 30);
                    fpanel[i].BackColor = Color.Transparent;
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
                    fcheck[i].Checked = sync_list[i];
                    fcheck[i].CheckedChanged += new EventHandler(sync_CheckedChanged);
                    fcheck[i].Name = i.ToString();
                    fcheck[i].Visible = false; // remove fcheck[i]
                    flabel[i].Size = new Size(40, 18);
                    flabel[i].BackColor = Color.White;
                    flabel[i].Top += 3;
                    flabel[i].Left += 20;
                    ftext[i].Size = new Size(45, 22);
                    ftext[i].Left += 60;
                    ftext[i].TextAlign = HorizontalAlignment.Right;

                    flabel[i].Name = i.ToString();
                    ftext[i].Name = i.ToString();

                    if (Motion.picfilename != null)
                    {
                        flabel[i].MouseDown += new MouseEventHandler(flMouseDown);
                        flabel[i].MouseMove += new MouseEventHandler(flMouseMove);
                    }
                    ftext[i].KeyPress += new KeyPressEventHandler(numbercheck);
                    fbar[i].Size = new Size(160, 22);
                    fbar[i].Left += 110;

                    fbar[i].Maximum = (int)(Max[i] + 9);
                    fbar[i].Minimum = (int)min[i];

                    fbar[i].Name = i.ToString();
                    fbar[i].Scroll += new ScrollEventHandler(scroll_event);
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

                    ttp.SetToolTip(fcheck[i], "Sychronize the selected motor.");
                    fpanel[i].Controls.Add(fcheck[i]);
                    fpanel[i].Controls.Add(flabel[i]);
                    fpanel[i].Controls.Add(ftext[i]);
                    fpanel[i].Controls.Add(fbar[i]);
                    Framelist.Controls.Add(fpanel[i]);
                    
                    count++;
                }
            }
        }

        private bool sync_list_empty()
        {
            for (int i = 0; i < 45; i++)
                if (sync_list[i] == true)
                    return false;

            return true;
        }

        public void sync_CheckedChanged(object sender, EventArgs e) //sender -> fcheck[i]
        {
            int index = int.Parse(((CheckBox)sender).Name);
            if (((CheckBox)sender).Checked == true && autocheck.Checked == false)
            {
                sync_list[index] = true;
                autocheck.Checked = true;
            }
            else if (((CheckBox)sender).Checked == true && autocheck.Checked == true && sync_list[index] == false)
            {
                sync_list[index] = true;
                for (int i = 0; i < 45; i++)
                {
                    if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && sync_list[i] == true)
                        autoframe[i] = (int.Parse(ftext[i].Text) + offset[i]);
                    else if (sync_list[i] == false)
                        autoframe[i] = 0;
                }
                autocheck.Enabled = false;

                if (string.Compare(com_port, "OFF") != 0)
                {
                    try
                    {
                        arduino.frameWrite(0x6F, autoframe, int.Parse(delaytext.Text));
                        Thread.Sleep(int.Parse(delaytext.Text));
                    }
                    catch
                    {
                        com_port = "OFF";
                        MessageBox.Show("Failed to send messages. Please check the connection and restart.");
                    }
                }
                autocheck.Enabled = true;
            }
            else if (((CheckBox)sender).Checked == false)
            {
                sync_list[index] = false;
                if (sync_list_empty())
                    autocheck.Checked = false;
            }
        }

        public void flMouseDown(object sender, MouseEventArgs e)
        {
                mdx = e.X;
                mdy = e.Y;
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
             //if (e.Type == ScrollEventType.EndScroll)
                this.ftext[int.Parse(((HScrollBar)sender).Name)].Text = ((HScrollBar)sender).Value.ToString();            
        }

        public void SyncSpeed(object sender, EventArgs e)
        {
            if (string.Compare(com_port, "OFF") != 0)
            {
                if(sync_speed.Value == 5)
                    arduino.setSyncSpeed(0);
                else
                    arduino.setSyncSpeed(400 + sync_speed.Value * 400);
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
                this.fbar[int.Parse(((MaskedTextBox)sender).Name)].Value = int.Parse(((MaskedTextBox)sender).Text);
                if (autocheck.Checked == true)
                {
                    if (!freshflag)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && sync_list[i] == true)
                                autoframe[i] = (int.Parse(ftext[i].Text) + offset[i]);
                            else if (sync_list[i] == false)
                                autoframe[i] = 0;
                        }
                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, 0);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Failed to send messages. Please check the connection and restart.");
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
        private void fileToolStripMenuItem_Click(object sender, EventArgs e) //new project
        {
            if (Motion != null)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save this project?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    saveFileToolStripMenuItem_Click(sender, e);
                }
            }
            NewMotion nMotion = new NewMotion();
            if (string.Compare(com_port, "OFF") != 0)
                nMotion.arduino = arduino;
            nMotion.ShowDialog();
            if (nMotion.DialogResult == DialogResult.OK)
            {
                Motion = nMotion;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                saveFileToolStripMenuItem.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                NewMotion.Enabled = false;
                ME_Motionlist = new ArrayList();
                MotionCombo.Items.Clear();
                MotionCombo.Text = "";
                Motionlist.Items.Clear();
                delaytext.Text = default_delay.ToString();
                typecombo.Text = "";
                board_ver86 = Motion.comboBox1.SelectedIndex;

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
                    motor_info[i] = Motion.fbox[i].SelectedIndex;
                    homeframe[i] = uint.Parse(nMotion.ftext2[i].Text);
                    min[i] = uint.Parse(nMotion.ftext3[i].Text);
                    Max[i] = uint.Parse(nMotion.ftext4[i].Text);
                    try
                    {
                        offset[i] = int.Parse(nMotion.ftext[i].Text);
                    }
                    catch
                    {
                        offset[i] = 0;
                        nMotion.ftext[i].Text = "0";
                        string error_msg = "The offset " + i.ToString() + " is illegal, set to 0";
                        MessageBox.Show(error_msg);
                    }
                }

                if(pictureBox1.Image != null)
                    pictureBox1.Image = null;
                if (nMotion.picfilename != null)
                {
                    picture_name = nMotion.picfilename;
                    draw_background();
                }

                this.richTextBox1.Text = "      1.Enter a Motion Name and 2.Press Add Motion --->";
            }
        }

        public void connect_comport()
        {
            if (arduino == null && (string.Compare(com_port, "OFF") != 0))
            {
                if (!have_86())
                {
                    ;
                }
                else
                {
                    if (string.Compare(com_port, "AUTO") == 0)
                    {
                        try
                        {
                            arduino = new Arduino();
                            arduino.setSyncSpeed(0);
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show("Cannot open 86Duino, entering offline mode", "",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        try
                        {
                            arduino = new Arduino(com_port);
                            arduino.setSyncSpeed(0);
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show("Cannot open 86Duino, entering offline mode", "",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)  //option
        {
            for (int i = 0; i < 45; i++)
                Motion.fcheck[i].Checked = false;
            autocheck.Checked = false;
            if (string.Compare(com_port, "OFF") != 0)
                Motion.arduino = arduino;
            Motion.ShowDialog();
            if (Motion.DialogResult == DialogResult.OK)
            {
                if (Motion.picfilename != null)
                {
                    picture_name = Motion.picfilename;
                    try
                    {
                        pictureBox1.Image = Image.FromFile(Motion.picfilename);
                    }
                    catch
                    {
                        Motion.picfilename = null;
                        MessageBox.Show("Cannot load the picture. Please check the file");
                    }
                }
                for (int i = 0; i < 45; i++)
                {
                    if (Motion.ftext[i].Text == "")
                        Motion.ftext[i].Text = "0";
                    if (Motion.ftext2[i].Text == "")
                        Motion.ftext2[i].Text = "1500";
                    if (Motion.ftext3[i].Text == "")
                        Motion.ftext3[i].Text = "600";
                    if (Motion.ftext4[i].Text == "")
                        Motion.ftext4[i].Text = "2400";
                    homeframe[i] = uint.Parse(Motion.ftext2[i].Text);
                    min[i] = uint.Parse(Motion.ftext3[i].Text);
                    Max[i] = uint.Parse(Motion.ftext4[i].Text);
                    motor_info[i] = Motion.fbox[i].SelectedIndex;
                    if (homeframe[i] > Max[i] || homeframe[i] < min[i])
                    {
                        homeframe[i] = 1500;
                        Motion.ftext2[i].Text = "1500";
                    }
                    try
                    {
                        offset[i] = int.Parse(Motion.ftext[i].Text);
                        if (offset[i] > offset_Max || offset[i] < offset_min)
                        {
                            offset[i] = 0;
                            Motion.ftext[i].Text = "0";
                        }
                    }
                    catch
                    {
                        offset[i] = 0;
                        Motion.ftext[i].Text = "0";
                        string error_msg = "The offset " + i.ToString() + " is illegal, set to 0";
                        MessageBox.Show(error_msg);
                    }
                }
                Update_framelist();
                update_motionlist();
                draw_background();
                board_ver86 = Motion.comboBox1.SelectedIndex;
            }
            else if (Motion.DialogResult == DialogResult.Cancel)
            {
                Motion.picfilename = picture_name;
                Motion.comboBox1.SelectedIndex = board_ver86;
                for (int i = 0; i < 45; i++)
                {
                    Motion.fbox[i].SelectedIndex = motor_info[i];
                    Motion.ftext[i].Text = offset[i].ToString();
                    Motion.ftext2[i].Text = homeframe[i].ToString();
                    Motion.ftext3[i].Text = min[i].ToString();
                    Motion.ftext4[i].Text = Max[i].ToString();
                }
            }
        }
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)    //save project
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "rbm files (*.rbm)|*.rbm";
            dialog.Title = "Save File";
            dialog.FileName = nfilename;
            if (dialog.ShowDialog() == DialogResult.OK && dialog.FileName != null)
            {
                nfilename = Path.GetFileName(dialog.FileName);
                TextWriter writer = new StreamWriter(dialog.OpenFile());
                string nFilePath = Path.GetDirectoryName(dialog.FileName);

                writer.Write("BoardVer ");
                writer.Write(Motion.comboBox1.SelectedItem.ToString());
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
                //
                writer.WriteLine("Sync " + sync_speed.Value.ToString());
                //
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

                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    writer.Write("Motion " + m.name + "\n");
                    for (int j = 0; j < m.Events.Count; j++)
                    {
                        if (m.Events[j] is ME_Frame)
                        {
                            ME_Frame f=(ME_Frame)m.Events[j];
                            if(f.type == 1)
                                writer.Write("frame " + f.delay.ToString() + " ");
                            else if(f.type == 0)
                                writer.Write("home " + f.delay.ToString() + " ");
                            int count = 0;
                            for(int k = 0; k < 45; k++)
                                if (String.Compare(Motion.fbox[k].Text,"---noServo---") != 0) {
                                    count++;
                                }
                            for (int k = 0; k < 45; k++)
                                if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                                {
                                    count--;
                                    writer.Write(f.frame[k].ToString());
                                    if (count != 0)
                                        writer.Write(" ");
                                }
                            writer.Write("\n");
                        }
                        else if(m.Events[j] is ME_Delay)
                        {
                            ME_Delay d=(ME_Delay)m.Events[j];
                            writer.Write("delay " + d.delay.ToString() + "\n");
                        }
                        else if (m.Events[j] is ME_Sound)
                        {
                            ME_Sound s = (ME_Sound)m.Events[j];
                            writer.Write("sound " + s.filename + " " + s.delay.ToString() + "\n");
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
                    }
                    writer.Write("MotionEnd " + m.name);
                    if (i != ME_Motionlist.Count - 1)
                        writer.Write("\n");
                }

                writer.Dispose();
                writer.Close();
            }
        }
        private void actionToolStripMenuItem_Click(object sender, EventArgs e)      //load project
        {
            if (Motion != null)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save this project?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    saveFileToolStripMenuItem_Click(sender, e);
                }
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "rbm files (*.rbm)|*.rbm";
            dialog.Title = "Open File";
            String filename = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : null;
            if (filename == null)
                return;
            load_project(filename);
            MessageBox.Show("Loding Complete");
        }

        public void load_project(string filename)
        {
            bool picmode = false;
            NewMotion nMotion = new NewMotion();
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
                                            "unknow"};
            string[] servo = new string[] { "---noServo---",
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
                                            "DMP_RS0263",         
                                            "DMP_RS1270",         
                                            "GWS_S777",           
                                            "GWS_S03T",           
                                            "GWS_MICRO",
                                            "OtherServos"};

            using (StreamReader reader = new StreamReader(filename))
            {
                nfilename = Path.GetFileName(filename);

                string[] datas = reader.ReadToEnd().Split(delimiterChars);
                if (datas.Length < 235)
                {
                    MessageBox.Show("The loaded file is corrupt. It will not be loaded.");
                    return;
                }
                if (datas[234] == "picmode")
                {
                    if (datas[0] != "BoardVer" || datas[2] != "Servo" || datas[48] != "Offset" ||
                       datas[95] != "Homeframe" || datas[142] != "Range")
                    {
                        MessageBox.Show("The loaded file is corrupt. It will not be loaded.");
                        return;
                    }
                }
                else
                {
                    if (datas[0] != "BoardVer" || datas[2] != "Servo" || datas[48] != "Offset" ||
                       datas[95] != "Homeframe" || datas[142] != "Range")
                    {
                        MessageBox.Show("The loaded file is corrupt. It will not be loaded.");
                        return;
                    }
                }

                ME_Motionlist = new ArrayList();
                ME_Motion motiontag = null;
                MotionCombo.Items.Clear();
                MotionCombo.Text = "";
                Motionlist.Items.Clear();
                delaytext.Text = default_delay.ToString();
                typecombo.Text = "";

                for (int i = 0; i < datas.Length; i++)
                {
                    if (String.Compare(datas[i], "BoardVer") == 0)
                    {
                        i++;
                        for (int j = 0; j < rbver.Length; j++)
                            if (String.Compare(datas[i], rbver[j]) == 0)
                            {
                                //***fix bug after remove rb
                                nMotion.comboBox1.SelectedIndex = j - 7;
                                board_ver86 = j - 7;
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
                                MessageBox.Show("The loaded file is corrupt. Please check the format of Offset.");
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
                                MessageBox.Show("The loaded file is corrupt. Please check the format of Homeframe.");
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
                                MessageBox.Show("The loaded file is corrupt. Please check the format of Range.");
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
                                MessageBox.Show("The loaded file is corrupt. Please check the format of Range.");
                            }
                        }
                    }
                    else if (String.Compare(datas[i], "Sync") == 0)
                    {
                        i++;
                        sync_speed.Value = int.Parse(datas[i]);
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
                                MessageBox.Show("The loaded file is corrupt. Please check the format of Servo.");
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
                            ME_Motionlist.Add(motiontag);
                        }
                    }
                    else if (String.Compare(datas[i], "MotionEnd") == 0)
                    {
                        i++;
                        if (motiontag != null)
                            if (String.Compare(datas[i], motiontag.name) == 0)
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
                            MessageBox.Show("The loaded file is corrupt. Please check the format of frame.");
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
                                    MessageBox.Show("The loaded file is corrupt. Please check the format of frame.");
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
                            MessageBox.Show("The loaded file is corrupt. Please check the format of frame.");
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
                                    MessageBox.Show("The loaded file is corrupt. Please check the format of frame.");
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
                            MessageBox.Show("The loaded file is corrupt. Please check the format of delay.");
                        }
                        motiontag.Events.Add(ndelay);
                    }
                    else if (String.Compare(datas[i], "sound") == 0)
                    {
                        ME_Sound nsound = new ME_Sound();
                        i++;
                        nsound.filename = datas[i];
                        i++;
                        nsound.delay = int.Parse(datas[i]);
                        motiontag.Events.Add(nsound);
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
                }
            }

            nMotion.write_back();

            if (nMotion.picfilename != null && picmode == true)
            {
                picture_name = nMotion.picfilename;
                try
                {
                    pictureBox1.Image = Image.FromFile(nMotion.picfilename);
                }
                catch
                {
                    nMotion.picfilename = null;
                    MessageBox.Show("Cannot load the picture. Please check the file");
                }
            }
            else
            {
                nMotion.picfilename = null;
                pictureBox1.Image = null;
            }

            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            saveFileToolStripMenuItem.Enabled = true;
            NewMotion.Enabled = false;
            Motion = nMotion;

            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                MotionCombo.Items.Add(m.name);
            }

            if (MotionCombo.Items.Count > 0)
                MotionCombo.SelectedIndex = 0;
            this.richTextBox1.Text =
                            "   ___   __   ____        _\n" +
                            "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                            "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                            " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                            "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MotionTest.Enabled = false;
            motion_pause.Enabled = false;
            motion_stop.Enabled = false;
        }

        private void typecombo_TextChanged(object sender, EventArgs e) // choose a type of action
        {
            Framelist.Controls.Clear();
            if (String.Compare(typecombo.Text, "Frame") == 0)
            {
                if (new_obj)
                {
                    Motionlist.Items.Insert(Motionlist.SelectedIndex+1,"[Frame] " + MotionCombo.SelectedItem.ToString() + "-" + framecount++.ToString());
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());

                    for (int i = 0; i < 45; i++)
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame[i] = (int)homeframe[i];
                    ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = default_delay;
                    Motionlist.SelectedIndex++;
                }
                delaytext.Enabled = true;
                label2.Enabled = true;
                Framelist.Enabled = true;
                capturebutton.Enabled = true;
                autocheck.Enabled= true;
                typecombo.Enabled = false;
                Update_framelist();
                new_obj = false;
                if(Motion.picfilename == null)
                    this.richTextBox1.Text = "Tune the settings of motors\n↓\n↓\n↓";
                else
                    this.richTextBox1.Text = "1.Left click on tag \"Ch XX\" and drag to move it\n2.Tune the settings of motors\n↓\n↓";
            }
            else if (String.Compare(typecombo.Text, "HomeFrame") == 0)
            {
                if (new_obj)
                {
                    Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Home] " + homecount++.ToString());
                    ME_Frame h = new ME_Frame();
                    h.type = 0;
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, h);

                    for (int i = 0; i < 45; i++)
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame[i] = (int)homeframe[i];
                    ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = default_delay;
                    Motionlist.SelectedIndex++;
                }
                delaytext.Enabled = true;
                label2.Enabled = true;
                capturebutton.Enabled = false;
                autocheck.Enabled = true;
                typecombo.Enabled = false;
                Update_framelist();
                Framelist.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "Homeframe just can be modified by\nOptions -> Robot Configuration";
            }
            else if (String.Compare(typecombo.Text, "Delay") == 0)
            {
                if (new_obj)
                {
                    Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Delay]");
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Delay());
                    ((ME_Delay)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = default_delay;
                    Motionlist.SelectedIndex++;
                }
                delaytext.Enabled = true;
                label2.Enabled = true;
                Framelist.Enabled = false;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "\n\n\n<--- Set the delay you want";
            }
            else if (String.Compare(typecombo.Text, "Sound") == 0)
            {
                if (new_obj)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "wav files (*.wav)|*.wav";
                    dialog.Title = "Link Sound";
                    String filename = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : null;
                    if (filename != null)
                    {
                            ME_Sound s = new ME_Sound();
                            s.filename = Path.GetFileName(filename);
                            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Sound] " + Path.GetFileName(filename));
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, s);
                            ((ME_Sound)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = default_delay;
                            Motionlist.SelectedIndex++;
                    }
                }
                delaytext.Enabled = true;
                label2.Enabled = true;
                Framelist.Enabled = true;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
            }
            else if (String.Compare(typecombo.Text, "Flag") == 0)
            {
                if (new_obj)
                {
                    Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Flag]");
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Flag());
                    Motionlist.SelectedIndex++;
                }
                delaytext.Text = "";
                delaytext.Enabled = false;
                label2.Enabled = false;
                Framelist.Enabled = true;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "Set the name of the flag\n↓\n↓\n↓";
            }
            else if (String.Compare(typecombo.Text, "Goto") == 0)
            {
                
                if(new_obj){
                    Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Goto]");
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Goto());
                    Motionlist.SelectedIndex++;
                }
                delaytext.Text = "";
                delaytext.Enabled = false;
                label2.Enabled = false;
                Framelist.Enabled = true;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "Set target Flag Name and the number of loops\n↓\n↓\n↓";
            }
            else if (String.Compare(typecombo.Text, "Select type") == 0)
            {
                new_obj = true;
                typecombo.Enabled = true;
                delaytext.Enabled = false;
                delaytext.Text = "";
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                Framelist.Enabled = false;
            }
            draw_background();
        }
        private void MotionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            update_motionlist();
            move_up.Enabled = false;
            move_down.Enabled = false;
            current_motionlist_idx = -1;
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

        private void Motionlist_SelectedIndexChanged(object sender, EventArgs e) // select motionlist
        {
            if (Motionlist.SelectedIndex == -1)
            {
                move_up.Enabled = false;
                move_down.Enabled = false;
                this.label2.Text = "Delay:";
            }
            if (Motionlist.SelectedItem != null && (MotionTest.Enabled))
            {
                move_up.Enabled = true;
                move_down.Enabled = true;
                current_motionlist_idx = Motionlist.SelectedIndex;
                groupBox1.Enabled = true;
                groupBox4.Enabled = true;
                string[] datas = Motionlist.SelectedItem.ToString().Split(' ');
                if (String.Compare(datas[0], "[Frame]") == 0)
                {
                    this.label2.Text = "Play Time:";
                    typecombo.SelectedIndex = 0;
                    typecombo.Text = "Frame";
                    delaytext.Text = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && sync_list[i] == true)
                                autoframe[i] = (int)(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] + offset[i]);
                            else if (sync_list[i] == false)
                                autoframe[i] = 0;
                        }
                        autocheck.Enabled = false;
                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, (int)((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Failed to send messages. Please check the connection and restart.");
                            }
                        }
                        autocheck.Enabled = true;
                    }
                    freshflag = false;
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && fpanel[i] != null)
                        {
                            freshflag = true;
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
                    if (!freshflag)
                        Update_framelist();
                    freshflag = false;
                    Framelist.Enabled = true;
                    draw_background();
                }
                else if (String.Compare(datas[0], "[Home]") == 0)
                {
                    this.label2.Text = "Play Time:";
                    typecombo.SelectedIndex = 4;
                    typecombo.Text = "HomeFrame";
                    delaytext.Text = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && sync_list[i] == true)
                                autoframe[i] = (int)homeframe[i] + offset[i];
                            else if (sync_list[i] == false)
                                autoframe[i] = 0;
                        }
                        autocheck.Enabled = false;
                        if (string.Compare(com_port, "OFF") != 0)
                        {
                            try
                            {
                                arduino.frameWrite(0x6F, autoframe, (int)((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Failed to send messages. Please check the connection and restart.");
                            }
                        }
                        autocheck.Enabled = true;
                    }
                    freshflag = false;
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && fpanel[i] != null)
                        {
                            freshflag = true;
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
                    if (!freshflag)
                        Update_framelist();
                    freshflag = false;
                    Framelist.Enabled = false;
                    draw_background();
                }
                else if (String.Compare(datas[0], "[Delay]") == 0)
                {
                    this.label2.Text = "Delay:";
                    typecombo.SelectedIndex = 1;
                    typecombo.Text = "Delay";
                    delaytext.Text = ((ME_Delay)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                    draw_background();
                }
                else if (String.Compare(datas[0], "[Sound]") == 0)
                {
                    this.label2.Text = "Play Time:";
                    typecombo.SelectedIndex = 2;
                    typecombo.Text = "Sound";
                    delaytext.Text = ((ME_Sound)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                }
                else if (String.Compare(datas[0], "[Flag]") == 0)
                {
                    typecombo.SelectedIndex = 3;
                    this.label2.Text = "Delay:";
                    typecombo.Text = "Flag";
                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = "Name: ";
                    xlabel.Size = new Size(40, 22);

                    MaskedTextBox xtext = new MaskedTextBox();
                    
                    xtext.Text = ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    xtext.TextChanged += new EventHandler(gototext);
                    xtext.Size = new Size(160, 22);
                    xtext.Left += 45;
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xtext);
                    Framelist.Enabled = true;
                    draw_background();
                    xtext.SelectionStart = xtext.Text.Length;
                    xtext.Focus();
                }
                else if (String.Compare(datas[0], "[Goto]") == 0)
                {
                    typecombo.SelectedIndex = 2;
                    this.label2.Text = "Delay:";
                    typecombo.Text = "Goto";
                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = "Target Flag Name: ";
                    xlabel.Size = new Size(95, 22);
                    MaskedTextBox xtext = new MaskedTextBox();
                    xtext.Text = ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    xtext.TextChanged += new EventHandler(gototext);
                    xtext.Size = new Size(160, 22);
                    xtext.Left += 100;
                    Label xlabel2 = new Label();
                    xlabel2.Text = "Enable Goto ";
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
                    xlabel4.Text = "Loop Infinitely ";
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
                    xlabel3.Text = "Number of loops: ";
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
                    this.richTextBox1.Text = "Set target Flag Name of the Goto\n↓\n↓\n↓";
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xtext);
                    Framelist.Controls.Add(xlabel2);
                    Framelist.Controls.Add(xcheckbox);
                    Framelist.Controls.Add(xlabel3);
                    Framelist.Controls.Add(xtext2);
                    Framelist.Controls.Add(xlabel4);
                    Framelist.Controls.Add(xcheckbox2);
                    Framelist.Enabled = true;
                    draw_background();
                    xtext.SelectionStart = xtext.Text.Length;
                    xtext.Focus();
                }
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
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Sound)
                {
                    ((ME_Sound)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay = int.Parse(((MaskedTextBox)sender).Text);
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
                    motionToolStripMenuItem.Text = "Add new action at the first field";
                    contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { motionToolStripMenuItem });
                    contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    contextMenuStrip1.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    contextMenuStrip1.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                    Framelist.Enabled = false;
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Frame)
                {
                    motionToolStripMenuItem.Text = "Add new action at the next field";
                    contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { motionToolStripMenuItem });
                    for (int i = 2; i < motionevent.Length - 1; i++)
                        contextMenuStrip1.Items.Add(motionevent[i]);
                    contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    contextMenuStrip1.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    contextMenuStrip1.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                }
                else if (Motionlist.SelectedItem != null)
                {
                    motionToolStripMenuItem.Text = "Add new action at the next field";
                    contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { motionToolStripMenuItem });
                    for (int i = 2; i < motionevent.Length - 2; i++)
                        contextMenuStrip1.Items.Add(motionevent[i]);
                    contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    contextMenuStrip1.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    contextMenuStrip1.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                }
            }
        }

        private void Motionlistevent(object sender, ToolStripItemClickedEventArgs e)
        {
            int n;
            for (int i = 0; i < motionevent.Length; i++)
                if (String.Compare(e.ClickedItem.Text, motionevent[i]) == 0)
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
                            typecombo.Enabled = false;
                            typecombo.Text="";
                            delaytext.Enabled = false;
                            delaytext.Text = "";
                            capturebutton.Enabled = false;
                            autocheck.Enabled= false;
                            Framelist.Controls.Clear();
                            Framelist.Enabled = false;
                            draw_background();
                            break;
                        case 3:
                            Framelist.Enabled = false;
                            n = Motionlist.SelectedIndex;
                            if (n == 0)
                                break;
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n - 1, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n + 1);
                            Motionlist.Items.Insert(n - 1,Motionlist.SelectedItem);
                            Motionlist.Items.RemoveAt(n + 1);
                            break;
                        case 4:
                            Framelist.Enabled = false;
                            n = Motionlist.SelectedIndex;
                            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count <= n+1)
                                break;
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n + 2, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n);
                            Motionlist.Items.Insert(n + 2, Motionlist.SelectedItem);
                            Motionlist.Items.RemoveAt(n);
                            break;
                        case 5:
                            Framelist.Enabled = false;
                            ME_Frame f = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]);
                            if (f.type == 1)
                            {
                                Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Frame] " + MotionCombo.SelectedItem.ToString() + "-" + framecount++.ToString());
                                ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());
                                Array.Copy(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame, ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame, 45);
                                ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay;
                                Motionlist.SelectedIndex++;
                            }
                            else if (f.type == 0)
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
                    }
        }

        private void Motionlistcloseevent(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.ItemClicked -= Motionlistevent;
            contextMenuStrip1.Closed -= Motionlistcloseevent;
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

        private void MotionCombo_TextChanged(object sender, EventArgs e)
        {
            current_motionlist_idx = -1;
            move_up.Enabled = false;
            move_down.Enabled = false;
            Boolean new_mot = true;
            NewMotion.Enabled = false;
            if (String.Compare(MotionCombo.Text, "") == 0)
                new_mot = false;
            for (int i = 0; i < MotionCombo.Items.Count; i++) 
                if (String.Compare(MotionCombo.Text, MotionCombo.Items[i].ToString()) == 0)
                    new_mot = false;
            if (new_mot) {
                NewMotion.Enabled = true;
            }
            Motionlist.Items.Clear();
            MotionTest.Enabled = false;
            motion_pause.Enabled = false;
            motion_stop.Enabled = false;
            groupBox1.Enabled = false;
            groupBox4.Enabled = false;
            this.richTextBox1.Text = "      1.Enter a Motion Name and 2.Press Add Motion --->"; ;
        }
        private void NewMotion_Click(object sender, EventArgs e)
        {
            if (MotionCombo.Text.IndexOf(" ") == -1)
            {
                MotionCombo.Items.Add(MotionCombo.Text);
                ME_Motion m = new ME_Motion();
                m.name = MotionCombo.Text;
                ME_Motionlist.Add(m);
                MotionCombo.SelectedIndex = MotionCombo.Items.Count - 1;
                Motionlist.Controls.Clear();
                draw_background();
                current_motionlist_idx = -1;
                move_up.Enabled = false;
                move_down.Enabled = false;
                this.richTextBox1.Text = "\n\n\n\nRight click in the white region and add an action --->";
                Motionlist.Focus();
            }
            else
                MessageBox.Show("Motion name should without space.");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool close = true;
            if (Motion != null)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save this project?", "Exit", MessageBoxButtons.YesNoCancel);
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
                    this.richTextBox1.Text = "\n\n\n\tThe used motors don't support Capture";
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
                else if (String.Compare(Motion.fbox[i].Text, "TOWERPRO_MG995") == 0)
                    captured[i] = false;
                else if (String.Compare(Motion.fbox[i].Text, "TOWERPRO_MG996") == 0)
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
                else if (String.Compare(Motion.fbox[i].Text, "OtherServos") == 0)
                    captured[i] = false;

                if (captured[i] == true)
                    can_cap = true;
            }
            return can_cap;
        }

        public void MotionOnTest()
        {
            MotionTest.Enabled = false;
            motion_pause.Enabled = true;
            motion_stop.Enabled = true;
            autocheck.Enabled = false;
            capturebutton.Enabled = false;
            Framelist.Enabled = false;
            SoundPlayer sp = null;
            if (ME_Motionlist == null)
                return;

            for (int j = mtest_start_pos; j < ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count; j++)
            {
                Motionlist.SelectedIndex = j;
                if (motiontest_state == (int)mtest_states.stop)
                    break;
                else if(motiontest_state == (int)mtest_states.pause)
                {
                    mtest_start_pos = j;
                    MotionTest.Enabled = true;
                    motion_pause.Enabled = false;
                    motion_stop.Enabled = true;
                    Motionlist.Enabled = false;
                    MotionCombo.Enabled = false;
                    if (sp != null)
                        sp.Stop();
                    this.richTextBox1.Text =
                            "   ___   __   ____        _\n" +
                            "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                            "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                            " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                            "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
                    return;
                }
                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j] is ME_Frame)
                {
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        {
                            autoframe[i] = (((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).frame[i] + offset[i]);
                        }
                    }
                    if (string.Compare(com_port, "OFF") != 0)
                    {
                        try
                        {
                            arduino.frameWrite(0x6F, autoframe, (int)((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).delay);
                            Thread.Sleep((int)((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).delay);
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show("Failed to send messages. Please check the connection and restart.");
                        }
                    }
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j] is ME_Delay)
                {
                    Thread.Sleep((int)((ME_Delay)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).delay);
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j] is ME_Sound)
                {
                    sp = new SoundPlayer(Application.StartupPath + "\\" + ((ME_Sound)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).filename);
                    sp.Play();
                    Thread.Sleep((int)((ME_Sound)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).delay);
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j] is ME_Goto)
                {
                    if (((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).is_goto &&
                        (((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).current_loop > 0 ||
                        ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).infinite))
                    {
                        if (((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).infinite == false)
                            ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).current_loop--;
                        for (int k = 0; k < j; k++)
                        {
                            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[k] is ME_Flag)
                            {
                                if (String.Compare(((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).name,
                                                ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[k]).name) == 0)
                                    j = k;
                            }
                        }
                    }
                    else if(((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).current_loop == 0)
                    {
                        int loop_num = int.Parse(((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).loops);
                        ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).current_loop = loop_num;
                    }
                }
            }
            mtest_start_pos = 0;
            typecombo.Text = "";
            MotionTest.Enabled = true;
            motion_pause.Enabled = false;
            motion_stop.Enabled = false;
            Motionlist.Enabled = true;
            MotionCombo.Enabled = true;
            for (int j = 0; j < ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count; j++)
            {
                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j] is ME_Goto)
                {
                    int loops = int.Parse(((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).loops);
                    ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).current_loop = loops;
                }
            }
            if (sp != null)
                sp.Stop();
            this.richTextBox1.Text =
                    "   ___   __   ____        _\n" +
                    "  ( _ ) / /_ |  _ \\ _   _(_)_ __   ___\n" +
                    "  / _ \\| '_ \\| | | | | | | | '_ \\ / _ \\\n" +
                    " | (_) | (_) | |_| | |_| | | | | | (_) |\n" +
                    "  \\___/ \\___/|____/ \\__,_|_|_| |_|\\___/";
        }

        private void MotionTest_Click(object sender, EventArgs e)
        {
            // TODO: Using Thread to control UI
            Motionlist.Focus();
            Framelist.Controls.Clear();
            motiontest_state = (int)mtest_states.start;
            Thread t;
            if (MotionCombo.SelectedItem != null)
            {
                CheckForIllegalCrossThreadCalls = false;// dangerous
                t = new Thread(MotionOnTest);
                t.Start();
            }
            Update_framelist();
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
                typecombo.Text = "";
                MotionTest.Enabled = true;
                motion_pause.Enabled = false;
                motion_stop.Enabled = false;
                Framelist.Enabled = false;
                Motionlist.Enabled = true;
                MotionCombo.Enabled = true;
                this.richTextBox1.Text =
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
            bool active_autocheck = false;
            if (MotionCombo.SelectedItem != null)
                if (ME_Motionlist != null)
                    if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Capacity - 1 >= Motionlist.SelectedIndex && Motionlist.SelectedIndex >= 0)
                        if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Frame)
                            active_autocheck = true;

            if (String.Compare(delaytext.Text, "") == 0)
                delaytext.Text = default_delay.ToString();
            if (active_autocheck)
            {
                if (autocheck.Checked == true && int.Parse(delaytext.Text) < 0)
                {
                    MessageBox.Show("Please set the correct delay time.");
                    autocheck.Checked = false;
                }
                else if (autocheck.Checked == true)
                {
                    if (sync_list_empty())
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                sync_list[i] = true;
                                fcheck[i].Checked = true;
                            }
                        }
                    }
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && sync_list[i] == true)
                            autoframe[i] = (int.Parse(ftext[i].Text) + offset[i]);
                        else if (sync_list[i] == false)
                            autoframe[i] = 0;
                    }

                    autocheck.Enabled = false;

                    if (string.Compare(com_port, "OFF") != 0)
                    {
                        try
                        {
                            if (last_sync_frame != Motionlist.SelectedIndex)
                            {
                                arduino.frameWrite(0x6F, autoframe, int.Parse(delaytext.Text));
                                Thread.Sleep(int.Parse(delaytext.Text));
                            }
                            else if(last_sync_frame == Motionlist.SelectedIndex)
                            {
                                arduino.frameWrite(0x6F, autoframe, 0);
                            }
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show("Failed to send messages. Please check the connection and restart.");
                        }
                    }
                    autocheck.Enabled = true;
                }
                else if(autocheck.Checked == false)
                {
                    last_sync_frame = Motionlist.SelectedIndex;
                    for(int i = 0; i < 45; i++)
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && sync_list[i] == true)
                            fcheck[i].Checked = false;
                }
            }
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.86duino.com/?p=11544");
        }

        public void generate_ino(string path, List<int> channels, int frame_count)
        {
            Directory.CreateDirectory(path + "\\_86Duino_Motion_Sketch");
            nfilename = path + "\\_86Duino_Motion_Sketch\\_86Duino_Motion_Sketch" + ".ino";
            TextWriter writer = new StreamWriter(nfilename);

            // include and declare
            writer.WriteLine("#include <Servo86.h>");
            writer.WriteLine();
            for(int i=0; i<channels.Count; i++)
                writer.WriteLine("Servo myservo" + channels[i].ToString() + ";");

            writer.WriteLine();
            writer.WriteLine("ServoOffset myoffs(\"86offset.txt\");");
            writer.WriteLine();

            for (int i = 0; i < frame_count; i++)
            {
                string fc = i.ToString();
                writer.WriteLine("ServoFrame frm" + fc + "(\"86frame_" + fc + ".txt\");");
            }
            writer.WriteLine();

            //void setup {}
            writer.WriteLine("void setup()");
            writer.WriteLine("{");
            for (int i = 0; i < channels.Count; i++)
                writer.WriteLine("  myservo" + channels[i].ToString() + ".attach(" + channels[i].ToString() + ");");
            writer.WriteLine("  myoffs.setOffsets();");
            writer.WriteLine("}");
            writer.WriteLine();

            //void loop {}
            ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
            writer.WriteLine("void loop()");
            writer.WriteLine("{");
            int space_num = 2;
            string space = set_space(space_num);
            for (int i = 0, j = 0, flag_count = 0; i < m.Events.Count; i++)
            {
                if (m.Events[i] is ME_Frame)
                {
                    ME_Frame f = (ME_Frame)m.Events[i];
                    writer.WriteLine(space + "frm" + f.num.ToString() + ".playPositions(" + f.delay.ToString() + ");");
                    writer.WriteLine(space + "while(isServoMultiMoving() == true);");
                    if (i != m.Events.Count - 1)
                        writer.WriteLine();
                    j++;
                }
                else if (m.Events[i] is ME_Delay)
                {
                    ME_Delay d = (ME_Delay)m.Events[i];
                    writer.WriteLine(space + "delay(" + d.delay.ToString() + ");");
                    if (i != m.Events.Count - 1)
                        writer.WriteLine();
                }
                else if (m.Events[i] is ME_Flag)
                {
                    for (int k = i; k < m.Events.Count; k++)
                    {
                        if (m.Events[k] is ME_Goto)
                        {
                            if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_Goto)m.Events[k]).name) == 0)
                            {
                                ME_Goto g = (ME_Goto)m.Events[k];

                                string for_var = g.name + "_" + flag_count.ToString();
                                writer.Write(space + "int " + for_var + " = 0;\n" + space + "flag_" + for_var + ":\n\n");
                                ((ME_Flag)m.Events[i]).var = for_var;

                                space_num += 2;
                                space = set_space(space_num);
                                flag_count++;
                            }
                        }
                    }
                }
                else if (m.Events[i] is ME_Goto)
                {
                    ME_Goto g = (ME_Goto)m.Events[i];
                    if (g.is_goto)
                    {
                        for (int k = 0; k < i; k++)
                        {
                            if (m.Events[k] is ME_Flag)
                            {
                                if (String.Compare(g.name, ((ME_Flag)m.Events[k]).name) == 0)
                                {
                                    string for_var = ((ME_Flag)m.Events[k]).var;
                                    space_num -= 2;
                                    space = set_space(space_num);
                                    if (((ME_Goto)m.Events[i]).infinite == false)
                                    {
                                        writer.Write(space + "if(" + for_var + "++ < " +
                                                     g.loops + ") goto flag_" + for_var + ";\n\n");
                                    }
                                    else
                                    {
                                        writer.WriteLine(space + "goto flag_" + for_var + ";\n");
                                    }
                                }
                            }
                        }
                    }
                } //ME_Goto
            }
            writer.WriteLine("}");

            writer.Dispose();
            writer.Close();
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (ME_Motionlist == null || MotionCombo.SelectedItem == null)
            {
                MessageBox.Show("You should add/select a motion name first");
                return;
            }

            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();
            string txtPath = path.SelectedPath;
            List<int> channels = new List<int>();
            int count = 0;
            bool add_channel = true;
            TextWriter writer;

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_Frame)
                    {
                        int ch_count = 0;
                        nfilename = txtPath + "\\86frame_" + count.ToString() + ".txt";
                        writer = new StreamWriter(nfilename);
                        ME_Frame f = (ME_Frame)m.Events[j];
                        for (int k = 0; k < 45; k++)
                        {
                            if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                            {
                                writer.Write("channel");
                                writer.Write(ch_count.ToString() + "=");
                                writer.WriteLine(f.frame[k].ToString());
                                if(add_channel)
                                    channels.Add(k);
                                ch_count++;
                            }
                        }
                        ((ME_Frame)m.Events[j]).num = count;
                        add_channel = false;
                        writer.Dispose();
                        writer.Close();
                        count++;
                    }
                }
                nfilename = txtPath + "\\86offset" + ".txt";
                writer = new StreamWriter(nfilename);
                int offset_count = 0;
                for (int i = 0; i < 45; i++)
                {
                    if (offset[i] != 0 && String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        writer.WriteLine("channel" + offset_count.ToString() + "=" + offset[i].ToString());
                    if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        offset_count++;
                }

                writer.Dispose();
                writer.Close();
                generate_ino(txtPath, channels, count);
                MessageBox.Show("Generating complete");
                reset_goto_parsed();
            }
        }

        public bool have_86()
        {
            ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2");
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PnPEntity");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject Obj in queryCollection)
            {
                if (Obj["Name"] != null && Obj["Name"].ToString().Contains("86Duino"))
                {
                    return true;
                }
            }
            com_port = "OFF";
            MessageBox.Show("Cannot find 86Duino, entering offline mode", "",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about a = new about();
            a.ShowDialog();
        }

        private void GenerateAllInOne_Click(object sender, EventArgs e)
        {
            if (ME_Motionlist == null || MotionCombo.SelectedItem == null)
            {
                MessageBox.Show("You should add/select a motion name first");
                return;
            }

            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();
            string txtPath = path.SelectedPath;
            List<int> channels = new List<int>();
            List<int> angle = new List<int>();
            int count = 0;
            bool add_channel = true;

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                Directory.CreateDirectory(path.SelectedPath + "\\AllinOne_Motion_Sketch");
                nfilename = path.SelectedPath + "\\AllinOne_Motion_Sketch\\AllinOne_Motion_Sketch.ino";
                TextWriter writer = new StreamWriter(nfilename);
                // include and declare
                writer.WriteLine("#include <Servo86.h>");
                writer.WriteLine();

                ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_Frame)
                    {
                        ME_Frame f = (ME_Frame)m.Events[j];
                        for (int k = 0; k < 45; k++)
                        {
                            if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                            {
                                angle.Add(f.frame[k]);
                                if (add_channel)
                                    channels.Add(k);
                            }
                        }
                        add_channel = false;
                        ((ME_Frame)m.Events[j]).num = count;
                        count++;
                    }
                }

                for (int i = 0; i < channels.Count; i++)
                    writer.WriteLine("Servo myservo" + channels[i].ToString() + ";");

                writer.WriteLine();
                writer.WriteLine("ServoOffset myoffs;");
                writer.WriteLine();

                for( int i=0; i < count; i++)
                    writer.WriteLine("ServoFrame frm" + i.ToString() + ";");

                writer.WriteLine();

                // setup
                writer.WriteLine("void setup()");
                writer.WriteLine("{");
                for (int i = 0; i < channels.Count; i++)
                    writer.WriteLine("  myservo" + channels[i].ToString() + ".attach(" + channels[i].ToString() + ");");
                writer.WriteLine();

                int offset_count = 0;
                for (int i = 0; i < 45; i++)
                {
                    if (offset[i] != 0 && String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        writer.WriteLine("  myoffs.offsets[" + offset_count.ToString() + "] = " + offset[i].ToString() + ";");
                    if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        offset_count++;
                }
                writer.WriteLine();

                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < channels.Count; j++)
                    {
                        writer.WriteLine("  frm" + i.ToString() + ".positions[" + j.ToString() + "] = " + angle[i * channels.Count + j] + ";");
                    }
                    writer.WriteLine();
                }

                writer.WriteLine("  myoffs.setOffsets();");

                writer.WriteLine("}");
                writer.WriteLine();

                // loop
                writer.WriteLine("void loop()");
                writer.WriteLine("{");
                int space_num = 2;
                string space = set_space(space_num);
                for (int i = 0, flag_count = 0; i < m.Events.Count; i++)
                {
                    if (m.Events[i] is ME_Frame)
                    {
                        ME_Frame f = (ME_Frame)m.Events[i];
                        writer.WriteLine(space + "frm" + f.num.ToString() + ".playPositions(" + f.delay.ToString() + ");");
                        writer.WriteLine(space + "while(isServoMultiMoving() == true);");
                        if (i != m.Events.Count - 1)
                            writer.WriteLine();
                    }
                    else if (m.Events[i] is ME_Delay)
                    {
                        ME_Delay d = (ME_Delay)m.Events[i];
                        writer.WriteLine(space + "delay(" + d.delay.ToString() + ");");
                        if (i != m.Events.Count - 1)
                            writer.WriteLine();
                    }
                    else if (m.Events[i] is ME_Flag)
                    {
                        for (int k = i; k < m.Events.Count; k++)
                        {
                            if (m.Events[k] is ME_Goto)
                            {
                                if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_Goto)m.Events[k]).name) == 0)
                                {
                                    ME_Goto g = (ME_Goto)m.Events[k];

                                    string for_var = g.name + "_" + flag_count.ToString();
                                    writer.Write(space + "int " + for_var + " = 0;\n" + space + "flag_" + for_var + ":\n\n");
                                    ((ME_Flag)m.Events[i]).var = for_var;

                                    space_num += 2;
                                    space = set_space(space_num);
                                    flag_count++;
                                }
                            }
                        }
                    }
                    else if (m.Events[i] is ME_Goto)
                    {
                        ME_Goto g = (ME_Goto)m.Events[i];
                        if (g.is_goto)
                        {
                            for (int k = 0; k < i; k++)
                            {
                                if (m.Events[k] is ME_Flag)
                                {
                                    if (String.Compare( g.name, ((ME_Flag)m.Events[k]).name) == 0)
                                    {
                                        string for_var = ((ME_Flag)m.Events[k]).var;
                                        space_num -= 2;
                                        space = set_space(space_num);
                                        if (((ME_Goto)m.Events[i]).infinite == false)
                                        {
                                            writer.Write(space + "if(" + for_var + "++ < " +
                                                         g.loops + ") goto flag_" + for_var + ";\n\n");
                                        }
                                        else
                                        {
                                            writer.WriteLine(space + "goto flag_" + for_var + ";\n");
                                        }
                                    }
                                }
                            }
                        }
                    } //ME_Goto
                }
                writer.WriteLine("}");

                MessageBox.Show("Generating complete");
                writer.Dispose();
                writer.Close();
                reset_goto_parsed();
            }
        }

        private void reset_goto_parsed()
        {
            ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
            for (int i = 0; i < m.Events.Count; i++)
                if (m.Events[i] is ME_Goto)
                    ((ME_Goto)m.Events[i]).parsed = false;
        }

        private string set_space(int n)
        {
            string ret_str = "";
            for (int i = 0; i < n; i++)
                ret_str += " ";
            return ret_str;
        }

        private void draw_background()
        {
            if(Motion.picfilename != null)
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(Motion.picfilename);
                }
                catch
                {
                    MessageBox.Show("Cannot load background image");
                }
            }
            Framelist.Controls.Add(pictureBox1);
        }

        private void update_motionlist()
        {
            if (autocheck.Checked == true)
            {
                autocheck.Checked = false;
            }
            groupBox1.Enabled = false;
            groupBox4.Enabled = false;
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
                            ME_Delay d = (ME_Delay)m.Events[j];
                            Motionlist.Items.Add("[Delay]");
                        }
                        else if (m.Events[j] is ME_Sound)
                        {
                            ME_Sound s = (ME_Sound)m.Events[j];
                            Motionlist.Items.Add("[Sound] " + s.filename);

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
            Motionlist.SelectedIndex = current_motionlist_idx - 1;
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
            Motionlist.SelectedIndex = current_motionlist_idx + 1;
        }
    }
}