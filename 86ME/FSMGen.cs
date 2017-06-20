using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.IO;

namespace _86ME_ver2
{
    enum mtest_method { always, keyboard, bluetooth, ps2, acc, wifi602, analog, esp8266 };
    enum keyboard_method { first, pressed, release };
    enum auto_method { on, off, title };
    enum serial_ports { serial1, serial2, serial3 };
    enum motion_property { blocking, nonblocking };
    enum internal_trigger { call, jump };

    public class GlobalSettings
    {
        public string[] ps2pins;
        public string bt_baud;
        public string bt_port;
        public string wifi602_port;
        public string esp8266_baud;
        public string esp8266_port;
        public string esp8266_chpd;

        public GlobalSettings()
        {
            this.wifi602_port = "Serial1";
            this.bt_port = "Serial1";
            this.bt_baud = "9600";
            this.ps2pins = new string[4] { "0", "0", "0", "0" };
            this.esp8266_port = "Serial1";
            this.esp8266_baud = "115200";
            this.esp8266_chpd = "0";
        }
    }

    class FSMGen
    {
        private string nfilename = "";
        private ArrayList ME_Motionlist;
        private List<int> keyboard_keys = new List<int>();
        private List<ME_Trigger> commands;
        private NewMotion Motion;
        private bool[] method_flag = new bool[16];
        private int[] offset = new int[45];
        private string[] ps2_pins = new string[4];
        private string bt_baud;
        private string bt_port;
        private string esp8266_baud;
        private string esp8266_port;
        private string esp8266_chpd;
        private string wifi602_port;
        private int opVar_num;
        private bool IMU_compensatory = false;
        private Quaternion invQ = new Quaternion();

        public FSMGen(NewMotion nMotion, int[] off, ArrayList motionlist, GlobalSettings gs, int opVar_num, List<ME_Trigger> trigger_cmd)
        {
            this.Motion = nMotion;
            this.opVar_num = opVar_num;
            this.offset = off;
            this.ME_Motionlist = motionlist;
            this.commands = trigger_cmd;
            gs.ps2pins.CopyTo(this.ps2_pins, 0);
            this.bt_baud = gs.bt_baud;
            this.bt_port = gs.bt_port;
            this.esp8266_baud = gs.esp8266_baud;
            this.esp8266_port = gs.esp8266_port;
            this.esp8266_chpd = gs.esp8266_chpd;
            this.wifi602_port = gs.wifi602_port;
            this.invQ.w = double.Parse(Motion.maskedTextBox1.Text);
            this.invQ.x = double.Parse(Motion.maskedTextBox2.Text);
            this.invQ.y = double.Parse(Motion.maskedTextBox3.Text);
            this.invQ.z = double.Parse(Motion.maskedTextBox4.Text);
            this.invQ = this.invQ.Normalized().Inverse();
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.trigger_index >= 0)
                {
                    ME_Trigger tr = commands[m.trigger_index];
                    int method = tr.trigger_method;
                    method_flag[method] = true;
                    if (method == (int)mtest_method.keyboard && !keyboard_keys.Contains(tr.keyboard_key))
                        keyboard_keys.Add(tr.keyboard_key);
                }
            }
            if (Motion.comboBox2.SelectedIndex != 0)
            {
                method_flag[4] = true;
                for (int i = 0; i < 45; i++)
                {
                    if (((Motion.fbox2[i].SelectedIndex != 0 && Motion.p_gain[i] != 0) ||
                        (Motion.fbox3[i].SelectedIndex != 0 && Motion.s_gain[i] != 0)) &&
                        String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        IMU_compensatory = true;
                }
            }
        }

        private bool isPureFrame(ME_Motion m)
        {
            for (int i = 0; i < m.Events.Count; i++)
                if (!(m.Events[i] is ME_Frame))
                    return false;
            return true;
        }

        private int convert_keynum(int keynum)
        {
            if (keynum <= 25)
                return keynum + 4;
            else if (keynum == 26)
                return 0x2C;
            else if (keynum == 27)
                return 0x50;
            else if (keynum == 28)
                return 0x4F;
            else if (keynum == 29)
                return 0x52;
            else if (keynum == 30)
                return 0x51;
            else if (keynum == 31)
                return 0x29;
            else
                return 0;
        }

        private string trigger_condition(ME_Trigger m)
        {
            switch (m.trigger_method)
            {
                case (int)mtest_method.always:
                    if (m.auto_method == (int)auto_method.on)
                        return "1";
                    else if (m.auto_method == (int)auto_method.off)
                        return "0";
                    else //title
                        return m.name.Substring(2) + "_title == 1";
                case (int)mtest_method.keyboard:
                    if (m.keyboard_Type == (int)keyboard_method.first)
                        return "keys_state[" + convert_keynum(m.keyboard_key) + "] == 2";
                    else if (m.keyboard_Type == (int)keyboard_method.pressed)
                        return "keys_state[" + convert_keynum(m.keyboard_key) + "] == 3";
                    else // release
                        return "keys_state[" + convert_keynum(m.keyboard_key) + "] == 1";
                case (int)mtest_method.bluetooth:
                    if (String.Compare("", m.bt_key) == 0)
                        return "0";
                    else
                        return bt_port + "_Command == \'" + m.bt_key + "\'";
                case (int)mtest_method.ps2:
                    if (m.ps2_type == (int)keyboard_method.first)
                        return "ps2x.ButtonPressed(" + m.ps2_key + ")";
                    else if (m.ps2_type == (int)keyboard_method.pressed)
                        return "ps2x.Button(" + m.ps2_key + ") && !ps2x.ButtonPressed(" + m.ps2_key + ")";
                    else
                        return "ps2x.ButtonReleased(" + m.ps2_key + ")";
                case (int)mtest_method.acc:
                    return m.name.Substring(2) + "::acc_state == 2";
                case (int)mtest_method.wifi602:
                    if (m.wifi602_key == 0)
                        return "abs(wifi602_data[2]) <= abs(wifi602_data[3]) && wifi602_data[3] > 0";
                    else if (m.wifi602_key == 1)
                        return "abs(wifi602_data[2]) <= abs(wifi602_data[3]) && wifi602_data[3] < 0";
                    else if (m.wifi602_key == 2)
                        return "abs(wifi602_data[2]) > abs(wifi602_data[3]) && wifi602_data[2] < 0";
                    else if (m.wifi602_key == 3)
                        return "abs(wifi602_data[2]) > abs(wifi602_data[3]) && wifi602_data[2] > 0";
                    else if (m.wifi602_key == 4) //A
                        return "wifi602_data[4] < 0";
                    else //B
                        return "wifi602_data[4] > 0";
                case (int)mtest_method.analog:
                    if (m.analog_cond == 0)
                        return "analogRead(A" + m.analog_pin + ") == " + m.analog_value;
                    else if (m.analog_cond == 1)
                        return "analogRead(A" + m.analog_pin + ") != " + m.analog_value;
                    else if (m.analog_cond == 2)
                        return "analogRead(A" + m.analog_pin + ") >= " + m.analog_value;
                    else if (m.analog_cond == 3)
                        return "analogRead(A" + m.analog_pin + ") <= " + m.analog_value;
                    else if (m.analog_cond == 4)
                        return "analogRead(A" + m.analog_pin + ") > " + m.analog_value;
                    else
                        return "analogRead(A" + m.analog_pin + ") < " + m.analog_value;
                case (int)mtest_method.esp8266:
                    return "strncmp(wifi_cmd, \"" + m.esp8266_key + "\", " + m.esp8266_key.Length + ") == 0";
                default:
                    return "0";
            }
        }

        private string op2str(int left, int val1, int val2, int n, int form)
        {
            if (left < opVar_num)
            {
                switch (form)
                {
                    case 0:
                        if (n == 0) return "    " + var2str(left) + " = " + var2str(val1) + "+" + var2str(val2) + ";";
                        else if (n == 1) return "    " + var2str(left) + " = " + var2str(val1) + "-" + var2str(val2) + ";";
                        else if (n == 2) return "    " + var2str(left) + " = " + var2str(val1) + "*" + var2str(val2) + ";";
                        else if (n == 3) return "    if(" + var2str(val2) + " != 0) " + var2str(left) + " = " + var2str(val1) +
                                                "/" + var2str(val2) + ";\n    else " + var2str(left) + " = 0;";
                        else if (n == 4) return "    " + var2str(left) + " = pow(" + var2str(val1) + ", " + var2str(val2) + ");";
                        else if (n == 5) return "    if(" + var2str(val2) + " != 0) " + var2str(left) + " = fmod(" + var2str(val1) +
                                                ", " + var2str(val2) + ");\n    else " + var2str(left) + " = 0;";
                        else if (n == 6) return "    " + var2str(left) + " = (int)" + var2str(val1) + " & (int)" + var2str(val2) + ";";
                        else if (n == 7) return "    " + var2str(left) + " = (int)" + var2str(val1) + " | (int)" + var2str(val2) + ";";
                        break;
                    case 1:
                        if (n == 0) return "    " + var2str(left) + " = ~((int)" + var2str(val2) + ");";
                        else if (n == 1) return "    " + var2str(left) + " = sqrt(" + var2str(val2) + ");";
                        else if (n == 2) return "    " + var2str(left) + " = exp(" + var2str(val2) + ");";
                        else if (n == 3) return "    if(" + var2str(val2) + " != 0) " + var2str(left) + " = log(" + var2str(val2) + ");\n" +
                                                "    else " + var2str(val2) + " = 0;";
                        else if (n == 4) return "    if(" + var2str(val2) + " != 0) " + var2str(left) + " = log10(" + var2str(val2) + ");\n" +
                                                "    else " + var2str(val2) + " = 0;";
                        else if (n == 5) return "    " + var2str(left) + " = fabs(" + var2str(val2) + ");";
                        else if (n == 6) return "    " + var2str(left) + " = -" + var2str(val2) + ";";
                        else if (n == 7) return "    " + var2str(left) + " = cos(" + var2str(val2) + ");";
                        else if (n == 8) return "    " + var2str(left) + " = sin(" + var2str(val2) + ");";
                        break;
                }
            }
            else
            {
                int gpio_pin = left - opVar_num;
                switch (form)
                {
                    case 0:
                        if (n == 0) return "    digitalWrite(" + gpio_pin + ", (int)" + var2str(val1) + "+ (int)" + var2str(val2) + ");";
                        else if (n == 1) return "    digitalWrite(" + gpio_pin + ", (int)" + var2str(val1) + "- (int)" + var2str(val2) + ");";
                        else if (n == 2) return "    digitalWrite(" + gpio_pin + ", (int)" + var2str(val1) + "* (int)" + var2str(val2) + ");";
                        else if (n == 3) return "    if(" + var2str(val2) + " != 0) digitalWrite(" + gpio_pin + ", (int)(" + var2str(val1) +
                                                "/" + var2str(val2) + "));\n    else digitalWrite(" + gpio_pin + ", 0);";
                        else if (n == 4) return "    digitalWrite(" + gpio_pin + ", (int)pow(" + var2str(val1) + ", " + var2str(val2) + "));";
                        else if (n == 5) return "    if(" + var2str(val2) + " != 0) digitalWrite(" + gpio_pin + ", (int)fmod(" + var2str(val1) +
                                                ", " + var2str(val2) + "));\n    else digitalWrite(" + gpio_pin + ", 0);";
                        else if (n == 6) return "    digitalWrite(" + gpio_pin + ", (int)" + var2str(val1) + " & (int)" + var2str(val2) + ");";
                        else if (n == 7) return "    digitalWrite(" + gpio_pin + ", (int)" + var2str(val1) + " | (int)" + var2str(val2) + ");";
                        break;
                    case 1:
                        if (n == 0) return "    digitalWrite(" + gpio_pin + ", ~((int)" + var2str(val2) + "));";
                        else if (n == 1) return "    digitalWrite(" + gpio_pin + ", (int)sqrt(" + var2str(val2) + "));";
                        else if (n == 2) return "    digitalWrite(" + gpio_pin + ", (int)exp(" + var2str(val2) + "));";
                        else if (n == 3) return "    if(" + var2str(val2) + " != 0) digitalWrite(" + gpio_pin + ", (int)log(" + var2str(val2) +
                                                "));\n    else digitalWrite(" + gpio_pin + ", 0);";
                        else if (n == 4) return "    if(" + var2str(val2) + " != 0) digitalWrite(" + gpio_pin + ", (int)log10(" + var2str(val2) +
                                                "));\n    else digitalWrite(" + var2str(val2) + ", 0);";
                        else if (n == 5) return "    digitalWrite(" + gpio_pin + ", (int)fabs(" + var2str(val2) + "));";
                        else if (n == 6) return "    digitalWrite(" + gpio_pin + ", (int)(-" + var2str(val2) + "));";
                        else if (n == 7) return "    digitalWrite(" + gpio_pin + ", (int)cos(" + var2str(val2) + "));";
                        else if (n == 8) return "    digitalWrite(" + gpio_pin + ", (int)sin(" + var2str(val2) + "));";
                        break;
                }
            }
            return "";
        }

