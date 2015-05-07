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

namespace _86ME_ver1._0
{
    public partial class Form1 : Form
    {
        public string com_port;
        Arduino arduino;
        private Panel[] fpanel = new Panel[45];
        Label[] flabel = new Label[45];
        MaskedTextBox[] ftext = new MaskedTextBox[45];
        HScrollBar[] fbar = new HScrollBar[45];
        NewMotion Motion;
        //public ArrayList ME_Triggerlist;
        public ArrayList ME_Motionlist;
        int framecount = 0;
        Boolean new_obj = false;
        String nfilename = "";
        string picture_name;
        string gotoflag_name;
        uint[] homeframe = new uint[45];
        uint[] Max = new uint[45];
        uint[] min = new uint[45];
        uint[] autoframe = new uint[45];
        int[] offset = new int[45];
        int board_ver86;
        int[] motor_info = new int[45];
        int mdx, mdy;
        bool freshflag;
        bool picmode_move = false;
        bool[] captured = new bool[45];
        string motiontestkey = "";
        string[] motionevent = {"Add new action at the next field",
                                "Delete action",
                                "Move action UP",
                                "Move action DOWN",
                                "Copy frame",
                                "Add new action at the first field"};
        char[] delimiterChars = { ' ', '\t', '\r', '\n' };
        public Form1()
        {
            InitializeComponent();
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            saveFileToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
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
                    fpanel[i] = new Panel();
                    flabel[i] = new Label();
                    ftext[i] = new MaskedTextBox();
                    fbar[i] = new HScrollBar();
                    fpanel[i].Size = new Size(260, 30);
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
                    flabel[i].Size = new Size(40, 18);
                    flabel[i].BackColor = Color.White;
                    flabel[i].Top += 3;
                    flabel[i].Left += 5;
                    ftext[i].Size = new Size(45, 22);
                    ftext[i].Left += 45;
                    ftext[i].TextAlign = HorizontalAlignment.Right;

                    flabel[i].Name = i.ToString();
                    ftext[i].Name = i.ToString();

                    if (Motion.picfilename != null)
                    {
                        flabel[i].MouseDown += new MouseEventHandler(flMouseDown);
                        flabel[i].MouseMove += new MouseEventHandler(flMouseMove);
                    }
                    ftext[i].TextChanged += new EventHandler(Text_Changed);
                    ftext[i].KeyPress += new KeyPressEventHandler(numbercheck);
                    fbar[i].Size = new Size(160, 22);
                    fbar[i].Left += 95;
                    if (Motion.checkBox2.Checked == true)
                    {
                        fbar[i].Maximum = (int)(Max[i] + 9);
                        fbar[i].Minimum = (int)min[i];
                    }
                    else {
                        fbar[i].Maximum = 2409;
                        fbar[i].Minimum = 600;
                    }
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
                            if (Motion.checkBox2.Checked == true)
                            {
                                if (int.Parse(ftext[i].Text) <= Max[i] && int.Parse(ftext[i].Text) >= min[i])
                                    fbar[i].Value = int.Parse(ftext[i].Text);
                            }
                            else
                            {
                                if (int.Parse(ftext[i].Text) <= 2400 && int.Parse(ftext[i].Text) >= 600)
                                    fbar[i].Value = int.Parse(ftext[i].Text);
                            }
                        }
                    }
                    else
                    {
                        ftext[i].Text = "0";
                    }
                    if (i < 10)
                        flabel[i].Text = "CH " + i.ToString() + ":";
                    else
                        flabel[i].Text = "CH" + i.ToString() + ":";

                    fpanel[i].Controls.Add(flabel[i]);
                    fpanel[i].Controls.Add(ftext[i]);
                    fpanel[i].Controls.Add(fbar[i]);
                    Framelist.Controls.Add(fpanel[i]);
                    
