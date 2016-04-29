using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace _86ME_ver1
{
    public partial class NewMotion : Form
    {
        public Dictionary<string, string> NewMotion_lang_dic;
        public Arduino arduino = null;
        public Panel[] fpanel = new Panel[45];
        public Label[] flabel = new Label[45];
        public ComboBox[] fbox = new ComboBox[45];
        public MaskedTextBox[] ftext = new MaskedTextBox[45];
        public MaskedTextBox[] ftext2 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext3 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext4 = new MaskedTextBox[45];
        public CheckBox[] fcheck = new CheckBox[45];
        public HScrollBar[] fbar_off = new HScrollBar[45];
        public HScrollBar[] fbar_home = new HScrollBar[45];
        int[] offset = new int[45];
        int last_index;
        uint[] homeframe = new uint[45];
        uint[] Max = new uint[45];
        uint[] min = new uint[45];
        char[] delimiterChars = { ' ', '\t', '\r', '\n' };
        public string picfilename = null;
        public int[] channelx = new int[45];
        public int[] channely = new int[45];
        public bool newflag = false;
        string last_picfilename = null;
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
                                                    "LSM330DLC",
                                                    "RM-G146",
                                                    });
            comboBox2.SelectedIndex = 0;

            for (int i = 0; i < 45; i++)
            {
                offset[i] = 0;
                min[i] = 600;
                Max[i] = 2400;
                homeframe[i] = 1500;
                channelx[i] = 0;
                channely[i] = 0;
            }
            create_panel(0, 45, 0);
            applyLang();
        }

        public void applyLang()
        {
            this.Text = NewMotion_lang_dic["NewMotion_title"];
            checkBox2.Text = NewMotion_lang_dic["NewMotion_checkBox2_Text"];
            label1.Text = NewMotion_lang_dic["NewMotion_label1_Text"];
            label2.Text = NewMotion_lang_dic["NewMotion_label2_Text"];
            label3.Text = NewMotion_lang_dic["NewMotion_label3_Text"];
            button3.Text = NewMotion_lang_dic["NewMotion_button3_Text"];
            ttp.SetToolTip(button3, NewMotion_lang_dic["NewMotion_loadpic_ToolTip"]);
            ttp.SetToolTip(checkBox2, NewMotion_lang_dic["NewMotion_minMax_ToolTip"]);
            for (int i = 0; i < 45; i++)
            {
                fcheck[i].Text = NewMotion_lang_dic["NewMotion_fcheckText"];
                ttp.SetToolTip(fcheck[i], NewMotion_lang_dic["NewMotion_fcheck_ToolTip"]);
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
            pic_loaded.Text = last_picfilename;
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.Compare(comboBox1.SelectedItem.ToString(), "---unset---") == 0)
                MessageBox.Show(NewMotion_lang_dic["NewMotion_err1"]);
            else
                this.DialogResult = DialogResult.OK;
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            last_index = comboBox1.SelectedIndex;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (last_index == comboBox1.SelectedIndex)
            {
            }
            //add 86duino series into the combobox
            else if (string.Compare(comboBox1.Text, "86Duino_One") == 0)
            {
                for (int i = 0; i < 45; i++ )
                {
                    if (fpanel[i] != null)
                        fpanel[i].Controls.Clear();
                }
                channelver.Controls.Clear();
                create_panel(0, 45, 0);
            }
            else if (string.Compare(comboBox1.Text, "86Duino_Zero") == 0)
            {
                for (int i = 0; i < 45; i++)
                {
                    if (fpanel[i] != null)
                        fpanel[i].Controls.Clear();
                }
                channelver.Controls.Clear();
                create_panel(0, 14, 0);
                create_panel(42, 45, 14);
            }
            else if (string.Compare(comboBox1.Text, "86Duino_EduCake") == 0)
            {
                for (int i = 0; i < 45; i++)
                {
                    if (fpanel[i] != null)
                        fpanel[i].Controls.Clear();
                }
                channelver.Controls.Clear();
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
            for (int i=0; i<45; i++)
            {
                offset[i] = int.Parse(ftext[i].Text);
                homeframe[i] = uint.Parse(ftext2[i].Text);
                min[i] = uint.Parse(ftext3[i].Text);
                Max[i] = uint.Parse(ftext4[i].Text);
            }
        }

        private void create_panel(int low, int high, int start_pos)
        {
            for (int i = low; i < high; i++, start_pos++)
            {
                fpanel[i] = new Panel();
                flabel[i] = new Label();
                fbox[i] = new ComboBox();
                ftext[i] = new MaskedTextBox();
                ftext2[i] = new MaskedTextBox();
                ftext3[i] = new MaskedTextBox();
                ftext4[i] = new MaskedTextBox();
                fcheck[i] = new CheckBox();
                fbar_off[i] = new HScrollBar();
                fbar_home[i] = new HScrollBar();

                fpanel[i].Size = new Size(520, 50);
                fpanel[i].Top += 5 + start_pos * 50;

                flabel[i].Size = new Size(65, 18);
                flabel[i].Top += 5;
                flabel[i].Left += 5;

                fbox[i].DropDownStyle = ComboBoxStyle.DropDownList;
                fbox[i].Size = new Size(135, 22);
                fbox[i].Left += 70;

                fcheck[i].Top += 24;
                fcheck[i].Left += 125;
                fcheck[i].Size = new Size(75, 22);
                fcheck[i].Text = NewMotion_lang_dic["NewMotion_fcheckText"];
                fcheck[i].Name = i.ToString();
                fcheck[i].Checked = false;
                fcheck[i].Enabled = false;
                fcheck[i].CheckedChanged += new EventHandler(autocheck_changed);
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
                fbox[i].SelectedIndexChanged += new EventHandler(this.motors_SelectedIndexChanged);
                if (i < 10)
                    flabel[i].Text = "SetServo " + i.ToString() + ":";
                else
                    flabel[i].Text = "SetServo" + i.ToString() + ":";

                fpanel[i].Controls.Add(flabel[i]);
                fpanel[i].Controls.Add(fbox[i]);
                fpanel[i].Controls.Add(ftext[i]);
                fpanel[i].Controls.Add(ftext2[i]);
                fpanel[i].Controls.Add(ftext3[i]);
                fpanel[i].Controls.Add(ftext4[i]);
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
                if (short_picfilename.Length < 16)
                    pic_loaded.Text = short_picfilename;
                else
                    pic_loaded.Text = short_picfilename.Substring(0, 13) + "...";
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
                    fbar_off[i].Value = n;
                if(arduino != null && fcheck[i].Checked == true)
                {
                    try
                    {
                        int[] autoframe = new int[45];
                        autoframe[i] = n + (int)(uint.Parse(ftext2[i].Text));
                        arduino.frameWrite(0x6F, autoframe, 0);
                    }
                    catch
                    {
                        MessageBox.Show(NewMotion_lang_dic["errorMsg1"]);
                    }
                }
            }
        }

        private void check_homeframe(object sender, EventArgs e)
        {
            uint n;
            int i = int.Parse(((MaskedTextBox)sender).Name);

            if (uint.TryParse(((MaskedTextBox)sender).Text, out n))
            {
                if (n >= uint.Parse(ftext3[i].Text) && n <= uint.Parse(ftext4[i].Text))
                {
                    fbar_home[i].Value = (int)n;
                    if (arduino != null && fcheck[i].Checked == true)
                    {
                        try
                        {
                            {
                                int[] autoframe = new int[45];
                                autoframe[i] = (int)n + (int.Parse(ftext[i].Text));
                                arduino.frameWrite(0x6F, autoframe, 0);
                            }
                        }
                        catch
                        {
                            MessageBox.Show(NewMotion_lang_dic["errorMsg1"]);
                        }
                    }
                }
            }
            else
            {
                ((MaskedTextBox)sender).Text = "1500";
            }
        }

        private void check_range(object sender, EventArgs e)
        {
            int i = int.Parse(((MaskedTextBox)sender).Name);
            fbar_home[i].Minimum = int.Parse(ftext3[i].Text);
            fbar_home[i].Maximum = int.Parse(ftext4[i].Text) + 9;
        }

        private void autocheck_changed(object sender, EventArgs e)
        {
            uint n;
            int i = int.Parse(((CheckBox)sender).Name);
            if(fcheck[i].Checked == true && arduino != null)
            {
                if (uint.TryParse(ftext2[i].Text, out n))
                {
                    try
                    {
                        if (n >= uint.Parse(ftext3[i].Text) && n <= uint.Parse(ftext4[i].Text))
                        {
                            int[] autoframe = new int[45];
                            autoframe[i] = (int)n + (int.Parse(ftext[i].Text));
                            arduino.frameWrite(0x6F, autoframe, 0);
                        }
                    }
                    catch
                    {
                        MessageBox.Show(NewMotion_lang_dic["errorMsg1"]);
                    }
                }
            }
        }

    }
}