        private string method2str(int n)
        {
            switch (n)
            {
                case 0:
                    return "==";
                case 1:
                    return "!=";
                case 2:
                    return ">=";
                case 3:
                    return "<=";
                case 4:
                    return ">";
                case 5:
                    return "<";
                default:
                    return "==";
            }
        }

        private string var2str(int n)
        {
            if (n == opVar_num)
                return "millis()";
            else if (n == opVar_num + 1)
                return "rand()";
            else if (n < opVar_num + 8 && n >= opVar_num + 2)
                return "analogRead(" + (n - opVar_num - 2) + ")";
            else if (n < opVar_num + 11 && n >= opVar_num + 8)
                return "_IMU_val[" + (n - opVar_num - 8) + "]";
            else if (n == opVar_num + 11)
                return "_roll";
            else if (n == opVar_num + 12)
                return "_pitch";
            else if (n < opVar_num + 58 && n >= opVar_num + 13)
                return "digitalRead(" + (n - opVar_num - 13) + ")";
            else if (n < opVar_num + commands.Count + 58 && n >= opVar_num + 58)
                return "_86ME_cmd[" + (n - opVar_num - 58) + "]";
            else
                return "_86ME_var[" + n + "]";
        }

        private void generate_namespace(ME_Motion m, TextWriter writer, List<int> channels, string space = "")
        {
            writer.Write(space + "namespace " + m.name + "\n" +
                         space + "{\n" +
                         space + "  enum {IDLE");
            if (m.Events.Count > 0)
                writer.Write(", ");
            m.goto_var.Clear();
            m.states.Clear();
            m.states.Add("IDLE");
            for (int i = 0; i < m.Events.Count; i++)
            {
                if (m.Events[i] is ME_Frame)
                {
                    writer.Write("FRAME_" + i.ToString() + ", " + "WAIT_FRAME_" + i.ToString());
                    m.states.Add("FRAME_" + i.ToString());
                    m.states.Add("WAIT_FRAME_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Delay)
                {
                    writer.Write("DELAY_" + i.ToString() + ", " + "WAIT_DELAY_" + i.ToString());
                    m.states.Add("DELAY_" + i.ToString());
                    m.states.Add("WAIT_DELAY_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Flag)
                {
                    writer.Write("FLAG_" + i.ToString());
                    m.states.Add("FLAG_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Goto)
                {
                    writer.Write("GOTO_" + i.ToString());
                    m.states.Add("GOTO_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                    for (int k = 0; k < m.Events.Count; k++)
                    {
                        if (m.Events[k] is ME_Flag)
                        {
                            if (String.Compare(((ME_Goto)m.Events[i]).name, ((ME_Flag)m.Events[k]).name) == 0)
                            {
                                ME_Goto g = (ME_Goto)m.Events[i];
                                ME_Flag f = (ME_Flag)m.Events[k];
                                if (g.is_goto && g.infinite == false)
                                {
                                    f.var = g.name + "_" + i;
                                    m.goto_var.Add(g.name + "_" + i);
                                    break; // match once
                                }
                            }
                        }
                    }
                }
                else if (m.Events[i] is ME_GotoMotion)
                {
                    writer.Write("MOTION_" + i.ToString() + ", " + "WAIT_MOTION_" + i.ToString());
                    m.states.Add("MOTION_" + i.ToString());
                    m.states.Add("WAIT_MOTION_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Release)
                {
                    writer.Write("RELEASE_" + i.ToString());
                    m.states.Add("RELEASE_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_If)
                {
                    writer.Write("IF_" + i.ToString());
                    m.states.Add("IF_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Compute)
                {
                    writer.Write("COMPUTE_" + i.ToString());
                    m.states.Add("COMPUTE_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
            }
            writer.WriteLine("};");
            writer.WriteLine(space + "  int state = IDLE;");
            writer.WriteLine(space + "  unsigned long time;");
            for (int i = 0; i < m.goto_var.Count; i++)
                writer.WriteLine(space + "  int " + m.goto_var[i] + " = 0;");
            writer.WriteLine(space + "  double comp_range = " + m.comp_range + ";");
            if (m.moton_layer == 1)
            {
                writer.Write(space + "  int mask[" + channels.Count + "] = { ");
                for (int j = 0; j < channels.Count; j++)
                {
                    if (m.used_servos.Contains(channels[j]))
                        writer.Write("0xffff");
                    else
                        writer.Write("0");
                    if (j != channels.Count - 1)
                        writer.Write(", ");
                }
                writer.WriteLine("};");
            }
            if (m.trigger_index >= 0)
            {
                ME_Trigger tr = commands[m.trigger_index];
                if (tr.trigger_method == (int)mtest_method.acc)
                {
                    writer.WriteLine(space + "  int acc_state = 0;");
                    writer.WriteLine(space + "  unsigned long acc_time;");
                }
            }
            writer.WriteLine(space + "}"); // namespace
        }

        private void generate_updateIMU(TextWriter writer, List<int> channels, string class_name = "", string space = "")
        {
            if (method_flag[4])
            {
                if (string.Compare(class_name, "") == 0)
                    writer.WriteLine(space + "void updateIMU()\n{");
                else
                    writer.WriteLine(space + "void " + class_name + "::updateIMU()\n{");
                writer.WriteLine(space + "  if(millis() - _IMU_update_time < 20 || _IMU_init_status != 0) return;\n"); // ~50fps
                writer.WriteLine(space + "  _IMU.getQ(_IMU_Q, _IMU_val);");
                writer.WriteLine(space + "  if(_comp_range == 0) {DisableMixing(); goto EXIT_COMP;} else {EnableMixing();}");
                writer.WriteLine(space + "  double _w, _x, _y, _z;");
                writer.WriteLine(space + "  _w = _IMU_Q[0]*" + invQ.w + " - _IMU_Q[1]*" + invQ.x +
                                 " - _IMU_Q[2]*" + invQ.y + " - _IMU_Q[3]*" + invQ.z + ";");
                writer.WriteLine(space + "  _x = _IMU_Q[1]*" + invQ.w + " + _IMU_Q[0]*" + invQ.x +
                                 " - _IMU_Q[3]*" + invQ.y + " + _IMU_Q[2]*" + invQ.z + ";");
                writer.WriteLine(space + "  _y = _IMU_Q[2]*" + invQ.w + " + _IMU_Q[3]*" + invQ.x +
                                 " + _IMU_Q[0]*" + invQ.y + " - _IMU_Q[1]*" + invQ.z + ";");
                writer.WriteLine(space + "  _z = _IMU_Q[3]*" + invQ.w + " - _IMU_Q[2]*" + invQ.x +
                                 " + _IMU_Q[1]*" + invQ.y + " + _IMU_Q[0]*" + invQ.z + ";");
                writer.WriteLine(space + "  _roll = atan2(2*(_w*_x + _y*_z), 1 - 2*(_x*_x + _y*_y));");
                writer.WriteLine(space + "  _pitch = asin(2*(_w*_y - _z*_x));");
                writer.WriteLine(space + "  _omega[0] = 0.1*_omega[0] + 0.9*_IMU_val[3];");
                writer.WriteLine(space + "  _omega[1] = 0.1*_omega[1] + 0.9*_IMU_val[4];");
                writer.WriteLine(space + "  memset(_mixOffsets, 0, sizeof(long)*45);");
                if (IMU_compensatory)
                {
                    for (int ps = 0; ps < 2; ps++)
                    {
                        for (int source = 1; source < 5; source++)
                        {
                            if (source == 1) // roll
                                writer.WriteLine(space + "  if(fabs(_roll*180/M_PI) <= _comp_range)\n  {");
                            else if (source == 2) // pitch
                                writer.WriteLine(space + "  if(fabs(_pitch*180/M_PI) <= _comp_range)\n  {");
                            for (int i = 0; i < channels.Count; i++)
                            {
                                if (ps == 0)
                                {
                                    if (Motion.p_gain[channels[i]] != 0 && Motion.fbox2[channels[i]].SelectedIndex != 0)
                                    {
                                        if (Motion.fbox2[channels[i]].SelectedIndex == source)
                                        {
                                            if (source == 1) // roll
                                                writer.WriteLine(space + "    _mixOffsets[" + i + "] += " +
                                                                 "(long)((180*_roll*" + Motion.p_gain[channels[i]] + ")/M_PI);");
                                            else if (source == 2) // pitch
                                                writer.WriteLine(space + "    _mixOffsets[" + i + "] += " +
                                                                 "(long)((180*_pitch*" + Motion.p_gain[channels[i]] + ")/M_PI);");
                                            else if (source == 3)
                                                writer.WriteLine(space + "  _mixOffsets[" + i + "] += " +
                                                                 "(long)(_omega[0]*" + Motion.p_gain[channels[i]] + ");");
                                            else if (source == 4)
                                                writer.WriteLine(space + "  _mixOffsets[" + i + "] += " +
                                                                 "(long)(_omega[1]*" + Motion.p_gain[channels[i]] + ");");
                                        }
                                    }
                                }
                                else if (ps == 1)
                                {
                                    if (Motion.s_gain[channels[i]] != 0 && Motion.fbox3[channels[i]].SelectedIndex != 0)
                                    {
                                        if (Motion.fbox3[channels[i]].SelectedIndex == source)
                                        {
                                            if (source == 1) // roll
                                                writer.WriteLine(space + "    _mixOffsets[" + i + "] += " +
                                                                 "(long)((180*_roll*" + Motion.s_gain[channels[i]] + ")/M_PI);");
                                            else if (source == 2) // pitch
                                                writer.WriteLine(space + "    _mixOffsets[" + i + "] += " +
                                                                 "(long)((180*_pitch*" + Motion.s_gain[channels[i]] + ")/M_PI);");
                                            else if (source == 3)
                                                writer.WriteLine(space + "  _mixOffsets[" + i + "] += " +
                                                                 "(long)(_omega[0]*" + Motion.s_gain[channels[i]] + ");");
                                            else if (source == 4)
                                                writer.WriteLine(space + "  _mixOffsets[" + i + "] += " +
                                                                 "(long)(_omega[1]*" + Motion.s_gain[channels[i]] + ");");
                                        }
                                    }
                                }
                            }
                            if (source == 1 || source == 2)
                                writer.WriteLine(space + "  }");
                        }
                    }
                    writer.WriteLine(space + "  servoMultiRealTimeMixing(_mixOffsets);");
                }
                writer.WriteLine(space + "EXIT_COMP:");

                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.acc)
                    {
                        string stmts = "";
                        if (tr.acc_Settings[0] < tr.acc_Settings[1])
                            stmts += space + "(_IMU_val[0] >= " + tr.acc_Settings[0] + " && _IMU_val[0] <= " + tr.acc_Settings[1] + ") &&\n";
                        else
                            stmts += space + "(_IMU_val[0] >= " + tr.acc_Settings[1] + " && _IMU_val[0] <= " + tr.acc_Settings[0] + ") &&\n";
                        if (tr.acc_Settings[2] < tr.acc_Settings[3])
                            stmts += space + "         (_IMU_val[1] >= " + tr.acc_Settings[2] + " && _IMU_val[1] <= " + tr.acc_Settings[3] + ") &&\n";
                        else
                            stmts += space + "         (_IMU_val[1] >= " + tr.acc_Settings[3] + " && _IMU_val[1] <= " + tr.acc_Settings[2] + ") &&\n";
                        if (tr.acc_Settings[4] < tr.acc_Settings[5])
                            stmts += space + "         (_IMU_val[2] >= " + tr.acc_Settings[4] + " && _IMU_val[2] <= " + tr.acc_Settings[5] + ")";
                        else
                            stmts += space + "         (_IMU_val[2] >= " + tr.acc_Settings[5] + " && _IMU_val[2] <= " + tr.acc_Settings[4] + ")";

                        writer.WriteLine(space + "  switch(" + m.name + "::acc_state)\n" +
                                         space + "  {\n" +
                                         space + "    case 0:\n" +
                                         space + "      if(" + stmts + ")\n" +
                                         space + "      {\n" +
                                         space + "        " + m.name + "::acc_state = 1;\n" +
                                         space + "        " + m.name + "::acc_time = millis();\n" +
                                         space + "      }\n" +
                                         space + "      break;\n" +
                                         space + "    case 1:\n" +
                                         space + "      if(" + stmts + ")\n" +
                                         space + "      {\n" +
                                         space + "        if(millis() - " + m.name + "::acc_time >= " + tr.acc_Settings[6] + ")\n" +
                                         space + "          " + m.name + "::acc_state = 2;\n" +
                                         space + "      }\n" +
                                         space + "      else\n" +
                                         space + "        " + m.name + "::acc_state = 0;\n" +
                                         space + "      break;\n" +
                                         space + "    case 3:\n" +
                                         space + "      if(_curr_motion[" + m.moton_layer + "] != _" + m.name.ToUpper() + ")\n" +
                                         space + "        " + m.name + "::acc_state = 0;\n" +
                                         space + "      break;\n" +
                                         space + "    default:\n" +
                                         space + "      break;\n" +
                                         space + "  }");
                    }
                }
                writer.WriteLine(space + "  _IMU_update_time = millis();");
                writer.WriteLine(space + "}");
            }
        }

        private void generate_isBlocked(TextWriter writer, string class_name = "", string space = "")
        {
            string l0 = "", l1 = "";
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine(space + "bool isBlocked(int layer)\n" + space + "{");
            else
                writer.WriteLine(space + "bool " + class_name + "::isBlocked(int layer)\n" + space + "{");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.moton_layer == 0 && m.property == (int)motion_property.blocking)
                    l0 += "    if(external_trigger[_" + m.name.ToUpper() + "]) return true;\n";
                else if (m.moton_layer == 1 && m.property == (int)motion_property.blocking)
                    l1 += "    if(external_trigger[_" + m.name.ToUpper() + "]) return true;\n";
            }
            if (l0 != "")
                writer.WriteLine(space + "  if(layer == 0)\n" +
                                 space + "  {\n" +
                                 space + l0 +
                                 space + "  }");
            if (l1 != "" && l0 != "")
                writer.WriteLine(space + "  else if(layer == 1)\n" +
                                 space + "  {\n" +
                                 space + l1 +
                                 space + "  }");
            else if (l1 != "" && l0 == "")
                writer.WriteLine(space + "  if(layer == 1)\n" +
                                 space + "  {\n" +
                                 space + l1 +
                                 space + "  }");
            writer.WriteLine(space + "  return false;");
            writer.WriteLine(space + "}");
        }

        private void generate_closeTriggers(TextWriter writer, string class_name = "", string space = "")
        {
            string l0 = "", l1 = "";
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine(space + "void closeTriggers(int layer)\n" + space + "{");
            else
                writer.WriteLine(space + "void " + class_name + "::closeTriggers(int layer)\n" + space + "{");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.moton_layer == 0)
                    l0 += "    external_trigger[_" + m.name.ToUpper() + "]= false; " +
                          "internal_trigger[_" + m.name.ToUpper() + "]= false;\n";
                else if (m.moton_layer == 1)
                    l1 += "    external_trigger[_" + m.name.ToUpper() + "]= false; " +
                          "internal_trigger[_" + m.name.ToUpper() + "]= false;\n";
            }
            if (l0 != "")
                writer.WriteLine(space + "  if(layer == 0)\n" +
                                 space + "  {\n" +
                                 space + l0 +
                                 space + "  }");
            if (l1 != "" && l0 != "")
                writer.WriteLine(space + "  else if(layer == 1)\n" +
                                 space + "  {\n" +
                                 space + l1 +
                                 space + "  }");
            else if (l1 != "" && l0 == "")
                writer.WriteLine(space + "  if(layer == 1)\n" +
                                 space + "  {\n" +
                                 space + l1 +
                                 space + "  }");
            writer.WriteLine(space + "}");
        }

        private void generate_updateCompRange(TextWriter writer, string class_name = "", string space = "")
        {
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine(space + "void updateCompRange()\n" + space + "{");
            else
                writer.WriteLine(space + "void " + class_name + "::updateCompRange()\n" + space + "{");
            writer.WriteLine(space + "  _comp_range = -1;");
            bool first = true;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (first)
                {
                    writer.WriteLine(space + "  if((external_trigger[_" + m.name.ToUpper() + "] || internal_trigger[_" +
                                     m.name.ToUpper() + "]) && " + m.name + "::comp_range >= _comp_range)\n" +
                                     space + "    _comp_range = " + m.name + "::comp_range;");
                    first = false;
                }
                else
                {
                    writer.WriteLine(space + "  else if((external_trigger[_" + m.name.ToUpper() + "] || internal_trigger[_" +
                                     m.name.ToUpper() + "]) && " + m.name + "::comp_range >= _comp_range)\n" +
                                     space + "    _comp_range = " + m.name + "::comp_range;");
                }
            }
            writer.WriteLine(space + "  else\n" +
                             space + "    _comp_range = 180;\n" +
                             space + "}");
        }

        private void generate_updateCommand(TextWriter writer, string class_name = "", string space = "")
        {
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine(space + "void updateCommand()\n" + space + "{");
            else
                writer.WriteLine(space + "void " + class_name + "::updateCommand()\n" + space + "{");
            // get input
            if (method_flag[1])
            {
                writer.WriteLine(space + "  usb.Task();");
                for (int i = 0; i < keyboard_keys.Count; i++)
                    writer.WriteLine(space + "  update_keys_state(" + convert_keynum(keyboard_keys[i]) + ");");
            }
            if (method_flag[2])
            {
                writer.WriteLine(space + "  if(" + bt_port + ".available()){ " + bt_port + "_Command = " + bt_port + ".read(); }");
                writer.WriteLine(space + "  else { if(renew_bt) " + bt_port + "_Command = 0xFFF; }");
            }
            if (method_flag[3])
            {
                writer.WriteLine(space + "  ps2x.read_gamepad();");
            }
            if (method_flag[5])
            {
                writer.WriteLine(space + "  read_wifi602pad(" + wifi602_port + ");");
            }
            if (method_flag[7])
            {
                writer.WriteLine(space + "  uint8_t robot_name[13] = \"86DuinoROBOT\";");
                writer.WriteLine(space + "  wifi.send_nb(2, robot_name, 12);");
                writer.WriteLine(space + "  uint32_t wifi_recvlen = wifi.recv_nb(&wifi_mux_id, wifi_buffer, sizeof(wifi_buffer));");
                writer.WriteLine(space + "  if (wifi_recvlen > 0){ strncpy(wifi_cmd, (char*)wifi_buffer, wifi_recvlen);" +
                                 " wifi_cmd[wifi_recvlen] = \'\\0\';}");
                writer.WriteLine(space + "  else { if(renew_esp8266) wifi_cmd[0] = \'\\0\'; }");
            }
            // update commands
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.trigger_index >= 0)
                {
                    writer.WriteLine(space + "  if(" + trigger_condition(commands[m.trigger_index]) + ") " + "{_86ME_cmd[" + m.trigger_index + "] = true;}");
                    writer.WriteLine(space + "  else {_86ME_cmd[" + m.trigger_index + "] = false;}");
                }
            }
            writer.WriteLine(space + "}");
        }

        private void generate_updateTrigger(TextWriter writer, string class_name = "", string space = "")
        {
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine(space + "void updateTrigger()\n" + space + "{");
            else
                writer.WriteLine(space + "void " + class_name + "::updateTrigger()\n" + space + "{");
            writer.WriteLine(space + "  if(isBlocked(0)) goto L1;");
            for (int layer = 0; layer < 2; layer++)
            {
                writer.WriteLine("L" + layer + ":");
                if (layer == 1)
                    writer.WriteLine(space + "  if(isBlocked(1)) return;");
                // update tirggers
                bool first = true;
                for (int i = 0; i < ME_Motionlist.Count; i++) //startup
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.always && tr.auto_method == (int)auto_method.title && m.moton_layer == layer)
                    {
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";
                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + "; " +
                                             m.name + "_title--;" + update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + "; " +
                                             m.name + "_title--;" + update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++) //acc
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.acc && m.moton_layer == layer)
                    {
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";

                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                                "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + "; " +
                                                m.name + "::acc_state = 3; " + update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                                "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + "; " +
                                                m.name + "::acc_state = 3; " + update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++) //analog
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.analog && m.moton_layer == layer)
                    {
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";

                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++) //esp8266
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.esp8266 && m.moton_layer == layer)
                    {
                        string renew_esp8266 = "";
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";

                        if (String.Compare(tr.esp8266_mode, "OneShot") == 0)
                            renew_esp8266 = "renew_esp8266 = true;";
                        else
                            renew_esp8266 = "renew_esp8266 = false;";
                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             renew_esp8266 + update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             renew_esp8266 + update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++) //bt
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.bluetooth && m.moton_layer == layer)
                    {
                        string renew_bt = "";
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";

                        if (String.Compare(tr.bt_mode, "OneShot") == 0)
                            renew_bt = "renew_bt = true;";
                        else
                            renew_bt = "renew_bt = false;";
                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             renew_bt + update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             renew_bt + update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++) //wifi602
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.wifi602 && m.moton_layer == layer)
                    {
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";

                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" +
                                             update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (!(tr.trigger_method == (int)mtest_method.always && tr.auto_method == (int)auto_method.on) &&
                        !(tr.trigger_method == (int)mtest_method.always && tr.auto_method == (int)auto_method.title) &&
                        !(tr.trigger_method == (int)mtest_method.bluetooth) && !(tr.trigger_method == (int)mtest_method.acc) &&
                        !(tr.trigger_method == (int)mtest_method.wifi602) && !(tr.trigger_method == (int)mtest_method.analog) &&
                        !(tr.trigger_method == (int)mtest_method.esp8266) && m.moton_layer == layer)
                    {
                        string cancel_release = "";
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";

                        if (tr.trigger_method == (int)mtest_method.keyboard && tr.keyboard_Type == (int)keyboard_method.release)
                            cancel_release = "keys_state[" + convert_keynum(tr.keyboard_key) + "] = 0;";
                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + "; " +
                                             cancel_release + update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + "; " +
                                             cancel_release + update_mask + "}");
                    }
                }
                for (int i = 0; i < ME_Motionlist.Count; i++) //always
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_index < 0)
                        continue;
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.always &&
                        tr.auto_method == (int)auto_method.on && m.moton_layer == layer)
                    {
                        string update_mask = "";
                        if (layer == 1)
                            update_mask = "memcpy(servo_mask, " + m.name + "::mask, sizeof(servo_mask));";
                        if (first)
                        {
                            writer.WriteLine(space + "  if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" + update_mask + "}");
                            first = false;
                        }
                        else
                            writer.WriteLine(space + "  else if(_86ME_cmd[" + m.trigger_index + "]) " +
                                             "{_curr_motion[" + layer + "] = _" + m.name.ToUpper() + ";" + update_mask + "}");
                    }
                }
                if (first)
                    writer.WriteLine(space + "  _curr_motion[" + layer + "] = _NONE;");
                else
                {
                    if (layer == 0)
                        writer.WriteLine(space + "  else _curr_motion[" + layer + "] = _NONE;");
                    else
                        writer.WriteLine(space + "  else {_curr_motion[" + layer + "] = _NONE;" +
                                         "  memset(servo_mask, 0, sizeof(servo_mask));}");
                }
                writer.WriteLine(space + "  if(_last_motion[" + layer + "] != _curr_motion[" + layer +
                                 "] && _curr_motion[" + layer + "] != _NONE)");
                writer.WriteLine(space + "  {\n" +
                                 space + "    closeTriggers(" + layer + ");\n" +
                                 space + "    external_trigger[_curr_motion[" + layer + "]] = true;");
                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.moton_layer == layer)
                    {
                        writer.WriteLine(space + "    " + m.name + "::state = 0;");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                    }
                }
                writer.WriteLine(space + "  }");
                writer.WriteLine(space + "  external_trigger[_curr_motion[" + layer + "]] = true;");
                writer.WriteLine(space + "  _last_motion[" + layer + "] = _curr_motion[" + layer + "];");
            }
            writer.WriteLine(space + "}");
        }

        private void generate_forvar(ME_Motion m)
        {
            for (int i = 0, flag_count = 0; i < m.Events.Count; i++)
            {
                if (m.Events[i] is ME_Flag)
                {
                    for (int k = 0; k < m.Events.Count; k++)
                    {
                        if (m.Events[k] is ME_Goto)
                        {
                            if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_Goto)m.Events[k]).name) == 0)
                            {
                                ME_Goto g = (ME_Goto)m.Events[k];
                                if (g.is_goto)
                                {
                                    string for_var = m.name + "_" + g.name + "_" + flag_count.ToString();
                                    ((ME_Flag)m.Events[i]).var = for_var;
                                    flag_count++;
                                    break;
                                }
                            }
                        }
                        else if (m.Events[k] is ME_If)
                        {
                            if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_If)m.Events[k]).name) == 0)
                            {
                                ME_If mif = (ME_If)m.Events[k];
                                string for_var = m.name + "_" + mif.name + "_" + flag_count.ToString();
                                ((ME_Flag)m.Events[i]).var = for_var;
                                flag_count++;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void generate_motion(ME_Motion m, string frm_name, TextWriter writer, int channels, string class_name = "", string space = "")
        {
            bool is_pure = isPureFrame(m);
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine("void " + m.name + "Update()\n" + space + "{");
            else
                writer.WriteLine("void " + class_name + "::" + m.name + "Update()\n" + space + "{");
            writer.WriteLine(space + "  switch(" + m.name + "::state)\n" +
                             space + "  {");
            writer.WriteLine(space + "  case " + m.name + "::IDLE:");
            if (m.states.Count > 1)
                writer.WriteLine(space + "    if(external_trigger[_" + m.name.ToUpper() + "] || internal_trigger[_" +
                                 m.name.ToUpper() + "]) " + m.name + "::state = " + m.name + "::" + m.states[1] + ";");
            else
                writer.WriteLine(space + "    if(external_trigger[_" + m.name.ToUpper() + "] || internal_trigger[_" +
                                 m.name.ToUpper() + "]) " + m.name + "::state = " + m.name + "::" + m.states[0] + ";");
            writer.WriteLine(space + "    else break;");
            string next_action = "";
            int state_counter = 1;
            generate_forvar(m);
            for (int i = 0; i < m.Events.Count; i++)
            {
                if (m.Events[i] is ME_Frame)
                {
                    ME_Frame f = (ME_Frame)m.Events[i];
                    string mask_str = "(~servo_mask[i])";
                    if (m.moton_layer == 1)
                        mask_str = "servo_mask[i]";
                    writer.WriteLine(space + "  case " + m.name + "::FRAME_" + i + ":");
                    writer.WriteLine(space + "    for(int i = " + channels + "; i-- > 0; )");
                    if (m.control_method != 0 && is_pure)
                        writer.WriteLine(space + "    {\n" +
                                         space + "      _86ME_RUN.positions[i] = " + frm_name + "[" + f.num + "].positions[i] & " + mask_str + ";\n" +
                                         space + "      _86ME_RUN.accelerations[i][0] = " + frm_name + "[" + f.num + "].accelerations[i][0];\n" +
                                         space + "      _86ME_RUN.accelerations[i][1] = " + frm_name + "[" + f.num + "].accelerations[i][1];\n" +
                                         space + "    }");
                    else
                        writer.WriteLine(space + "      _86ME_RUN.positions[i] = " + frm_name +
                                         "[" + f.num + "].positions[i] & " + mask_str + ";");
                    writer.WriteLine(space + "    _86ME_RUN.playPositions((unsigned long)" + f.delay + ");");
                    writer.WriteLine(space + "    " + m.name + "::time = millis();");
                    writer.WriteLine(space + "    " + m.name + "::state = " + m.name + "::WAIT_FRAME_" + i + ";");
                    writer.WriteLine(space + "  case " + m.name + "::WAIT_FRAME_" + i + ":");
                    writer.WriteLine(space + "    if(millis() - " + m.name + "::time >= " + f.delay + ")");
                    if (i != m.Events.Count - 1)
                    {
                        state_counter += 2;
                        next_action = m.name + "::" + m.states[state_counter];
                        writer.WriteLine(space + "      " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space + "    {\n" +
                                         space + "      " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "      " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space + "      internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "      external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    }");
                    }
                    writer.WriteLine(space + "    break;");
                }
                else if (m.Events[i] is ME_Delay)
                {
                    ME_Delay d = (ME_Delay)m.Events[i];
                    writer.WriteLine(space + "  case " + m.name + "::DELAY_" + i + ":");
                    writer.WriteLine(space + "    " + m.name + "::time = millis();");
                    writer.WriteLine(space + "    " + m.name + "::state = " + m.name + "::WAIT_DELAY_" + i + ";");
                    writer.WriteLine(space + "  case " + m.name + "::WAIT_DELAY_" + i + ":");
                    writer.WriteLine(space + "    if(millis() - " + m.name + "::time >= " + d.delay + ")");
                    if (i != m.Events.Count - 1)
                    {
                        state_counter += 2;
                        next_action = m.name + "::" + m.states[state_counter];
                        writer.WriteLine(space + "      " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space + "    {\n      " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "      " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space + "      internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "      external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    }");
                    }
                    writer.WriteLine(space + "    break;");
                }
                else if (m.Events[i] is ME_Flag)
                {
                    writer.WriteLine(space + "  case " + m.name + "::FLAG_" + i + ":");
                    state_counter++;
                    for (int k = 0; k < m.Events.Count; k++)
                    {
                        if (m.Events[k] is ME_Goto)
                        {
                            if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_Goto)m.Events[k]).name) == 0)
                            {
                                ME_Goto g = (ME_Goto)m.Events[k];
                                if (g.is_goto)
                                {
                                    writer.WriteLine(space + "    flag_" + ((ME_Flag)m.Events[i]).var + ":");
                                    break;
                                }
                            }
                        }
                        else if (m.Events[k] is ME_If)
                        {
                            if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_If)m.Events[k]).name) == 0)
                            {
                                ME_If mif = (ME_If)m.Events[k];
                                writer.WriteLine(space + "    flag_" + ((ME_Flag)m.Events[i]).var + ":");
                                break;
                            }
                        }
                    }
                    if (i == m.Events.Count - 1)
                    {
                        writer.WriteLine(space + "    " + m.name + "::state = " + m.name + "::IDLE;");
                        writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space + "    break;");
                    }
                }
                else if (m.Events[i] is ME_Goto)
                {
                    ME_Goto g = (ME_Goto)m.Events[i];
                    writer.WriteLine(space + "  case " + m.name + "::GOTO_" + i + ":");
                    state_counter++;
                    if (g.is_goto)
                    {
                        bool has_flag = false;
                        for (int k = 0; k < m.Events.Count; k++)
                        {
                            if (m.Events[k] is ME_Flag)
                            {
                                if (String.Compare(g.name, ((ME_Flag)m.Events[k]).name) == 0)
                                {
                                    has_flag = true;
                                    string for_var = ((ME_Flag)m.Events[k]).var;
                                    if (((ME_Goto)m.Events[i]).infinite == false)
                                    {
                                        writer.WriteLine(space + "    if(" + m.name + "::" + g.name + "_" + i +
                                                         "++ < " + g.loops + ") goto flag_" + for_var + ";");
                                        break;
                                    }
                                    else
                                    {
                                        writer.WriteLine(space + "    if(1) goto flag_" + for_var + ";");
                                        break;
                                    }
                                }
                            }
                        }
                        if (i != m.Events.Count - 1 && has_flag)
                        {
                            next_action = m.name + "::" + m.states[state_counter];
                            if (g.infinite == false)
                                writer.WriteLine(space + "    " + m.name + "::" + g.name + "_" + i + " = 0;");
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        }
                        else if (i != m.Events.Count - 1 && !has_flag)
                        {
                            next_action = m.name + "::" + m.states[state_counter];
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        }
                        else if (i == m.Events.Count - 1 && has_flag)
                        {
                            writer.WriteLine(space + "    else\n    {");
                            next_action = m.name + "::IDLE";
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space + "      " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space + "      internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "      external_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "      " + m.name + "::state = " + next_action + ";");
                            writer.WriteLine(space + "    }");
                        }
                        else
                        {
                            next_action = m.name + "::IDLE";
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        }
                        writer.WriteLine(space + "    break;");
                    }
                }
                else if (m.Events[i] is ME_GotoMotion)
                {
                    ME_GotoMotion t = (ME_GotoMotion)m.Events[i];
                    writer.WriteLine(space + "  case " + m.name + "::MOTION_" + i + ":");
                    if (m.moton_layer == 1)
                    {
                        if (i != m.Events.Count - 1)
                        {
                            state_counter += 2;
                            next_action = m.name + "::" + m.states[state_counter];
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        }
                        else
                        {
                            next_action = m.name + "::IDLE";
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "    break;");
                        }
                        continue;
                    }
                    if (String.Compare(t.name, "") != 0)
                    {
                        for (int j = 0; j < ME_Motionlist.Count; j++) //reset goto_var of the triggered motion
                        {
                            ME_Motion tr_m = (ME_Motion)ME_Motionlist[j];
                            if (tr_m.name == t.name)
                            {
                                for (int k = 0; k < tr_m.goto_var.Count; k++)
                                    writer.WriteLine(space + "    " + tr_m.name + "::" + tr_m.goto_var[k] + " = 0;");
                            }
                        }
                    }
                    if (String.Compare(t.name, "") == 0)
                    {
                        if (i != m.Events.Count - 1)
                        {
                            state_counter += 2;
                            next_action = m.name + "::" + m.states[state_counter];
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        }
                        else
                        {
                            next_action = m.name + "::IDLE";
                            writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                        }
                    }
                    else if (t.method == (int)internal_trigger.call)
                    {
                        writer.WriteLine(space + "    " + m.name + "::state = " + m.name + "::WAIT_MOTION_" + i + ";");
                        writer.WriteLine(space + "    " + "internal_trigger[_" + t.name.ToUpper() + "] = true;");
                        writer.WriteLine(space + "    " + t.name + "::state = " + t.name + "::IDLE;");
                        writer.WriteLine(space + "  case " + m.name + "::WAIT_MOTION_" + i + ":");
                        writer.WriteLine(space + "    if(!internal_trigger[_" + t.name.ToUpper() + "])");
                        if (i != m.Events.Count - 1)
                        {
                            state_counter += 2;
                            next_action = m.name + "::" + m.states[state_counter];
                            writer.WriteLine(space + "      " + m.name + "::state = " + next_action + ";");
                        }
                        else
                        {
                            next_action = m.name + "::IDLE";
                            writer.WriteLine(space + "    {\n      " + m.name + "::state = " + next_action + ";");
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space + "      " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space + "      internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "      external_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space + "    }");
                        }
                    }
                    else if (t.method == (int)internal_trigger.jump)
                    {
                        writer.WriteLine(space + "    " + m.name + "::state = " + m.name + "::IDLE;");
                        writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    internal_trigger[_" + t.name.ToUpper() + "] = true;");
                        writer.WriteLine(space + "    " + t.name + "::state = " + t.name + "::IDLE;");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                    }
                    writer.WriteLine(space + "    break;");
                }
                else if (m.Events[i] is ME_Release)
                {
                    writer.WriteLine(space + "  case " + m.name + "::RELEASE_" + i + ":");
                    writer.WriteLine(space + "    for(int i = " + channels + "; i-- > 0; )");
                    writer.WriteLine(space + "      used_servos[i].release();");
                    state_counter++;
                    if (i != m.Events.Count - 1)
                    {
                        next_action = m.name + "::" + m.states[state_counter];
                        writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                    }
                    writer.WriteLine(space + "    break;");
                }
                else if (m.Events[i] is ME_If)
                {
                    ME_If mif = (ME_If)m.Events[i];
                    writer.WriteLine(space + "  case " + m.name + "::IF_" + i + ":");
                    if (mif.left_var >= opVar_num + 13 && mif.left_var < opVar_num + 58)
                        writer.WriteLine(space + "    pinMode(" + (mif.left_var - opVar_num - 13) + ", INPUT);");
                    if (mif.form == 0)
                    {
                        if (mif.right_var >= opVar_num + 13 && mif.right_var < opVar_num + 58)
                            writer.WriteLine(space + "    pinMode(" + (mif.right_var - opVar_num - 13) + ", INPUT);");
                        writer.WriteLine(space + "    if(" + var2str(mif.left_var) + method2str(mif.method) + var2str(mif.right_var) + ")");
                    }
                    else if (mif.form == 1)
                        writer.WriteLine(space + "    if(" + var2str(mif.left_var) + method2str(mif.method) + mif.right_const + ")");
                    bool hasTarget = false;
                    for (int k = 0; k < m.Events.Count; k++)
                    {
                        if (m.Events[k] is ME_Flag)
                        {
                            if (String.Compare(mif.name, ((ME_Flag)m.Events[k]).name) == 0)
                            {
                                writer.WriteLine(space + "      goto flag_" + ((ME_Flag)m.Events[k]).var + ";");
                                hasTarget = true;
                                break;
                            }
                        }
                    }
                    if (!hasTarget)
                        writer.WriteLine(space + "      ;");
                    state_counter++;
                    if (i != m.Events.Count - 1)
                    {
                        next_action = m.name + "::" + m.states[state_counter];
                        writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                    }
                    writer.WriteLine(space + "    break;");
                }
                else if (m.Events[i] is ME_Compute)
                {
                    ME_Compute op = (ME_Compute)m.Events[i];
                    writer.WriteLine(space + "  case " + m.name + "::COMPUTE_" + i + ":");
                    switch (op.form)
                    {
                        case 0:
                            if (op.f1_var1 >= opVar_num + 13 && op.f1_var1 < opVar_num + 58)
                            {
                                int gpio_pin = op.f1_var1 - opVar_num - 13;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", INPUT);");
                            }
                            if (op.f1_var2 >= opVar_num + 13 && op.f1_var2 < opVar_num + 58)
                            {
                                int gpio_pin = op.f1_var2 - opVar_num - 13;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", INPUT);");
                            }
                            if (op.left_var >= opVar_num && op.left_var < opVar_num + 45)
                            {
                                int gpio_pin = op.left_var - opVar_num;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", OUTPUT);");
                            }
                            writer.WriteLine(op2str(op.left_var, op.f1_var1, op.f1_var2, op.f1_op, 0));
                            break;
                        case 1:
                            if (op.f1_var1 >= opVar_num + 13 && op.f1_var1 < opVar_num + 58)
                            {
                                int gpio_pin = op.f1_var1 - opVar_num - 13;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", INPUT);");
                            }
                            if (op.left_var >= opVar_num && op.left_var < opVar_num + 45)
                            {
                                int gpio_pin = op.left_var - opVar_num;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", OUTPUT);");
                            }
                            writer.WriteLine(op2str(op.left_var, 0, op.f2_var, op.f2_op, 1));
                            break;
                        case 2:
                            if (op.f3_var >= opVar_num + 13 && op.f3_var < opVar_num + 58)
                            {
                                int gpio_pin = op.f3_var - opVar_num - 13;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", INPUT);");
                            }
                            if (op.left_var >= opVar_num && op.left_var < opVar_num + 45)
                            {
                                int gpio_pin = op.left_var - opVar_num;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", OUTPUT);");
                                writer.WriteLine(space + "    digitalWrite(" + gpio_pin + ", (int)" + var2str(op.f3_var) + ");");
                            }
                            else
                                writer.WriteLine(space + "    " + var2str(op.left_var) + " = " + var2str(op.f3_var) + ";");
                            break;
                        case 3:
                            if (op.left_var >= opVar_num && op.left_var < opVar_num + 45)
                            {
                                int gpio_pin = op.left_var - opVar_num;
                                writer.WriteLine(space + "    pinMode(" + gpio_pin + ", OUTPUT);");
                                writer.WriteLine(space + "    digitalWrite(" + gpio_pin + ", " + (int)(op.f4_const) + ");");
                            }
                            else
                                writer.WriteLine(space + "    " + var2str(op.left_var) + " = " + op.f4_const + ";");
                            break;
                        default:
                            break;
                    }
                    state_counter++;
                    if (i != m.Events.Count - 1)
                    {
                        next_action = m.name + "::" + m.states[state_counter];
                        writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space + "    " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space + "    " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space + "    internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space + "    break;");
                    }
                }
            }
            writer.WriteLine(space + "  default:");
            writer.WriteLine(space + "    break;");
            writer.WriteLine(space + "  }"); //switch
            writer.WriteLine(space + "}");
        }

        private void generate_reset(TextWriter writer, List<int> channels, string class_name)
        {
            writer.WriteLine("void " + class_name + "::reset(bool initIMU)\n{");
            for (int i = 0; i < channels.Count; i++)
                writer.WriteLine("  used_servos[" + i.ToString() + "].detach();");
            writer.WriteLine("  begin(initIMU);");
            writer.WriteLine("}");
        }

        private void generate_include_headers(TextWriter writer, bool include_arduino)
        {
            writer.WriteLine("#include <time.h>");
            writer.WriteLine("#include <math.h>");
            if (include_arduino)
                writer.WriteLine("#include <Arduino.h>");
            writer.WriteLine("#include <Servo86.h>");
            if (method_flag[1]) // keyboard
                writer.WriteLine("#include <KeyboardController.h>");
            if (method_flag[3]) // ps2
                writer.WriteLine("#include <PS2X_lib.h>");
            if (method_flag[4]) // acc
                writer.WriteLine("#include <EEPROM.h>\n#include <FreeIMU1.h>\n#include <Wire.h>");
            if (method_flag[7]) // esp8266
                writer.WriteLine("#include <ESP8266.h>");
            writer.WriteLine();
        }

        private void generate_setup(TextWriter writer, List<int> channels, List<uint> home, bool isAllinOne, List<int> angle = null, string class_name = "")
        {
            string frm_name;
            int processed = 0;
            if (string.Compare(class_name, "") == 0)
            {
                writer.WriteLine("void setup()\n{");
            }
            else
            {
                generate_reset(writer, channels, class_name);
                writer.WriteLine("void " + class_name + "::begin(bool initIMU)\n{");
            }

            writer.WriteLine("  srand(time(NULL));");

            if (method_flag[2]) // bt
                writer.WriteLine("  " + bt_port + ".begin(" + bt_baud + ");");

            if (method_flag[3]) // ps2
                writer.WriteLine("  ps2x.config_gamepad(" + ps2_pins[3] + ", " + ps2_pins[1] +
                                 ", " + ps2_pins[2] + ", " + ps2_pins[0] + ", false, false);\n");
            if (method_flag[4]) // acc
            {
                string sp = "";
                if (string.Compare(class_name, "") != 0)
                {
                    writer.WriteLine("  if(initIMU)\n  {");
                    sp = "  ";
                }
                if (Motion.comboBox2.SelectedIndex == 1) //LSM330DLC of One
                    writer.WriteLine(sp + "  Wire.begin();\n" + sp + "  delay(5);\n" + sp +
                                     "  _IMU_init_status = _IMU.initEX(0, true);\n" + sp + "  delay(5);");
                else if (Motion.comboBox2.SelectedIndex == 2) //RM-G146
                    writer.WriteLine(sp + "  Wire.begin();\n" + sp + "  delay(5);\n" + sp +
                                     "  _IMU_init_status = _IMU.initEX(2, true);\n" + sp + "  delay(5);");
                else if (Motion.comboBox2.SelectedIndex == 3) //LSM330DLC of AI
                    writer.WriteLine(sp + "  Wire.begin();\n" + sp + "  delay(5);\n" + sp +
                                     "  _IMU_init_status = _IMU.initEX(3, true);\n" + sp + "  delay(5);");
                if (string.Compare(class_name, "") != 0)
                    writer.WriteLine("  }");
            }

            if (method_flag[5]) // wifi602
                writer.WriteLine("  " + wifi602_port + ".begin(115200);");

            if (method_flag[7]) // esp8266
            {
                writer.WriteLine("  wifi.init(" + esp8266_port + ", " + esp8266_baud + ", " + esp8266_chpd + ");");
                writer.WriteLine("  wifi.setOprToSoftAP();");
                writer.WriteLine("  wifi.enableMUX();");
                writer.WriteLine("  wifi.startTCPServer(23);");
                writer.WriteLine("  wifi.registerUDP(2, \"255.255.255.255\", 6000);");
            }

            for (int i = 0; i < channels.Count; i++)
            {
                int min = int.Parse(Motion.ftext3[channels[i]].Text);
                int max = int.Parse(Motion.ftext4[channels[i]].Text);
                writer.WriteLine("  used_servos[" + i.ToString() + "].attach(" + channels[i].ToString() + ", " + min + ", " + max + ");");
            }
            writer.WriteLine();

            if (isAllinOne)
            {
                int offset_count = 0;
                for (int i = 0; i < 45; i++)
                {
                    if (offset[i] != 0 && String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        writer.WriteLine("  offsets.offsets[" + offset_count.ToString() + "] = " + offset[i].ToString() + ";");
                    if (String.Compare(Motion.fbox[i].Text, "---noServo---") != 0)
                        offset_count++;
                }
                writer.WriteLine();

                for (int k = 0; k < ME_Motionlist.Count; k++)
                {
                    frm_name = ((ME_Motion)ME_Motionlist[k]).name.ToString() + "_frm";
                    for (int i = 0; i < ((ME_Motion)ME_Motionlist[k]).frames; i++)
                    {
                        for (int j = 0; j < channels.Count; j++)
                            writer.WriteLine("  " + frm_name + "[" + i + "].positions[" + j.ToString() + "] = " +
                                             angle[processed + i * channels.Count + j] + ";");
                        writer.WriteLine();
                    }
                    processed += channels.Count * ((ME_Motion)ME_Motionlist[k]).frames;
                }
            }
            else
            {
                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    int frame_count = ((ME_Motion)ME_Motionlist[i]).frames;
                    string current_motion_name = ((ME_Motion)ME_Motionlist[i]).name;
                    frm_name = ((ME_Motion)ME_Motionlist[i]).name + "_frm";
                    for (int j = 0; j < frame_count; j++)
                    {
                        writer.WriteLine("  " + frm_name + "[" + j + "].load(\"" + "_86ME_settings\\\\" +
                                         current_motion_name + "_frm" + j + ".txt\");");
                    }
                }
            }

            for (int j = 0; j < channels.Count; j++)
                writer.WriteLine("  _86ME_HOME.positions[" + j.ToString() + "] = " + home[j] + ";");
            writer.WriteLine();
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.control_method != 0 && isPureFrame(m))
                {
                    if (m.control_method == 1)
                        writer.WriteLine("  servoBeginSplineMotion(CONSTRAINED_CUBIC, " +
                                         m.name + "_frm, _" + m.name + "_frm_time, " + m.frames + ");");
                    else if (m.control_method == 2)
                        writer.WriteLine("  servoBeginSplineMotion(NATURAL_CUBIC, " +
                                         m.name + "_frm, _" + m.name + "_frm_time, " + m.frames + ");");
                    else if (m.control_method == 3)
                        writer.WriteLine("  servoBeginSplineMotion(CATMULL_ROM, " +
                                         m.name + "_frm, _" + m.name + "_frm_time, " + m.frames + ");");
                }
            }
            writer.WriteLine();
            writer.WriteLine("  offsets.setOffsets();");
            writer.WriteLine();
            if (method_flag[4])
            {
                writer.WriteLine("  if(_IMU_init_status == 0)\n  {\n    for(int i = 0; i < 5; i++)\n" +
                                 "    {\n      _IMU.getQ(_IMU_Q, _IMU_val);\n" +
                                 "      delay(20);\n    }\n  }\n");
            }
            writer.WriteLine("  _86ME_HOME.playPositions((unsigned long)0);");

            writer.WriteLine("}");
            writer.WriteLine();
        }

        private void generate_loop(TextWriter writer, string class_name = "")
        {
            if (string.Compare(class_name, "") == 0)
                writer.WriteLine("void loop()");
            else
                writer.WriteLine("void " + class_name + "::update()");
            writer.WriteLine("{");
            if (method_flag[4]) //IMU
            {
                writer.WriteLine("  updateCompRange();");
                writer.WriteLine("  updateIMU();");
            }
            writer.WriteLine("  updateCommand();");
            writer.WriteLine("  updateTrigger();");
            for (int j = 0; j < ME_Motionlist.Count; j++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[j];
                writer.WriteLine("  " + m.name + "Update();");
            }
            writer.WriteLine("}");
            // generate simple update()
            if (string.Compare(class_name, "") != 0)
            {
                writer.WriteLine("void " + class_name + "::update_S()");
                writer.WriteLine("{");
                writer.WriteLine("  updateTrigger();");
                for (int j = 0; j < ME_Motionlist.Count; j++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[j];
                    writer.WriteLine("  " + m.name + "Update();");
                }
                writer.WriteLine("}");
            }
        }

        private void get_positions(List<int> channels, List<int> angle, List<uint> home)
        {
            int count = 0;
            bool add_channel = true;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                count = 0;
                for (int j = 0; j < ((ME_Motion)ME_Motionlist[i]).Events.Count; j++)
                {
                    if (((ME_Motion)ME_Motionlist[i]).Events[j] is ME_Frame)
                    {
                        ME_Frame f = (ME_Frame)(((ME_Motion)ME_Motionlist[i]).Events[j]);
                        for (int k = 0; k < 45; k++)
                        {
                            if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                            {
                                if (f.type == 1)
                                    angle.Add(f.frame[k]);
                                else if (f.type == 0)
                                    angle.Add(int.Parse(Motion.ftext2[k].Text));
                                home.Add(uint.Parse(Motion.ftext2[k].Text));
                                if (add_channel)
                                    channels.Add(k);
                            }
                        }
                        add_channel = false;
                        ((ME_Frame)((ME_Motion)ME_Motionlist[i]).Events[j]).num = count;
                        count++;
                    }
                }
                    ((ME_Motion)ME_Motionlist[i]).frames = count;
            }
        }

        private void generate_global_var(TextWriter writer, string space = "")
        {
            if (method_flag[1]) // keyboard
            {
                writer.WriteLine(space + "void keyPressed(void);");
                writer.WriteLine(space + "void keyReleased(void);");
                writer.WriteLine(space + "void update_keys_state(int k);");
            }
            if (method_flag[5]) // wifi602
            {
                writer.WriteLine(space + "bool check_wifi602_data(int* data);");
                writer.WriteLine(space + "void read_wifi602pad(HardwareSerial &uart);");
            }
        }

        private void generate_class(TextWriter writer, string class_name, List<int> channels)
        {
            writer.WriteLine("class " + class_name + " {");
            writer.WriteLine("private:");
            string space = "  ";
            if (opVar_num > 0)
                writer.WriteLine(space + "double _86ME_var[" + opVar_num + "];");
            writer.WriteLine(space + "bool _86ME_cmd[" + commands.Count + "];");
            writer.WriteLine(space + "double _roll;");
            writer.WriteLine(space + "double _pitch;");
            writer.WriteLine(space + "double _comp_range;");
            writer.WriteLine(space + "double _IMU_val[12];");
            writer.WriteLine(space + "double _IMU_Q[4];");
            writer.WriteLine(space + "double _omega[2];");
            writer.WriteLine(space + "int _IMU_init_status;");
            if (channels.Count > 0)
            {
                writer.WriteLine(space + "int servo_mask[" + channels.Count + "];");
                writer.WriteLine(space + "Servo used_servos[" + channels.Count + "];");
            }
            else
            {
                writer.WriteLine(space + "int servo_mask[1];");
                writer.WriteLine(space + "Servo used_servos[1];");
            }

            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.control_method != 0)
                {
                    if (m.frames > 0)
                        writer.WriteLine(space + "unsigned long _" + m.name + "_frm_time[" + m.frames + "];");
                    else
                        writer.WriteLine(space + "unsigned long _" + m.name + "_frm_time[1];\n");
                }
            }
            writer.WriteLine();
            writer.Write(space + "enum {");
            for (int i = 0; i < ME_Motionlist.Count; i++)
                writer.Write("_" + ((ME_Motion)ME_Motionlist[i]).name.ToUpper() + ", ");
            writer.Write("_NONE");
            writer.WriteLine("};");
            writer.WriteLine(space + "int _last_motion[2];");
            writer.WriteLine(space + "int _curr_motion[2];");
            int triggers_num = ME_Motionlist.Count + 1;
            writer.WriteLine(space + "bool internal_trigger[" + triggers_num + "];");
            writer.WriteLine(space + "bool external_trigger[" + triggers_num + "];");
            writer.WriteLine();
            writer.WriteLine(space + "ServoOffset offsets;");
            writer.WriteLine();
            writer.WriteLine(space + "ServoFrame _86ME_HOME;");// automatic homeframe
            writer.WriteLine(space + "ServoFrame _86ME_RUN;\n");

            string frm_name;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                frm_name = m.name.ToString() + "_frm";
                if (m.trigger_index >= 0)
                {
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.always && tr.auto_method == (int)auto_method.title)
                        writer.WriteLine(space + "int " + ((ME_Motion)ME_Motionlist[i]).name + "_title;");
                }
                writer.WriteLine(space + "ServoFrame " + frm_name + "[" + ((ME_Motion)ME_Motionlist[i]).frames + "];");
            }
            writer.WriteLine();
            
            writer.WriteLine(space + "void updateIMU(void);");
            writer.WriteLine(space + "bool isBlocked(int layer);");
            writer.WriteLine(space + "void closeTriggers(int layer);");
            writer.WriteLine(space + "bool isNoMotion(void);");
            writer.WriteLine(space + "void updateCompRange(void);");
            writer.WriteLine(space + "void updateCommand(void);");
            writer.WriteLine(space + "void updateTrigger(void);");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine(space + "void " + m.name + "Update(void);");
            }
            writer.WriteLine(space + "void update_S(void);");

            writer.WriteLine("public:");
            writer.WriteLine(space + class_name + "();");
            writer.WriteLine(space + "void begin(bool initIMU);");
            writer.WriteLine(space + "void reset(bool initIMU);");
            writer.WriteLine(space + "void update(void);");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine(space + "void " + m.name + "(int times = 1);");
            }
            writer.WriteLine("};");
        }

        private void generate_constructor(TextWriter writer, string class_name, List<int> channels, List<uint> home, string space = "")
        {
            writer.WriteLine(space + class_name + "::" + class_name + "()");
            writer.Write(space + ": ");
            if (opVar_num > 0)
                writer.Write("_86ME_var(), ");
            writer.WriteLine("_86ME_cmd(), _roll(0), _pitch(0), _comp_range(180), _IMU_val(),");
            writer.WriteLine("  _IMU_Q{ 1, 0, 0, 0}, _omega(), _IMU_init_status(-1), servo_mask(),");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.control_method != 0)
                {
                    if (m.frames > 0)
                    {
                        writer.Write("  _" + m.name + "_frm_time{ ");
                        int k = 0;
                        for (int j = 0; j < m.Events.Count; j++)
                        {
                            if (m.Events[j] is ME_Frame)
                            {
                                writer.Write(((ME_Frame)(m.Events[j])).delay);
                                if (k != m.frames - 1)
                                    writer.Write(", ");
                                else
                                    writer.Write("},\n");
                                k++;
                            }
                        }
                    }
                    else
                        writer.Write("  _" + m.name + "_frm_time(),\n");
                }
            }
            string frm_name;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                frm_name = m.name.ToString() + "_frm";
                if (m.trigger_index >= 0)
                {
                    ME_Trigger tr = commands[m.trigger_index];
                    if (tr.trigger_method == (int)mtest_method.always && tr.auto_method == (int)auto_method.title)
                        writer.WriteLine("  " + ((ME_Motion)ME_Motionlist[i]).name + "_title(1),");
                }
            }
            writer.WriteLine("  _last_motion{ _NONE, _NONE}, _curr_motion{ _NONE, _NONE},");
            writer.WriteLine("  internal_trigger(), external_trigger()");
            writer.WriteLine("{ }");
            writer.WriteLine();
        }

        private void generate_isNoMotion(TextWriter writer, string class_name)
        {
            string stmt = "";
            writer.WriteLine("bool " + class_name + "::isNoMotion()\n{");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                stmt += "external_trigger[_" + m.name.ToUpper() + "] || internal_trigger[_" + m.name.ToUpper() + "] ";
                if (i != ME_Motionlist.Count - 1)
                    stmt += "||\n     ";
                else
                    stmt += ")\n";
            }
            writer.WriteLine("  if(" + stmt + "    return false;");
            writer.WriteLine("  return true;\n}");
        }

        private void generate_simple_motion(TextWriter writer, ME_Motion m, string class_name)
        {
            writer.WriteLine("void " + class_name + "::" + m.name + "(int times)\n" +
                             "{\n" +
                             "  _86ME_cmd[" + m.trigger_index + "] = true;\n" +
                             "  for(int i = 0; i < times; i++)\n" +
                             "  {\n" +
                             "    do\n" +
                             "    {\n" +
                             "      update_S();\n" +
                             "    } while (!isNoMotion());\n" +
                             "  }\n" +
                             "  _86ME_cmd[" + m.trigger_index + "] = false;\n" +
                             "}");
        }

        private void generate_functions(TextWriter writer, string class_name, List<int> channels)
        {
            if (method_flag[1]) // keyboard
            {
                writer.WriteLine("USBHost usb;");
                writer.WriteLine("KeyboardController keyboard(usb);");
                writer.WriteLine("int keys_state[128] = {0};");
                writer.WriteLine("int keys_pressed[128] = {0};");
                writer.WriteLine("void keyPressed() { keys_pressed[keyboard.getOemKey()] = 1; }");
                writer.WriteLine("void keyReleased() { keys_pressed[keyboard.getOemKey()] = 0; }");
                writer.WriteLine("void update_keys_state(int k)\n" +
                                 "{\n" +
                                 "  if(keys_pressed[k] == 1 && (keys_state[k]&2) != 2)\n" +
                                 "      keys_state[k] = 2;\n" +
                                 "  else if(keys_pressed[k] == 1 && (keys_state[k]&2) == 2)\n" +
                                 "    keys_state[k] = 3;\n" +
                                 "  else if(keys_pressed[k] == 0 && (keys_state[k]&2) == 2)\n" +
                                 "    keys_state[k] = 1;\n" +
                                 "  else\n" +
                                 "    keys_state[k] = 0;\n" +
                                 "}");
            }
            if (method_flag[2]) // bt
            {
                writer.WriteLine("int " + bt_port + "_Command = 0xFFF;");
                writer.WriteLine("bool renew_bt = true;");
            }
            if (method_flag[3]) // ps2
                writer.WriteLine("PS2X ps2x;");
            if (method_flag[4]) // acc
            {
                writer.WriteLine("unsigned long _IMU_update_time = millis();");
                writer.WriteLine("FreeIMU1 _IMU = FreeIMU1();");
                writer.WriteLine("long _mixOffsets[45] = {0};");
            }
            if (method_flag[5]) // wifi602
            {
                writer.WriteLine("bool wifi602_frame_start = false;");
                writer.WriteLine("int wifi602_data[11] = {0};");
                writer.WriteLine("bool check_wifi602_data(int* data)\n" +
                                 "{\n" +
                                 "  if(data == NULL)\n" +
                                 "  {\n" +
                                 "    wifi602_frame_start = false;\n" +
                                 "    return false;\n" +
                                 "  }\n" +
                                 "  int checksum = data[1];\n" +
                                 "  for(int i = 2; i < 10; i++)\n" +
                                 "    checksum ^= data[i];\n" +
                                 "  if(checksum != data[0] || data[10] != 0x82)\n" +
                                 "  {\n" +
                                 "    wifi602_frame_start = false;\n" +
                                 "    return false;\n" +
                                 "  }\n" +
                                 "  return true;\n" +
                                 "}");
                writer.WriteLine("void " + "read_wifi602pad(HardwareSerial &uart)\n" +
                                 "{\n" +
                                 "  int tmp[11];\n" +
                                 "  if(wifi602_frame_start == false)\n" +
                                 "  {\n" +
                                 "    if(uart.available())\n" +
                                 "      if(uart.read() == 0x81)\n" +
                                 "        wifi602_frame_start = true;\n" +
                                 "  }\n" +
                                 "  else if(wifi602_frame_start == true)\n" +
                                 "  {\n" +
                                 "    if(uart.available() >= 11)\n" +
                                 "    {\n" +
                                 "      for(int i = 0; i < 11; i++)\n" +
                                 "        tmp[i] = uart.read();\n" +
                                 "      if(check_wifi602_data(tmp))\n" +
                                 "      {\n" +
                                 "        tmp[2] > 100? wifi602_data[2] = tmp[2] - 256 : wifi602_data[2] = tmp[2];\n" +
                                 "        tmp[3] > 100? wifi602_data[3] = tmp[3] - 256 : wifi602_data[3] = tmp[3];\n" +
                                 "        tmp[4] > 100? wifi602_data[4] = tmp[4] - 256 : wifi602_data[4] = tmp[4];\n" +
                                 "      }\n" +
                                 "      wifi602_frame_start = false;\n" +
                                 "    }\n" +
                                 "  }\n" +
                                 "}");
            }
            if (method_flag[7]) // esp8266
            {
                writer.WriteLine("char wifi_cmd[128] = {0};");
                writer.WriteLine("bool renew_esp8266 = true;");
                writer.WriteLine("ESP8266 wifi;");
                writer.WriteLine("uint8_t wifi_buffer[128] = {0};");
                writer.WriteLine("uint8_t wifi_mux_id;");
            }
            generate_updateIMU(writer, channels, class_name);
            generate_isBlocked(writer, class_name);
            generate_closeTriggers(writer, class_name);
            generate_isNoMotion(writer, class_name);
            generate_updateCompRange(writer, class_name);
            generate_updateCommand(writer, class_name);
            generate_updateTrigger(writer, class_name);
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                string frm_name = ((ME_Motion)ME_Motionlist[i]).name.ToString() + "_frm";
                generate_motion(m, frm_name, writer, channels.Count, class_name);
                generate_simple_motion(writer, m, class_name);
            }
        }

        private void generate_lib_properties(TextWriter writer, string class_name)
        {
            writer.WriteLine("name=" + class_name);
            writer.WriteLine("version=1.0");
            writer.WriteLine("author=DMP");
            writer.WriteLine("maintainer=DMP <info@dmp.com.tw>");
            writer.WriteLine("sentence=This library is generated from 86ME.");
            writer.WriteLine("paragraph=");
            writer.WriteLine("category=Device Control");
            writer.WriteLine("url=");
            writer.WriteLine("architectures=x86");
            writer.WriteLine("types=86Duino");
        }

        private void generate_keywords(TextWriter writer, string class_name)
        {
            writer.WriteLine("#######################################");
            writer.WriteLine("# Syntax Coloring Map Servo");
            writer.WriteLine("#######################################");
            writer.WriteLine();
            writer.WriteLine("#######################################");
            writer.WriteLine("# Datatypes (KEYWORD1)");
            writer.WriteLine("#######################################");
            writer.WriteLine();
            writer.WriteLine(class_name + "\tKEYWORD1");
            writer.WriteLine();
            writer.WriteLine("#######################################");
            writer.WriteLine("# Methods and Functions (KEYWORD2)");
            writer.WriteLine("#######################################");
            writer.WriteLine();
            writer.WriteLine("begin\tKEYWORD2");
            writer.WriteLine("update\tKEYWORD2");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine(m.name + "\tKEYWORD2");
            }
            writer.WriteLine();
            writer.WriteLine("#######################################");
            writer.WriteLine("# Constants (LITERAL1)");
            writer.WriteLine("#######################################");
        }

        private void generate_example1(TextWriter writer, string class_name)
        {
            writer.WriteLine("#include <" + class_name + ".h>\n");
            writer.WriteLine(class_name + " robot;\n");
            writer.WriteLine("void setup()\n" +
                             "{\n" +
                             "  robot.begin(true);\n" +
                             "}\n");
            writer.WriteLine("void loop()\n" + 
                             "{\n" +
                             "  robot.update();\n" +
                             "}\n");
        }

        private void generate_example2(TextWriter writer, string class_name)
        {
            writer.WriteLine("#include <" + class_name + ".h>\n");
            writer.WriteLine(class_name + " robot;\n");
            writer.WriteLine("void setup()\n" +
                             "{\n" +
                             "  robot.begin(false);\n" +
                             "}\n");
            writer.WriteLine("void loop()\n{");
            for(int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine("  //robot." + m.name + "();");
            }
            writer.WriteLine("}\n");
        }

        public void generate_AllinOne()
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                List<int> channels = new List<int>();
                List<int> angle = new List<int>();
                List<uint> home = new List<uint>();
                string motion_sketch_name = "\\AllinOne_Motion_Sketch";
                Directory.CreateDirectory(path.SelectedPath + motion_sketch_name);
                nfilename = path.SelectedPath + motion_sketch_name + motion_sketch_name + ".ino";
                TextWriter writer = new StreamWriter(nfilename);

                get_positions(channels, angle, home);
                
                string class_name = "Robot86ME";

                generate_include_headers(writer, false);
                generate_global_var(writer);
                generate_class(writer, class_name, channels);
                
                for (int i = 0; i < ME_Motionlist.Count; i++)
                    generate_namespace((ME_Motion)ME_Motionlist[i], writer, channels);
                generate_constructor(writer, class_name, channels, home);
                generate_functions(writer, class_name, channels);
                generate_setup(writer, channels, home, true, angle, class_name);
                generate_loop(writer, class_name);
                writer.WriteLine(class_name + " robot;\n");
                writer.WriteLine("void setup()\n" +
                                 "{\n" +
                                 "  robot.begin(true);\n" +
                                 "}\n");
                writer.WriteLine("void loop()\n" +
                                 "{\n" +
                                 "  robot.update();\n" +
                                 "}\n");

                MessageBox.Show("The sketch is generated in " + path.SelectedPath + motion_sketch_name + "\\");
                writer.Dispose();
                writer.Close();
            }
        }

        public void generate_Library(string library_dir = "")
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                generate_LibrarayWithPath(path.SelectedPath);
                MessageBox.Show("The library is generated in " + path.SelectedPath + library_dir + "\\Robot86ME\\");
            }
        }

        private void generate_LibrarayWithPath(string path, string library_dir = "")
        {
            List<int> channels = new List<int>();
            List<int> angle = new List<int>();
            List<uint> home = new List<uint>();
            if (string.Compare(library_dir, "") == 0)
                library_dir = "\\Robot86ME";
            Directory.CreateDirectory(path + library_dir);
            nfilename = path + library_dir + library_dir + ".h";
            TextWriter writerh = new StreamWriter(nfilename);
            nfilename = path + library_dir + library_dir + ".cpp";
            TextWriter writer = new StreamWriter(nfilename);
            nfilename = path + library_dir + "\\keywords.txt";
            TextWriter writerk = new StreamWriter(nfilename);
            nfilename = path + library_dir + "\\library.properties";
            TextWriter writerl = new StreamWriter(nfilename);
            Directory.CreateDirectory(path + library_dir + "\\examples");
            Directory.CreateDirectory(path + library_dir + "\\examples\\RobotControlSample");
            nfilename = path + library_dir + "\\examples\\RobotControlSample\\RobotControlSample.ino";
            TextWriter writer_ex1 = new StreamWriter(nfilename);
            Directory.CreateDirectory(path + library_dir + "\\examples\\SingleMotion");
            nfilename = path + library_dir + "\\examples\\SingleMotion\\SingleMotion.ino";
            TextWriter writer_ex2 = new StreamWriter(nfilename);

            get_positions(channels, angle, home);

            string class_name = library_dir.Substring(1);
            writerh.WriteLine("#ifndef __" + class_name.ToUpper() + "_h");
            writerh.WriteLine("#define __" + class_name.ToUpper() + "_h");
            writerh.WriteLine();
            generate_include_headers(writerh, true);
            generate_global_var(writerh);
            generate_class(writerh, class_name, channels);
            writerh.WriteLine("\n#endif");

            writer.WriteLine("#include <" + class_name + ".h>\n");
            for (int i = 0; i < ME_Motionlist.Count; i++)
                generate_namespace((ME_Motion)ME_Motionlist[i], writer, channels);
            writerh.WriteLine();
            generate_constructor(writer, class_name, channels, home);
            generate_functions(writer, class_name, channels);
            generate_setup(writer, channels, home, true, angle, class_name);
            generate_loop(writer, class_name);

            generate_lib_properties(writerl, class_name);
            generate_keywords(writerk, class_name);
            generate_example1(writer_ex1, class_name);
            generate_example2(writer_ex2, class_name);

            writerh.Dispose();
            writerh.Close();
            writer.Dispose();
            writer.Close();
            writerk.Dispose();
            writerk.Close();
            writerl.Dispose();
            writerl.Close();
            writer_ex1.Dispose();
            writer_ex1.Close();
            writer_ex2.Dispose();
            writer_ex2.Close();
        }

        private void generate_blocks(string path)
        {
            TextWriter writer = new StreamWriter(path + "\\s2r_fm.s2e");
            string port = "50209";
            string url = "\"https://github.com/MrYsLab/PyMata\"";
            writer.WriteLine("{");
            writer.WriteLine("\t\"extensionName\": \"s2r_fm - Scratch to Robot\",");
            writer.WriteLine("\t\"extensionPort\": " + port + ",");
            writer.WriteLine("\t\"url\": " + url + ",");
            writer.WriteLine("\t\"blockSpecs\": [");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine("\t\t[\n");
                writer.WriteLine("\t\t\t\"w\",");
                writer.WriteLine("\t\t\t\"Perfrom " + m.name + " %n times\",");
                writer.WriteLine("\t\t\t\"perform_" + m.name + "\",");
                writer.WriteLine("\t\t\t\"1\"");
                writer.Write("\t\t]");
                if (i != ME_Motionlist.Count - 1)
                    writer.WriteLine(",");
                else
                    writer.WriteLine();
            }
            writer.WriteLine("\t]");
            writer.WriteLine("}");
            writer.Dispose();
            writer.Close();
        }

        private void generate_FirmataPlus(string path)
        {
            TextWriter writer = new StreamWriter(path + "\\FirmataPlus.ino");
            string template = Properties.Resources.FirmataPlus;
            string insert = "";
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (i == 0)
                    insert += "      if (motion_id == " + i + ") robot." + m.name + "(motion_times);";
                else
                    insert += "\n      else if (motion_id == " + i + ") robot." + m.name + "(motion_times);";
            }
            writer.Write(template.Replace("      // call motions here", insert));
            writer.Dispose();
            writer.Close();
        }

        private void generate_handler(string path)
        {
            TextWriter writer = new StreamWriter(path + "\\scratch_command_handlers.py");
            string template = Properties.Resources.scratch_command_handlers;
            string insert_function = "";
            string insert_command = "";
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                insert_function += "    def perform_" + m.name + "(self, command):\n" +
                                   "        id = int(command[1])\n" +
                                   "        motion = " + i + "\n" +
                                   "        times = int(command[2])\n" +
                                   "        self.firmata.perform_motion(id, motion, times)\n" +
                                   "        if id not in self.busy_ID:\n" +
                                   "            self.busy_ID.append(id)\n" +
                                   "        return \'okay\'\n\n";
                insert_command += ",\n                    \'perform_" + m.name + "\': perform_" + m.name;
            }
            template = template.Replace("    # Define functions to perform motions here", insert_function);
            writer.Write(template.Replace("# insert motion commands here", insert_command));
            writer.Dispose();
            writer.Close();
        }

        public void generate_ScratchProject()
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                Directory.CreateDirectory(path.SelectedPath + "\\ScratchProject");
                generate_LibrarayWithPath(path.SelectedPath + "\\ScratchProject");
                generate_blocks(path.SelectedPath + "\\ScratchProject");
                generate_FirmataPlus(path.SelectedPath + "\\ScratchProject");
                generate_handler(path.SelectedPath + "\\ScratchProject");
                MessageBox.Show("The project is generated in " + path.SelectedPath + "\\ScratchProject\\");
            }
        }
    }
}
