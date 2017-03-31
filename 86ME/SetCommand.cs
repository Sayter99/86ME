using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace _86ME_ver2
{
    public partial class SetCommand : Form
    {
        List<ME_Trigger> global_cmd;
        List<ME_Trigger> local_cmd = new List<ME_Trigger>();
        Dictionary<string, string> cmd_lang_dic;
        ListViewItem last;
        ME_Trigger current_trigger;
        ArrayList ME_Motionlist;
        ArrayList used_element = new ArrayList();
        ArrayList tmp_element = new ArrayList();
        GlobalSettings gs;
        Arduino arduino;
        string com_port;
        NewMotion Motion;
        int cmd_num;
        int var_num;
        int board_ver86;

        public SetCommand(List<ME_Trigger> commands, Dictionary<string, string> lang_dic, ArrayList ME_List, GlobalSettings gs,
                          Arduino arduino, string com_port, NewMotion Motion, int var_num, int board_ver86)
        {
            InitializeComponent();
            this.cmd_lang_dic = lang_dic;
            this.global_cmd = commands;
            this.ME_Motionlist = ME_List;
            this.gs = gs;
            this.arduino = arduino;
            this.com_port = com_port;
            this.Motion = Motion;
            this.var_num = var_num;
            this.board_ver86 = board_ver86;
            for (int i = 0; i < commands.Count; i++)
            {
                local_cmd.Add(commands[i].Copy());
                cmd_num = cmd_num + 1;
                string[] row = { cmd_num.ToString(), commands[i].name };
                var cmdItem = new ListViewItem(row);
                CmdListView.Items.Add(cmdItem);
            }
            if (CmdListView.Items.Count > 0)
                CmdListView.Items[0].Selected = true;
            else
                MotionConfig.Enabled = false;

            accLXText.Name = "0";
            accHXText.Name = "1";
            accLYText.Name = "2";
            accHYText.Name = "3";
            accLZText.Name = "4";
            accHZText.Name = "5";
            accDurationText.Name = "6";

            initGSpins();
            initAnalog();
            ps2DATCombo.Text = gs.ps2pins[0];
            ps2CMDCombo.Text = gs.ps2pins[1];
            ps2ATTCombo.Text = gs.ps2pins[2];
            ps2CLKCombo.Text = gs.ps2pins[3];
            ESP8266CHPDCombo.Text = gs.esp8266_chpd;

            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.trigger_index != -1 && global_cmd[m.trigger_index].used && !used_element.Contains(m))
                {
                    used_element.Add(m);
                    tmp_element.Add(m.Copy());
                }
                for (int j = 0; j < m.Events.Count; j++)
                {
                    if (m.Events[j] is ME_If mif)
                    {
                        if (mif.left_var >= var_num + 58 && global_cmd[mif.left_var - var_num - 58].used && !used_element.Contains(mif))
                        {
                            used_element.Add(mif);
                            tmp_element.Add(mif.Copy());
                        }
                    }
                }
            }
        }

        private bool checkName()
        {
            if (!(new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9_]{1,20}$")).IsMatch(nameMaskedTextBox.Text))
            {
                if (nameMaskedTextBox.Text.Length < 20)
                    warningLabel.Text = cmd_lang_dic["errorMsg12"];
                else
                    warningLabel.Text = cmd_lang_dic["errorMsg13"];
                nameMaskedTextBox.Focus();
                nameMaskedTextBox.SelectAll();
            }
            else if (nameMaskedTextBox.Text.IndexOf(" ") == -1)
            {
                foreach (ListViewItem item in CmdListView.Items)
                {
                    if (string.Compare(item.SubItems[1].Text, "c_" + nameMaskedTextBox.Text) == 0)
                    {
                        warningLabel.Text = cmd_lang_dic["errorMsg26"];
                        nameMaskedTextBox.Focus();
                        nameMaskedTextBox.SelectAll();
                        return false;
                    }
                }
                return true;
            }
            else
            {
                warningLabel.Text = cmd_lang_dic["errorMsg14"];
                nameMaskedTextBox.Focus();
                nameMaskedTextBox.SelectAll();
            }
            return false;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            global_cmd.Clear();
            if (cmd_num > 0)
            {
                for (int i = 0; i < local_cmd.Count; i++)
                    global_cmd.Add(local_cmd[i]);
            }
            for (int i = 0; i < used_element.Count; i++)
            {
                if (used_element[i] is ME_Motion m)
                {
                    m.trigger_index = ((ME_Motion)tmp_element[i]).trigger_index;
                }
                else if (used_element[i] is ME_If mif)
                {
                    mif.left_var = ((ME_If)tmp_element[i]).left_var;
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void CmdListView_SelectedIndexChanged(object senderm, EventArgs e)
        {
            warningLabel.Text = "";
            if (CmdListView.SelectedItems.Count != 0)
            {
                CmdListView.SelectedItems[0].BackColor = Color.SkyBlue;
                nameMaskedTextBox.Text = CmdListView.SelectedItems[0].SubItems[1].Text.Substring(2);
                string name = CmdListView.SelectedItems[0].SubItems[1].Text;
                for (int i = 0; i < local_cmd.Count; i++)
                {
                    if (string.Compare(local_cmd[i].name, name) == 0)
                    {
                        current_trigger = local_cmd[i];
                        update_triggers();
                        break;
                    }
                }
            }
            if (last == null)
                last = CmdListView.SelectedItems[0];
            else if (CmdListView.SelectedItems.Count != 0)
            {
                last.BackColor = Color.White;
                last = CmdListView.SelectedItems[0];
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (checkName() == true)
            {
                cmd_num = cmd_num + 1;
                string[] row = { cmd_num.ToString(), "c_" + nameMaskedTextBox.Text };
                var varItem = new ListViewItem(row);
                CmdListView.Items.Add(varItem);
                warningLabel.Text = "";
                varItem.Selected = true;
                ME_Trigger tr = new ME_Trigger();
                tr.name = row[1];
                local_cmd.Add(tr);
                current_trigger = tr;
                update_triggers();
                enableTriggerGroup(Always_groupBox);
            }
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            if (CmdListView.SelectedItems.Count == 0)
                return;
            ListViewItem cur = CmdListView.SelectedItems[0];
            if (string.Compare(nameMaskedTextBox.Text, cur.SubItems[1].Text) == 0)
            {
                warningLabel.Text = "";
                return;
            }
            else if (checkName() == true)
            {
                warningLabel.Text = "";
                current_trigger.name = "c_" + nameMaskedTextBox.Text;
                cur.SubItems[1].Text = "c_" + nameMaskedTextBox.Text;
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            removeSelectedItem();
        }

        private void CmdListView_KeyDown(object sender, KeyEventArgs e)
        {
            removeSelectedItem();
        }

        private void removeSelectedItem()
        {
            if (CmdListView.SelectedItems.Count == 0)
                return;
            ListViewItem cur = CmdListView.SelectedItems[0];
            if (current_trigger != null)
            {
                if (current_trigger.used == true)
                {
                    warningLabel.Text = cmd_lang_dic["errorMsg27"];
                    return;
                }
            }
            int index = CmdListView.Items.IndexOf(cur);
            if (index != CmdListView.Items.Count - 1)
            {
                for (int i = index + 1; i < cmd_num; i++)
                {
                    int origin_num = int.Parse(CmdListView.Items[i].SubItems[0].Text);
                    CmdListView.Items[i].SubItems[0].Text = (origin_num - 1).ToString();
                    for (int j = 0; j < tmp_element.Count; j++)
                    {
                        int prev_num = global_cmd.Count;
                        if (tmp_element[j] is ME_Motion m)
                        {
                            if (m.trigger_index == i && m.trigger_index < prev_num)
                                m.trigger_index = m.trigger_index - 1;
                        }
                        else if (tmp_element[j] is ME_If mif)
                        {
                            int offset = var_num + 58;
                            if (mif.left_var - offset == i && mif.left_var < offset + prev_num)
                                mif.left_var = mif.left_var - 1;
                        }
                    }
                }
            }
            for (int i = 0; i < local_cmd.Count; i++)
            {
                if (string.Compare(local_cmd[i].name, cur.SubItems[1].Text) == 0)
                {
                    local_cmd.Remove(local_cmd[i]);
                    break;
                }
            }
            CmdListView.Items.Remove(cur);
            cmd_num = cmd_num - 1;
            nameMaskedTextBox.Text = "";
            warningLabel.Text = "";
        }

        private void ESP8266ModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.esp8266_mode = ESP8266ModeCombo.Text;
        }

        private void ESP8266BaudCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.esp8266_baud = ESP8266BaudCombo.Text;
        }

        private void ESP8266PortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.esp8266_port = ESP8266PortCombo.Text;
        }

        private void ESP8266KeyText_TextChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.esp8266_key = ESP8266KeyText.Text;
        }

        private void ESP8266_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ' || e.KeyChar == '\"' || e.KeyChar == '\'')
                e.Handled = true;
        }

        private void ESP8266_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (ESP8266_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.esp8266;
                    enableTriggerGroup(ESP8266_groupBox);
                }
            }
        }

        private void analog_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (analog_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.analog;
                    enableTriggerGroup(analog_groupBox);
                }
            }
        }

        private void analogValueText_Changed(object sender, EventArgs e)
        {
            int output;
            if (current_trigger != null)
            {
                if (int.TryParse(((MaskedTextBox)sender).Text, out output))
                {
                    if (output >= 1024)
                    {
                        output = 1023;
                        ((MaskedTextBox)sender).Text = "1023";
                        ((MaskedTextBox)sender).SelectionStart = ((MaskedTextBox)sender).Text.Length;
                    }
                    current_trigger.analog_value = output;
                }
                else
                    current_trigger.analog_value = 0;
            }
        }

        private void numbercheck(object sender, KeyPressEventArgs e)
        {
            if (((int)e.KeyChar < 48 | (int)e.KeyChar > 57) & (int)e.KeyChar != 8)
                e.Handled = true;
        }

        private void analogCondCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.analog_cond = analogCondCombo.SelectedIndex;
        }

        private void analogPinCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.analog_pin = analogPinCombo.SelectedIndex;
        }

        private void wifi602_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (wifi602_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.wifi602;
                    enableTriggerGroup(wifi602_groupBox);
                }
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
                MessageBox.Show(cmd_lang_dic["errorMsg20"]);
            }
        }

        private void accDurationText_Changed(object sender, EventArgs e)
        {
            int output;
            if (current_trigger != null)
            {
                if (int.TryParse(((MaskedTextBox)sender).Text, out output))
                    current_trigger.acc_Settings[6] = output;
                else
                    current_trigger.acc_Settings[6] = 0;
            }
        }

        private void accXYZText_Changed(object sender, EventArgs e)
        {
            double output;
            if (double.TryParse(((MaskedTextBox)sender).Text, out output))
                current_trigger.acc_Settings[int.Parse(((MaskedTextBox)sender).Name)] = output;
            else if (((MaskedTextBox)sender).Text == "-" || ((MaskedTextBox)sender).Text == "" ||
                     ((MaskedTextBox)sender).Text == "-." || ((MaskedTextBox)sender).Text == ".")
                current_trigger.acc_Settings[int.Parse(((MaskedTextBox)sender).Name)] = 0;
            else
            {
                MessageBox.Show(cmd_lang_dic["errorMsg19"]);
                ((MaskedTextBox)sender).SelectAll();
            }
        }

        private void floatcheck(object sender, KeyPressEventArgs e)
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

        private void wifi602PortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.wifi602_port = wifi602PortCombo.Text;
        }

        private void ESP8266CHPDCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.esp8266_chpd = ESP8266CHPDCombo.Text;
        }

        private void wifi602KeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.wifi602_key = wifi602KeyCombo.SelectedIndex;
        }

        private void acc_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (acc_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.acc;
                    enableTriggerGroup(acc_groupBox);
                }
            }
        }

        private void ps2CLKCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[3] = ps2CLKCombo.Text;
        }

        private void ps2ATTCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[2] = ps2ATTCombo.Text;
        }

        private void ps2CMDCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[1] = ps2CMDCombo.Text;
        }

        private void ps2DATCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.ps2pins[0] = ps2DATCombo.Text;
        }

        private void ps2TypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.ps2_type = ps2TypeCombo.SelectedIndex;
        }

        private void ps2KeyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.ps2_key = ps2KeyCombo.Text;
        }

        private void ps2_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (ps2_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.ps2;
                    enableTriggerGroup(ps2_groupBox);
                }
            }
        }

        private void btTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.bt_mode = btModeCombo.Text;
        }

        private void btBaudCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.bt_baud = btBaudCombo.Text;
        }

        private void btPortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            gs.bt_port = btPortCombo.Text;
        }

        private void btKeyText_TextChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.bt_key = btKeyText.Text;
        }

        private void bt_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (bt_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.bluetooth;
                    enableTriggerGroup(bt_groupBox);
                }
            }
        }

        private void Keyboard_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (Keyboard_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.keyboard;
                    enableTriggerGroup(Keyboard_groupBox);
                }
            }
        }

        private void Always_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
            {
                if (Always_radioButton.Checked == true)
                {
                    current_trigger.trigger_method = (int)mtest_method.always;
                    enableTriggerGroup(Always_groupBox);
                }
            }
        }

        private void KeyboardTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.keyboard_Type = KeyboardTypeCombo.SelectedIndex;
        }

        private void KeyboardCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                current_trigger.keyboard_key = KeyboardCombo.SelectedIndex;
        }

        private void TitleMotion_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                if (TitleMotion.Checked == true)
                    current_trigger.auto_method = (int)auto_method.title;
        }

        private void AlwaysOff_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                if (AlwaysOff.Checked == true)
                    current_trigger.auto_method = (int)auto_method.off;
        }

        private void AlwaysOn_CheckedChanged(object sender, EventArgs e)
        {
            if (current_trigger != null)
                if (AlwaysOn.Checked == true)
                    current_trigger.auto_method = (int)auto_method.on;
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

        private void clearGSpins()
        {
            ps2DATCombo.Items.Clear();
            ps2CMDCombo.Items.Clear();
            ps2ATTCombo.Items.Clear();
            ps2CLKCombo.Items.Clear();
            ESP8266CHPDCombo.Items.Clear();
        }

        private void createGSpins(int i)
        {
            ps2DATCombo.Items.Add(i.ToString());
            ps2CMDCombo.Items.Add(i.ToString());
            ps2ATTCombo.Items.Add(i.ToString());
            ps2CLKCombo.Items.Add(i.ToString());
            ESP8266CHPDCombo.Items.Add(i.ToString());
        }

        private void initGSpins()
        {
            if (board_ver86 == 0) //one
            {
                clearGSpins();
                for (int i = 0; i < 45; i++)
                    createGSpins(i);
            }
            else if (board_ver86 == 1) //zero
            {
                clearGSpins();
                for (int i = 0; i < 14; i++)
                    createGSpins(i);
                for (int i = 42; i < 45; i++)
                    createGSpins(i);
            }
            else if (board_ver86 == 2) //edu
            {
                clearGSpins();
                for (int i = 0; i < 21; i++)
                    createGSpins(i);
                for (int i = 31; i < 33; i++)
                    createGSpins(i);
                for (int i = 42; i < 45; i++)
                    createGSpins(i);
            }
            else if (board_ver86 == 3) //ai
            {
                clearGSpins();
                for (int i = 0; i < 36; i++)
                    createGSpins(i);
            }
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
            ESP8266_groupBox.Enabled = false;
            gb.Enabled = true;
        }

        private void update_triggers()
        {
            if (current_trigger != null)
                MotionConfig.Enabled = true;
            else
                return;

            ME_Trigger tr = current_trigger;
            if (tr.trigger_method == (int)mtest_method.always)
            {
                enableTriggerGroup(Always_groupBox);
                Always_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.keyboard)
            {
                enableTriggerGroup(Keyboard_groupBox);
                Keyboard_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.bluetooth)
            {
                enableTriggerGroup(bt_groupBox);
                bt_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.ps2)
            {
                enableTriggerGroup(ps2_groupBox);
                ps2_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.acc)
            {
                enableTriggerGroup(acc_groupBox);
                acc_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.wifi602)
            {
                enableTriggerGroup(wifi602_groupBox);
                wifi602_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.analog)
            {
                enableTriggerGroup(analog_groupBox);
                analog_radioButton.Checked = true;
            }
            else if (tr.trigger_method == (int)mtest_method.esp8266)
            {
                enableTriggerGroup(ESP8266_groupBox);
                ESP8266_radioButton.Checked = true;
            }

            if (tr.auto_method == (int)auto_method.on)
                AlwaysOn.Checked = true;
            else if (tr.auto_method == (int)auto_method.off)
                AlwaysOff.Checked = true;
            else if (tr.auto_method == (int)auto_method.title)
                TitleMotion.Checked = true;

            KeyboardCombo.SelectedIndex = tr.keyboard_key;
            KeyboardTypeCombo.SelectedIndex = tr.keyboard_Type;

            btKeyText.Text = tr.bt_key;
            btModeCombo.Text = tr.bt_mode;
            btPortCombo.Text = gs.bt_port;
            btBaudCombo.Text = gs.bt_baud;

            wifi602PortCombo.Text = gs.wifi602_port;
            wifi602KeyCombo.SelectedIndex = tr.wifi602_key;

            ps2DATCombo.Text = gs.ps2pins[0];
            ps2CMDCombo.Text = gs.ps2pins[1];
            ps2ATTCombo.Text = gs.ps2pins[2];
            ps2CLKCombo.Text = gs.ps2pins[3];
            ps2KeyCombo.Text = tr.ps2_key;
            ps2TypeCombo.SelectedIndex = tr.ps2_type;

            accLXText.Text = tr.acc_Settings[0].ToString();
            accHXText.Text = tr.acc_Settings[1].ToString();
            accLYText.Text = tr.acc_Settings[2].ToString();
            accHYText.Text = tr.acc_Settings[3].ToString();
            accLZText.Text = tr.acc_Settings[4].ToString();
            accHZText.Text = tr.acc_Settings[5].ToString();
            accDurationText.Text = tr.acc_Settings[6].ToString();

            analogCondCombo.SelectedIndex = tr.analog_cond;
            analogPinCombo.SelectedIndex = tr.analog_pin;
            analogValueText.Text = tr.analog_value.ToString();

            ESP8266KeyText.Text = tr.esp8266_key;
            ESP8266ModeCombo.Text = tr.esp8266_mode;
            ESP8266BaudCombo.Text = gs.esp8266_baud;
            ESP8266PortCombo.Text = gs.esp8266_port;
            ESP8266CHPDCombo.Text = gs.esp8266_chpd;
        }
    }
}
