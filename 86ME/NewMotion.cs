using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace _86ME_ver1._0
{
    public partial class NewMotion : Form
    {
        public Panel[] fpanel = new Panel[45];
        public Label[] flabel = new Label[45];
        public ComboBox[] fbox = new ComboBox[45];
        public MaskedTextBox[] ftext = new MaskedTextBox[45];
        public MaskedTextBox[] ftext2 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext3 = new MaskedTextBox[45];
        public MaskedTextBox[] ftext4 = new MaskedTextBox[45];
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
        public NewMotion()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(new object[] { //"---unset---",
                                                    //"RB_100b1",
                                                    //"RB_100b2",
                                                    //"RB_100b3",
                                                    //"RB_100",
                                                    //"RB_100RD",
                                                    //"RB_110",
                                                    "86Duino_One",
                                                    "86Duino_Zero",
                                                    "86Duino_EduCake",
                                                    //"unknow"
                                                    });
            comboBox1.SelectedIndex = 0;

            for (int i = 0; i < 45; i++)
            {
                offset[i] = 0;
                min[i] = 600;
                Max[i] = 2300;
                homeframe[i] = 1500;
                channelx[i] = 0;
                channely[i] = 0;
            }

            create_panel(0, 45, 0);
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
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8 & e.KeyChar != (char)('-'))
            {
                e.Handled = true;
            }
            if (((MaskedTextBox)sender).Text == "-" && e.KeyChar == (char)('-'))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.Compare(comboBox1.SelectedItem.ToString(), "---unset---") == 0)
                MessageBox.Show("Error:\nYou have not choice your Board version.");
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                for (int i = 0; i < 45; i++)
                {
                    ftext2[i].Enabled = true;
                }
            }
            else
            {
                for (int i = 0; i < 45; i++)
                {
                    ftext2[i].Enabled = false;
                }
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
                fpanel[i].Size = new Size(520, 30);
                fpanel[i].Top += start_pos * 30;
                flabel[i].Size = new Size(65, 18);
                flabel[i].Top += 5;
                flabel[i].Left += 5;
                fbox[i].Size = new Size(185, 22);
                fbox[i].Left += 70;
                ftext[i].Text = offset[i].ToString();
                ftext[i].TextAlign = HorizontalAlignment.Right;
                ftext[i].KeyPress += new KeyPressEventHandler(numbercheck_offset);
                ftext[i].Size = new Size(50, 22);
                ftext[i].Left += 270;
                ftext2[i].Text = homeframe[i].ToString();
                ftext2[i].TextAlign = HorizontalAlignment.Right;
                ftext2[i].KeyPress += new KeyPressEventHandler(numbercheck);
                ftext2[i].Size = new Size(50, 22);
                ftext2[i].Left += 350;
                ftext2[i].Enabled = false;

                ftext3[i].Text = min[i].ToString();
                ftext3[i].TextAlign = HorizontalAlignment.Right;
                ftext3[i].KeyPress += new KeyPressEventHandler(numbercheck);
                ftext3[i].Size = new Size(40, 22);
                ftext3[i].Left += 410;
                ftext3[i].Enabled = false;

                ftext4[i].Text = Max[i].ToString();
                ftext4[i].TextAlign = HorizontalAlignment.Right;
                ftext4[i].KeyPress += new KeyPressEventHandler(numbercheck);
                ftext4[i].Size = new Size(40, 22);
                ftext4[i].Left += 460;
                ftext4[i].Enabled = false;
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
                                                      "DMP_RS0263",         
                                                      "DMP_RS1270",         
                                                      "GWS_S777",           
                                                      "GWS_S03T",           
                                                      "GWS_MICRO"});
                fbox[i].SelectedIndex = 0;
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
                channelver.Controls.Add(fpanel[i]);
            }
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
                newflag = true;
            }
            else
            {
                picfilename = null;

            }
        }
    }
}