                    count++;
                }
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
        public void scroll_event(object sender, ScrollEventArgs e){   //Scroll event
             //if (e.Type == ScrollEventType.EndScroll)
                this.ftext[int.Parse(((HScrollBar)sender).Name)].Text = ((HScrollBar)sender).Value.ToString();            
        }
        public void Text_Changed(object sender, EventArgs e) //Text event
        {
            int n;
            if (Motion.checkBox2.Checked == true)
            {
                if (int.Parse(((MaskedTextBox)sender).Text) <= Max[int.Parse(((MaskedTextBox)sender).Name)] && int.Parse(((MaskedTextBox)sender).Text) >= min[int.Parse(((MaskedTextBox)sender).Name)])
                {
                    this.fbar[int.Parse(((MaskedTextBox)sender).Name)].Value = int.Parse(((MaskedTextBox)sender).Text);
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                autoframe[i] = uint.Parse(ftext[i].Text);
                            }
                        }
                        if (!freshflag)
                        {
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
            }
            else if((((MaskedTextBox)sender).Text) == "")
            {
                (((MaskedTextBox)sender).Text) = "0";
            }
            else
            {
                if (int.Parse(((MaskedTextBox)sender).Text) <= 2400 && int.Parse(((MaskedTextBox)sender).Text) >= 900)
                {
                    this.fbar[int.Parse(((MaskedTextBox)sender).Name)].Value = int.Parse(((MaskedTextBox)sender).Text);
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                autoframe[i] = uint.Parse(ftext[i].Text);
                            }
                        }
                        if (!freshflag)
                        {
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
            nMotion.ShowDialog();
            if (nMotion.DialogResult == DialogResult.OK)
            {
                Motion = nMotion;
                groupBox2.Enabled = true;
                groupBox3.Enabled = true;
                saveFileToolStripMenuItem.Enabled = true;
                editToolStripMenuItem.Enabled = true;
                NewMotion.Enabled = false;
                //ME_Triggerlist = new ArrayList();
                ME_Motionlist = new ArrayList();
                //triggerlist.Items.Clear();
                MotionCombo.Items.Clear();
                MotionCombo.Text = "";
                Motionlist.Items.Clear();
                delaytext.Text = "0";
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
                        nMotion.ftext4[i].Text = "2300";
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
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Cannot open 86Duino, entering offline mode");
                            }
                        }
                        else
                        {
                            try
                            {
                                arduino = new Arduino(com_port);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Cannot open 86Duino, entering offline mode");
                            }
                        }
                    }
                }

                if(pictureBox1.Image != null)
                    pictureBox1.Image = null;
                if (nMotion.picfilename != null)
                {
                    picture_name = nMotion.picfilename;
                    draw_background();
                }

                this.richTextBox1.Text = "\t\t\t\t    2.press Add Motion --->\n\n\n\t\t\t\t 1.Enter a Motion Name --->";
            }
        }
        private void optionToolStripMenuItem_Click(object sender, EventArgs e)  //option
        {
            autocheck.Checked = false;
            Motion.ShowDialog();
            if (Motion.DialogResult == DialogResult.OK)
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
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Cannot open 86Duino, entering offline mode");
                            }
                        }
                        else
                        {
                            try
                            {
                                arduino = new Arduino(com_port);
                            }
                            catch
                            {
                                com_port = "OFF";
                                MessageBox.Show("Cannot open 86Duino, entering offline mode");
                            }
                        }
                    }
                }
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
                Update_framelist();
                draw_background();
                for (int i = 0; i < 45; i++)
                {
                    if (Motion.ftext[i].Text == "")
                        Motion.ftext[i].Text = "0";
                    if (Motion.ftext2[i].Text == "")
                        Motion.ftext2[i].Text = "1500";
                    if (Motion.ftext3[i].Text == "")
                        Motion.ftext3[i].Text = "600";
                    if (Motion.ftext4[i].Text == "")
                        Motion.ftext4[i].Text = "2300";
                    homeframe[i] = uint.Parse(Motion.ftext2[i].Text);
                    min[i] = uint.Parse(Motion.ftext3[i].Text);
                    Max[i] = uint.Parse(Motion.ftext4[i].Text);
                    motor_info[i] = Motion.fbox[i].SelectedIndex;
                    try
                    {
                        offset[i] = int.Parse(Motion.ftext[i].Text);
                    }
                    catch
                    {
                        offset[i] = 0;
                        Motion.ftext[i].Text = "0";
                        string error_msg = "The offset " + i.ToString() + " is illegal, set to 0";
                        MessageBox.Show(error_msg);
                    }
                }
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
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)    //save file
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

                writer.Write("RoBoardVer ");
                writer.Write(Motion.comboBox1.SelectedItem.ToString());
                writer.Write("\n");
                writer.Write("Homeframe ");
                if(Motion.checkBox1.Checked == true)
                    writer.Write("Yes");
                else
                    writer.Write("No");
                writer.Write("\n");
                writer.Write("Range ");
                if (Motion.checkBox2.Checked == true)
                    writer.Write("Yes");
                else
                    writer.Write("No");
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

                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    writer.Write("Motion " + m.name + "\n");
                    for (int j = 0; j < m.Events.Count; j++) {
                        if (m.Events[j] is ME_Frame) 
                        {
                            ME_Frame f=(ME_Frame)m.Events[j];
                            writer.Write("frame " + f.delay.ToString() + " ");
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
                            writer.Write("goto " + g.name + " " + g.key + "\n");

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

                for (int i = 0; i < 45; i++)
                {
                    if (uint.Parse(Motion.ftext[i].Text) != 0)
                    {
                        using (writer = new StreamWriter(nFilePath + "\\offset.txt"))
                        {
                            for (int j = 0; j < 45; j++)
                            {
                                if (string.Compare(Motion.ftext[j].Text, "") == 0)
                                {
                                    Motion.ftext[j].Text = "0";
                                }
                                writer.Write(Motion.ftext[j].Text + " ");
                            }
                        }
                        break;
                    }
                }
                for (int i = 0; i < 45; i++)
                {
                    if (uint.Parse(Motion.ftext2[i].Text) != 1500)
                    {
                        using (writer = new StreamWriter(nFilePath + "\\homeframe.txt"))
                        {
                            for (int j = 0; j < 45; j++)
                            {
                                if (string.Compare(Motion.ftext[j].Text, "") == 0)
                                {
                                    Motion.ftext2[j].Text = "1500";
                                }
                                writer.Write(Motion.ftext2[j].Text + " ");
                            }
                        }
                        break;
                    }
                }
                for (int i = 0; i < 45; i++)
                {
                    if (uint.Parse(Motion.ftext3[i].Text) != 600 || uint.Parse(Motion.ftext4[i].Text) != 2300)
                    {
                        using (writer = new StreamWriter(nFilePath + "\\Range.txt"))
                        {
                            for (int j = 0; j < 45; j++)
                            {
                                if (string.Compare(Motion.ftext3[j].Text, "") == 0)
                                {
                                    Motion.ftext3[j].Text = "600";
                                }
                                writer.Write(Motion.ftext3[j].Text + " ");
                            }
                            for (int j = 0; j < 45; j++)
                            {
                                if (string.Compare(Motion.ftext4[j].Text, "") == 0)
                                {
                                    Motion.ftext4[j].Text = "2400";
                                }
                                writer.Write(Motion.ftext4[j].Text + " ");
                            }
                        }
                        break;
                    }
                }
                if (Motion.picfilename != null)
                {
                    using (writer = new StreamWriter(nFilePath + "\\picmode.txt"))
                    {
                        writer.Write(Motion.picfilename + " ");
                        for (int i = 0; i < 45; i++)
                            writer.Write(Motion.channelx[i] + " ");
                        for (int i = 0; i < 45; i++)
                            writer.Write(Motion.channely[i] + " ");
                    }
                }
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
            NewMotion nMotion = new NewMotion();
            //ME_Triggerlist = new ArrayList();
            ME_Motionlist = new ArrayList();
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
                                            "GWS_MICRO"};
            ME_Motion motiontag = null;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "rbm files (*.rbm)|*.rbm";
            dialog.Title = "Open File";
            String filename = (dialog.ShowDialog() == DialogResult.OK) ? dialog.FileName : null;
            if (filename == null)
            {
                return;
            }
            using (StreamReader reader = new StreamReader(filename))
            {
                nfilename = Path.GetFileName(dialog.FileName);
                MotionCombo.Items.Clear();
                MotionCombo.Text = "";
                Motionlist.Items.Clear();
                delaytext.Text = "0";
                typecombo.Text = "";
                string[] datas = reader.ReadToEnd().Split(delimiterChars);
                for (int i = 0; i < datas.Length; i++)
                {
                    if (String.Compare(datas[i], "RoBoardVer") == 0)
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
                    else if (String.Compare(datas[i], "Homeframe") == 0)
                    {
                        i++;
                        if (String.Compare(datas[i], "Yes") == 0)
                            nMotion.checkBox1.Checked = true;
                        else
                            nMotion.checkBox1.Checked = false;
                    }
                    else if (String.Compare(datas[i], "Range") == 0)
                    {
                        i++;
                        if (String.Compare(datas[i], "Yes") == 0)
                            nMotion.checkBox2.Checked = true;
                        else
                            nMotion.checkBox2.Checked = false;
                    }
                    else if (String.Compare(datas[i], "Servo") == 0)
                    {

                        for (int k = 0; k < 45; k++)
                        {
                            i++;
                            for (int j = 0; j < servo.Length; j++)
                            {
                                if (String.Compare(datas[i], servo[j]) == 0)
                                {
                                    nMotion.fbox[k].SelectedIndex = j;
                                    motor_info[k] = j;
                                }
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
                        i++;
                        nframe.delay = int.Parse(datas[i]);
                        int j = 0;
                        while (j < 45)
                        {
                            if (String.Compare(nMotion.fbox[j].SelectedItem.ToString(), "---noServo---") != 0)
                            {
                                i++;
                                nframe.frame[j] = int.Parse(datas[i]);
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
                        ndelay.delay = int.Parse(datas[i]);
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
                        ngoto.key = datas[i];
                        motiontag.Events.Add(ngoto);
                    }
                }
            }

            string project_path = Path.GetDirectoryName(dialog.FileName);
            nMotion.load_prereference(project_path);
            if (nMotion.picfilename != null)
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
            
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            saveFileToolStripMenuItem.Enabled = true;
            NewMotion.Enabled = false;
            Motion = nMotion;
            for (int i = 0; i < 45; i++)
            {
                offset[i] = int.Parse(nMotion.ftext[i].Text);
                homeframe[i] = uint.Parse(nMotion.ftext2[i].Text);
                min[i] = uint.Parse(nMotion.ftext3[i].Text);
                Max[i] = uint.Parse(nMotion.ftext4[i].Text);
            }
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                MotionCombo.Items.Add(m.name);
            }
            this.richTextBox1.Text = "\n\n\n\t\t         Choose or New a Motion Name --->";
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
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show("Cannot open 86Duino, entering offline mode");
                        }
                    }
                    else
                    {
                        try
                        {
                            arduino = new Arduino(com_port);
                        }
                        catch
                        {
                            com_port = "OFF";
                            MessageBox.Show("Cannot open 86Duino, entering offline mode");
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MotionTest.Enabled = false;
        }

        private void typecombo_TextChanged(object sender, EventArgs e) // choose a type of action
        {
            Framelist.Controls.Clear();
            if (String.Compare(typecombo.Text, "Frame") == 0)
            {
                if (new_obj) {
                    Motionlist.Items.Insert(Motionlist.SelectedIndex+1,"[Frame] " + MotionCombo.SelectedItem.ToString() + "-" + framecount++.ToString());
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());

                    Motionlist.SelectedIndex++;

                    for (int i = 0; i < 45; i++)
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] = (int)homeframe[i];
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
                    this.richTextBox1.Text = "Tune the settings of motors\n↓\n↓\n↓\n↓\n↓\n↓";
                else
                    this.richTextBox1.Text = "1.Left click on tag \"Ch XX\" and drag to move it\n2.Tune the settings of motors\n↓\n↓\n↓\n↓\n↓";
            }
            else if (String.Compare(typecombo.Text, "Delay") == 0)
            {
                if (new_obj)
                {
                    Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Delay]");
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Delay());
                    Motionlist.SelectedIndex++;
                }
                delaytext.Enabled = true;
                label2.Enabled = true;
                Framelist.Enabled = false;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "\n\n\n\n<--- Set the delay you want";
            }
            else if (String.Compare(typecombo.Text, "Sound") == 0)
            {
                if (new_obj){
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
                gotoflag_name = "";
                delaytext.Text = "";
                delaytext.Enabled = false;
                label2.Enabled = false;
                Framelist.Enabled = true;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "Set the name of the flag\n↓\n↓\n↓\n↓\n↓\n↓";
            }
            else if (String.Compare(typecombo.Text, "Goto") == 0)
            {
                
                if(new_obj){
                    Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Goto]");
                    ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Goto());
                    Motionlist.SelectedIndex++;
                }
                gotoflag_name = "";
                delaytext.Text = "";
                delaytext.Enabled = false;
                label2.Enabled = false;
                Framelist.Enabled = true;
                capturebutton.Enabled = false;
                autocheck.Enabled= false;
                typecombo.Enabled = false;
                new_obj = false;
                this.richTextBox1.Text = "Set target Flag Name and hotkey of the Goto\n\n*HotKey must be set, or the Goto will be functionless*\n↓\n↓\n↓\n↓";
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
        }

        private void gototext(object sender, EventArgs e)// set names of Goto & Flag
        {
            gotoflag_name = ((MaskedTextBox)sender).Text;
            //if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Flag)
            //    ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = ((MaskedTextBox)sender).Text;
            //else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Goto)
            //    ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = ((MaskedTextBox)sender).Text;
        }

        private void gotobutton(object sender, EventArgs e)
        {
            TriggerSet ts = new TriggerSet(((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).key);
            ts.ShowDialog();
            if (ts.DialogResult == DialogResult.OK)
            {
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).key = ts.Keyvalue.Text;
                ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = gotoflag_name;
                update_motionlist();
            }
        }

        private void flagbutton(object sender, EventArgs e)
        {
            ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name = gotoflag_name;
            update_motionlist();
        }

        private void Motionlist_SelectedIndexChanged(object sender, EventArgs e) // select motionlist
        {
            if (Motionlist.SelectedItem != null && (MotionTest.Enabled))
            {
                groupBox1.Enabled = true;
                string[] datas = Motionlist.SelectedItem.ToString().Split(' ');
                if (String.Compare(datas[0], "[Frame]") == 0)
                {
                    typecombo.SelectedIndex = 0;
                    typecombo.Text = "Frame";
                    delaytext.Text = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                    
                    if (autocheck.Checked == true)
                    {
                        for (int i = 0; i < 45; i++)
                        {
                            if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                            {
                                autoframe[i] = (uint)(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i] + offset[i]);
                            }
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
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0 && fpanel[i] != null)
                        {
                            freshflag = true;
                            ftext[i].Text = ((uint)((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame[i]).ToString();// + offset[i]).ToString();
                        }
                    if (!freshflag)
                        Update_framelist();
                    freshflag = false;
                    Framelist.Enabled = true;
                    draw_background();
                }
                else if (String.Compare(datas[0], "[Delay]") == 0)
                {
                    typecombo.SelectedIndex = 1;
                    typecombo.Text = "Delay";
                    delaytext.Text = ((ME_Delay)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();
                    draw_background();
                }
                else if (String.Compare(datas[0], "[Sound]") == 0)
                {
                    typecombo.SelectedIndex = 2;
                    typecombo.Text = "Sound";
                    delaytext.Text = ((ME_Sound)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay.ToString();

                }
                else if (String.Compare(datas[0], "[Flag]") == 0)
                {
                    //typecombo.SelectedIndex = 4;
                    typecombo.SelectedIndex = 3;
                    typecombo.Text = "Flag";
                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = "Name: ";
                    xlabel.Size = new Size(40, 22);

                    MaskedTextBox xtext = new MaskedTextBox();
                    xtext.TextChanged += new EventHandler(gototext);
                    xtext.Text = ((ME_Flag)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    xtext.Size = new Size(160, 22);
                    xtext.Left += 45;
                    Button xbutton = new Button();
                    xbutton.Text = "set Flag";
                    xbutton.Click += new EventHandler(flagbutton);
                    xbutton.Left += 210;
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xtext);
                    Framelist.Controls.Add(xbutton);
                    Framelist.Enabled = true;
                    draw_background();
                }
                else if (String.Compare(datas[0], "[Goto]") == 0)
                {
                    //typecombo.SelectedIndex = 3;
                    typecombo.SelectedIndex = 2;
                    typecombo.Text = "Goto";
                    Framelist.Controls.Clear();
                    Label xlabel = new Label();
                    xlabel.Text = "Target Flag Name: ";
                    xlabel.Size = new Size(95, 22);
                    MaskedTextBox xtext = new MaskedTextBox();
                    xtext.TextChanged += new EventHandler(gototext);
                    xtext.Text = ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).name;
                    xtext.Size = new Size(160, 22);
                    xtext.Left += 100;
                    Button xbutton = new Button();
                    xbutton.Text = "set Hotkey";
                    xbutton.Click += new EventHandler(gotobutton);
                    xbutton.Left += 270;
                    if (((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).key == "")
                    {
                        this.richTextBox1.Text = "Set target Flag Name and hotkey of the Goto\n\n*HotKey must be set, or the Goto will be functionless*\n↓\n↓\n↓\n↓";
                    }
                    else
                    {
                        this.richTextBox1.Text = "Set target Flag Name and hotkey of the Goto\n↓\n↓\n↓\n↓\n↓\n↓";
                    }
                    Framelist.Controls.Add(xlabel);
                    Framelist.Controls.Add(xtext);
                    Framelist.Controls.Add(xbutton);
                    Framelist.Enabled = true;
                    draw_background();
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
                    contextMenuStrip1.Items.Add("Add new action at the first field");
                    contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    contextMenuStrip1.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    contextMenuStrip1.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                    Framelist.Enabled = false;
                }
                else if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex] is ME_Frame)
                {
                    for (int i = 0; i < motionevent.Length - 1; i++)
                        contextMenuStrip1.Items.Add(motionevent[i]);
                    contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(Motionlistevent);
                    contextMenuStrip1.Closed += new ToolStripDropDownClosedEventHandler(Motionlistcloseevent);
                    contextMenuStrip1.Show(new Point(System.Windows.Forms.Cursor.Position.X, System.Windows.Forms.Cursor.Position.Y));
                }
                else if (Motionlist.SelectedItem != null)
                {
                    for (int i = 0; i < motionevent.Length - 2; i++)
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
                    switch (i) {
                        case 0:
                            groupBox1.Enabled = true;
                            typecombo.Text = "Select type";
                            this.richTextBox1.Text = "\n\n<--- Choose a type of action you want";
                            break;
                        case 1:
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
                        case 2:
                            Framelist.Enabled = false;
                            n = Motionlist.SelectedIndex;
                            if (n == 0)
                                break;
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n - 1, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n + 1);
                            Motionlist.Items.Insert(n - 1,Motionlist.SelectedItem);
                            Motionlist.Items.RemoveAt(n + 1);
                            break;
                        case 3:
                            Framelist.Enabled = false;
                            n = Motionlist.SelectedIndex;
                            if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count <= n+1)
                                break;
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(n + 2, ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[n]);
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.RemoveAt(n);
                            Motionlist.Items.Insert(n + 2, Motionlist.SelectedItem);
                            Motionlist.Items.RemoveAt(n);
                            break;
                        case 4:
                            Framelist.Enabled = false;
                            Motionlist.Items.Insert(Motionlist.SelectedIndex + 1, "[Frame] " + MotionCombo.SelectedItem.ToString() + "-" + framecount++.ToString());
                            ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Insert(Motionlist.SelectedIndex + 1, new ME_Frame());
                            Array.Copy(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).frame, ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).frame,45);
                            ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex + 1]).delay = ((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[Motionlist.SelectedIndex]).delay;
                            Motionlist.SelectedIndex++;
                            break;
                        case 5:
                            groupBox1.Enabled = true;
                            typecombo.Text = "Select type";
                            this.richTextBox1.Text = "\n\n<--- Choose a type of action you want";
                            break;
                    }
        }
        private void Motionlistcloseevent(object sender, EventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            contextMenuStrip1.ItemClicked -= Motionlistevent;
            contextMenuStrip1.Closed -= Motionlistcloseevent;
        }

        private void MotionCombo_TextChanged(object sender, EventArgs e)
        {
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
            groupBox1.Enabled = false;
            this.richTextBox1.Text = "\t\t\t\t    2.press Add Motion --->\n\n\n\t\t\t\t 1.Enter a Motion Name --->";
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
                this.richTextBox1.Text = "\n\n\n\n\n\nRight click in the white region and add an action --->";
            }
            else
                MessageBox.Show("Motion name should without space.");
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool close = true;
            if (Motion != null)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to save this project?", "exit", MessageBoxButtons.YesNoCancel);
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
            if (Motion != null && close)
            {
                if (arduino != null)
                    arduino.Close();
            }
        }        
        private void capturebutton_Click(object sender, EventArgs e)
        {
            if (string.Compare(com_port, "OFF") != 0)
            {
                bool have_cap = false;
                capturebutton.Enabled = false;
                autocheck.Checked = false;
                uint[] frame = new uint[45];
                servo_captured();
                for (int i = 0; i < 45; i++)
                {
                    if (captured[i])
                    {
                        have_cap = true;
                        arduino.frame_capture(i);
                        Thread.Sleep(100);
                        ftext[i].Text = arduino.captured_data.ToString();
                    }
                }
                capturebutton.Enabled = true;
                if (have_cap == false)
                    this.richTextBox1.Text = "\n\n\n\tThe choosed motors don't support Capture";
            }
        }

        private void servo_captured()
        {
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
            }
        }

        private void Motionlist_KeyDown(object sender, KeyEventArgs e)
        {
            string a, b;
            if (e.Modifiers == Keys.Control)
                a = "ctrl";
            else if (e.Modifiers == Keys.Alt)
                a = "alt";
            else if (e.Modifiers == Keys.Shift)
                a = "shift";
            else
                a = "";
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                b = ((char)e.KeyCode).ToString();
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                b = ((char)e.KeyCode).ToString();
            else if (e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F12)
                b = "F" + ((int)(e.KeyCode - Keys.F1 + 1)).ToString();
            else if (e.KeyCode == Keys.Up)
                b = "Up";
            else if (e.KeyCode == Keys.Down)
                b = "Down";
            else if (e.KeyCode == Keys.Left)
                b = "Left";
            else if (e.KeyCode == Keys.Right)
                b = "Right";
            else if (e.KeyCode == Keys.Escape)
                b = "ESC";
            else
                b = "";

            if (String.Compare(a, "") != 0 && String.Compare(b, "") != 0)
                motiontestkey = (a + "+" + b);
            else if (String.Compare(b, "") != 0)
                motiontestkey = (b);
            else if (String.Compare(b, "") == 0)
                motiontestkey = (a);
            else
                motiontestkey = "";
        }
        private void Motionlist_KeyUp(object sender, KeyEventArgs e)
        {
            motiontestkey = "";
        }

        public void MotionOnTest()
        {
            if (autocheck.Checked == true)
            {
                autocheck.Checked = false;
            }

            MotionTest.Enabled = false;
            SoundPlayer sp = null;
            if (ME_Motionlist == null)
                return;

            this.richTextBox1.Text = "\n\nExecuting Motion Test ... ...\n\nIf the test doesn't stop automatically, long press ESC until stop";
            for (int j = 0; j < ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events.Count; j++)
            {
                if (string.Compare(motiontestkey, "ESC") == 0)
                    break;
                Motionlist.SelectedIndex = j;
                if (((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j] is ME_Frame)
                {
                    for (int i = 0; i < 45; i++)
                    {
                        if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        {
                            autoframe[i] = (uint)(((ME_Frame)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).frame[i] + offset[i]);
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
                    if (string.Compare(motiontestkey, ((ME_Goto)((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).Events[j]).key) == 0 && motiontestkey != "")
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
            }
            MotionTest.Enabled = true;
            Framelist.Enabled = false;
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

        private void autocheck_CheckedChanged(object sender, EventArgs e)
        {

            if (autocheck.Checked == true && int.Parse(delaytext.Text) < 0)
            {
                MessageBox.Show("Please set the correct delay time.");
                autocheck.Checked = false;
            }
            else if (autocheck.Checked == true)
            {
                for (int i = 0; i < 45; i++)
                {
                    if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                    {
                        autoframe[i] = (uint)(int.Parse(ftext[i].Text) + offset[i]);
                    }
                }

                autocheck.Enabled = false;
                autocheck.Capture = false;

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
                autocheck.Capture = true;
                autocheck.Enabled = true;
            }
            else if(autocheck.Checked == false)
            {
                if (string.Compare(com_port, "OFF") != 0)
                {
                    arduino.motor_release();
                    Thread.Sleep(100);
                }
            }
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.86duino.com/?p=11544");
        }

        public void generate_ino(string path, List<int> channels, int frame_count, List<int> frame_delay, List<int> total_event)
        {
            nfilename = path + "\\86Duino Motion Sketch" + ".ino";
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

            //void loop{}
            writer.WriteLine("void loop()");
            writer.WriteLine("{");

            for (int i = 0, j = 0; i < total_event.Count; i++)
            {
                if (total_event[i] == 0)
                {
                    string fc = j.ToString();
                    writer.WriteLine("  frm" + fc + ".playPositions(" + frame_delay[j].ToString() + ");");
                    writer.WriteLine("  while(isServoMultiMoving() == true);");
                    if(i != total_event.Count-1)
                        writer.WriteLine();
                    j++;
                }
                else
                {
                    if(total_event[i] == -1)
                        writer.WriteLine("  delay(0);");
                    else
                        writer.WriteLine("  delay(" + total_event[i] + ");");
                    if (i != total_event.Count - 1)
                        writer.WriteLine();
                }
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
            List<int> frame_delay = new List<int>();
            List<int> total_event = new List<int>();
            int count = 0;
            bool add_channel = true;
            TextWriter writer;

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_Frame)
                    {
                        int ch_count = 0;
                        total_event.Add(0);
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
                        add_channel = false;
                        frame_delay.Add(f.delay);
                        writer.Dispose();
                        writer.Close();
                        count++;
                    }
                    else if (m.Events[j] is ME_Delay)
                    {
                        ME_Delay d = (ME_Delay)m.Events[j];
                        if (d.delay == 0)
                            total_event.Add(-1);
                        else
                            total_event.Add(d.delay);
                    }
                    else
                    {
                        ;
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
                generate_ino(txtPath, channels, count, frame_delay, total_event);
                MessageBox.Show("Generate program complete");
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
            MessageBox.Show("Cannot find 86Duino, entering offline mode");
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
            List<int> frame_delay = new List<int>();
            List<int> total_event = new List<int>();
            List<int> angle = new List<int>();
            int count = 0;
            bool add_channel = true;

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                nfilename = path.SelectedPath + "\\AllinOne Motion Sketch.ino";
                TextWriter writer = new StreamWriter(nfilename);
                // include and declare
                writer.WriteLine("#include <Servo86.h>");
                writer.WriteLine();

                ME_Motion m = (ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex];
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_Frame)
                    {
                        total_event.Add(0);
                        ME_Frame f = (ME_Frame)m.Events[j];
                        for (int k = 0; k < 45; k++)
                        {
                            if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                            {
                                //f.frame[k];
                                angle.Add(f.frame[k]);
                                if (add_channel)
                                    channels.Add(k);
                            }
                        }
                        add_channel = false;
                        frame_delay.Add(f.delay);
                        count++;
                    }
                    else if (m.Events[j] is ME_Delay)
                    {
                        ME_Delay d = (ME_Delay)m.Events[j];
                        if (d.delay == 0)
                            total_event.Add(-1);
                        else
                            total_event.Add(d.delay);
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
                for (int i = 0, j = 0; i < total_event.Count; i++)
                {
                    if (total_event[i] == 0)
                    {
                        string fc = j.ToString();
                        writer.WriteLine("  frm" + fc + ".playPositions(" + frame_delay[j].ToString() + ");");
                        writer.WriteLine("  while(isServoMultiMoving() == true);");
                        if (i != total_event.Count - 1)
                            writer.WriteLine();
                        j++;
                    }
                    else
                    {
                        if (total_event[i] == -1)
                            writer.WriteLine("  delay(0);");
                        else
                            writer.WriteLine("  delay(" + total_event[i] + ");");
                        if (i != total_event.Count - 1)
                            writer.WriteLine();
                    }
                }
                writer.WriteLine("}");
                MessageBox.Show("Generate program complete");
                writer.Dispose();
                writer.Close();
            }
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
            Motionlist.Items.Clear();
            framecount = 0;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (String.Compare(MotionCombo.SelectedItem.ToString(), m.name.ToString()) == 0)
                {
                    for (int j = 0; j < m.Events.Count; j++)
                    {
                        if (m.Events[j] is ME_Frame)
                        {
                            Motionlist.Items.Add("[Frame] " + ((ME_Motion)ME_Motionlist[MotionCombo.SelectedIndex]).name + "-" + framecount);
                            framecount++;
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
        }
    }
}