using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;

namespace _86ME_ver1
{
    class FSMGen
    {
        private string nfilename = "";
        private ArrayList ME_Motionlist;
        private NewMotion Motion;
        private bool[] method_flag = new bool[16];
        private int[] offset = new int[45];
        private string[] ps2_pins = new string[4];
        private string bt_baud;
        private string bt_port;

        public FSMGen(NewMotion nMotion, int[] off, ArrayList motionlist, string[] ps2pins, string bt_baud, string bt_port)
        {
            this.Motion = nMotion;
            this.offset = off;
            this.ME_Motionlist = motionlist;
            this.ps2_pins = ps2pins;
            this.bt_baud = bt_baud;
            this.bt_port = bt_port;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                method_flag[((ME_Motion)ME_Motionlist[i]).trigger_method] = true;
            }
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

        private string trigger_condition(ME_Motion m)
        {
            switch (m.trigger_method)
            {
                case (int)mtest_method.always:
                    if (m.auto_method == (int)auto_method.on)
                        return "1";
                    else if (m.auto_method == (int)auto_method.off)
                        return "0";
                    else //title
                        return m.name + "_title == 1";
                case (int)mtest_method.keyboard:
                    if (m.trigger_keyType == (int)keyboard_method.first)
                        return "keys_state[" + convert_keynum(m.trigger_key) + "] == 0";
                    else if (m.trigger_keyType == (int)keyboard_method.pressed)
                        return "keys_state[" + convert_keynum(m.trigger_key) + "] == 1";
                    else // release
                        return "keys_state[" + convert_keynum(m.trigger_key) + "] == 2";
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
                default:
                    return "1";
            }
        }

        private string set_space(int n)
        {
            string ret_str = "";
            for (int i = 0; i < n; i++)
                ret_str += " ";
            return ret_str;
        }

        private void generate_variable(ME_Motion m, TextWriter writer)
        {
            bool have_delay = false;
            int space_num = 2;
            string space = set_space(space_num);
            writer.Write("namespace " + m.name + "\n{\n" + space + "enum {IDLE");
            if (m.Events.Count > 0)
                writer.Write(", ");
            m.goto_var.Clear();
            for (int i = 0; i < m.Events.Count; i++)
            {
                if(m.Events[i] is ME_Frame)
                {
                    writer.Write("FRAME_" + i.ToString() + ", " + "WAIT_FRAME_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Delay)
                {
                    writer.Write("DELAY_" + i.ToString() + ", " + "WAIT_DELAY_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                    have_delay = true;
                }
                else if (m.Events[i] is ME_Flag)
                {
                    writer.Write("FLAG_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
                else if (m.Events[i] is ME_Goto)
                {
                    writer.Write("GOTO_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                    for (int k = 0; k < i; k++)
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
                else if (m.Events[i] is ME_Trigger)
                {
                    writer.Write("MOTION_" + i.ToString() + ", " + "WAIT_MOTION_" + i.ToString());
                    if (i != m.Events.Count - 1)
                        writer.Write(", ");
                }
            }
            writer.WriteLine("};");
            writer.WriteLine(space + "int state = IDLE;");
            if (have_delay)
                writer.WriteLine(space + "unsigned long time;");
            for (int i = 0; i < m.goto_var.Count; i++)
                writer.WriteLine(space + "int " + m.goto_var[i] + " = 0;");
            writer.WriteLine("}");
        }

        private void generate_isBlocked(TextWriter writer)
        {
            int space_num = 2;
            string space = set_space(space_num);
            writer.WriteLine("bool isBlocked()\n{");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.property == (int)motion_property.blocking)
                    writer.WriteLine(space + "if(external_trigger[_" + m.name.ToUpper() + "]) return true;");
            }
            writer.WriteLine(space + "return false;");
            writer.WriteLine("}");
        }

        private void generate_closeTriggers(TextWriter writer)
        {
            int space_num = 2;
            string space = set_space(space_num);
            writer.WriteLine("void closeTriggers()\n{");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine(space + "external_trigger[_" + m.name.ToUpper() + "]= false; " +
                                         "internal_trigger[_" + m.name.ToUpper() + "]= false;");
            }
            writer.WriteLine("}");
        }

        private void generate_updateTrigger(TextWriter writer)
        {
            writer.WriteLine("void updateTrigger()\n" + "{");
            int space_num = 2;
            string space = set_space(space_num);
            writer.WriteLine(space + "if(isBlocked()) return;");
            // get input
            if (method_flag[1])
            {
                writer.WriteLine("  usb.Task();");
                int[] keys_state = new int[128];
                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    if (m.trigger_method == (int)mtest_method.keyboard && keys_state[convert_keynum(m.trigger_key)] == 0)
                    {
                        writer.WriteLine("  keys_state[" + convert_keynum(m.trigger_key) +
                                         "] = key_state(" + convert_keynum(m.trigger_key) + ");");
                        keys_state[convert_keynum(m.trigger_key)] = 1;
                    }
                }
            }
            if (method_flag[2])
            {
                writer.WriteLine("  if(" + bt_port + ".available()){ " + bt_port + "_Command = " + bt_port +
                             ".read(); } else { " + bt_port + "_Command = 0xFFF; }");
            }
            if (method_flag[3])
                writer.WriteLine("  ps2x.read_gamepad();");
            // update tirggers
            bool first = true;
            for (int i = 0; i < ME_Motionlist.Count; i++) //startup
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.trigger_method == (int)mtest_method.always && m.auto_method == (int)auto_method.title)
                {
                    if (first)
                    {
                        writer.WriteLine(space + "if(" + trigger_condition(m) + ") " +
                                         "{_curr_motion = _" + m.name.ToUpper() + "; " +
                                         m.name + "_title--;}");
                        first = false;
                    }
                    else
                        writer.WriteLine(space + "else if(" + trigger_condition(m) + ") " +
                                         "{_curr_motion = _" + m.name.ToUpper() + "; " +
                                         m.name + "_title--;}");
                }
            }
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (!(m.trigger_method == (int)mtest_method.always && m.auto_method == (int)auto_method.on) &&
                    !(m.trigger_method == (int)mtest_method.always && m.auto_method == (int)auto_method.title))
                {
                    if (first)
                    {
                        writer.WriteLine(space + "if(" + trigger_condition(m) + ") " +
                                         "{_curr_motion = _" + m.name.ToUpper() + ";}");
                        first = false;
                    }
                    else
                        writer.WriteLine(space + "else if(" + trigger_condition(m) + ") " +
                                         "{_curr_motion = _" + m.name.ToUpper() + ";}");
                }
            }
            for (int i = 0; i < ME_Motionlist.Count; i++) //always
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                if (m.trigger_method == (int)mtest_method.always && m.auto_method == (int)auto_method.on)
                {
                    if (first)
                    {
                        writer.WriteLine(space + "if(" + trigger_condition(m) + ") " +
                                         "{_curr_motion = _" + m.name.ToUpper() + ";}");
                        first = false;
                    }
                    else
                        writer.WriteLine(space + "else if(" + trigger_condition(m) + ") " +
                                         "{_curr_motion = _" + m.name.ToUpper() + ";}");
                }
            }
            writer.WriteLine(space + "else _curr_motion = _NONE;");
            writer.WriteLine(space + "if(_last_motion != _curr_motion && _curr_motion != _NONE)");
            writer.WriteLine(space + "{\n    closeTriggers();\n    external_trigger[_curr_motion] = true;");
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                writer.WriteLine(space + "  " + m.name + "::state = 0;");
                for (int j = 0; j < m.goto_var.Count; j++ )
                    writer.WriteLine(space + "  " + m.name + "::" + m.goto_var[j] + " = 0;");
            }
            writer.WriteLine(space + "}");
            writer.WriteLine(space + "external_trigger[_curr_motion] = true;");
            writer.WriteLine(space + "_last_motion = _curr_motion;");
            space_num -= 2;
            space = set_space(space_num);
            writer.WriteLine("}");
        }

        private void generate_motion(ME_Motion m, string frm_name, TextWriter writer)
        {
            string space = set_space(2);
            string space4 = set_space(4);
            writer.WriteLine("void " + m.name + "Update()\n{");
            writer.WriteLine(space + "switch(" + m.name + "::state)\n  {");
            writer.WriteLine(space + "case " + m.name + "::IDLE:");
            writer.WriteLine(space4 + "if(external_trigger[_" + m.name.ToUpper() + "] || internal_trigger[_" +
                             m.name.ToUpper() + "]) " + m.name + "::state = 1;");
            writer.WriteLine(space4 + "else break;");
            string next_action = "";
            int state_counter = 1;
            for (int i = 0, flag_count = 0; i < m.Events.Count; i++)
            {
                if (m.Events[i] is ME_Frame)
                {
                    ME_Frame f = (ME_Frame)m.Events[i];
                    writer.WriteLine(space + "case " + m.name + "::FRAME_" + i + ":");
                    writer.WriteLine(space4 + frm_name + "[" + f.num + "].playPositions(" + f.delay + ");");
                    writer.WriteLine(space4 + m.name + "::state = "+ m.name + "::WAIT_FRAME_" + i + ";");
                    writer.WriteLine(space + "case " + m.name + "::WAIT_FRAME_" + i + ":");
                    writer.WriteLine(space4 + "if(!isServoMultiMoving())");
                    if (i != m.Events.Count - 1)
                    {
                        state_counter += 2;
                        next_action = state_counter.ToString();
                        writer.WriteLine(space4 + "  " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space4 + "{\n      " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space4 + "  " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space4 + "  internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "  external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "}");
                    }
                    writer.WriteLine(space4 + "break;");
                }
                else if (m.Events[i] is ME_Delay)
                {
                    ME_Delay d = (ME_Delay)m.Events[i];
                    writer.WriteLine(space + "case " + m.name + "::DELAY_" + i + ":");
                    writer.WriteLine(space4 + m.name + "::time = millis();");
                    writer.WriteLine(space4 + m.name + "::state = " + m.name + "::WAIT_DELAY_" + i + ";");
                    writer.WriteLine(space + "case " + m.name + "::WAIT_DELAY_" + i + ":");
                    writer.WriteLine(space4 + "if(millis() - " + m.name + "::time >= " + d.delay + ")");
                    if (i != m.Events.Count - 1)
                    {
                        state_counter += 2;
                        next_action = state_counter.ToString();
                        writer.WriteLine(space4 + "  " + m.name + "::state = " + next_action + ";");
                    }
                    else
                    {
                        next_action = m.name + "::IDLE";
                        writer.WriteLine(space4 + "{\n      " + m.name + "::state = " + next_action + ";");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space4 + "  " + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space4 + "  internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "  external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "}");
                    }
                    writer.WriteLine(space4 + "break;");
                }
                else if (m.Events[i] is ME_Flag)
                {
                    writer.WriteLine(space + "case " + m.name + "::FLAG_" + i + ":");
                    for (int k = i; k < m.Events.Count; k++)
                    {
                        if (m.Events[k] is ME_Goto)
                        {
                            if (String.Compare(((ME_Flag)m.Events[i]).name, ((ME_Goto)m.Events[k]).name) == 0)
                            {
                                ME_Goto g = (ME_Goto)m.Events[k];
                                if (g.is_goto)
                                {
                                    state_counter++;
                                    string for_var = m.name + "_" + g.name + "_" + flag_count.ToString();
                                    ((ME_Flag)m.Events[i]).var = for_var;
                                    writer.WriteLine(space4 + "flag_" + for_var + ":");
                                    flag_count++;
                                }
                            }
                        }
                    }
                    if (i == m.Events.Count - 1)
                    {
                        writer.WriteLine(space4 + m.name + "::state = " + m.name + "::IDLE;");
                        writer.WriteLine(space4 + "internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "external_trigger[_" + m.name.ToUpper() + "] = false;");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space4 + m.name + "::" + m.goto_var[j] + " = 0;");
                        writer.WriteLine(space4 + "break;");
                    }
                }
                else if (m.Events[i] is ME_Goto)
                {
                    ME_Goto g = (ME_Goto)m.Events[i];
                    writer.WriteLine(space + "case " + m.name + "::GOTO_" + i + ":");
                    if (g.is_goto)
                    {
                        for (int k = 0; k < i; k++)
                        {
                            if (m.Events[k] is ME_Flag)
                            {
                                if (String.Compare(g.name, ((ME_Flag)m.Events[k]).name) == 0)
                                {
                                    string for_var = ((ME_Flag)m.Events[k]).var;
                                    if (((ME_Goto)m.Events[i]).infinite == false)
                                    {
                                        writer.WriteLine(space4 + "if(" + m.name + "::" + g.name + "_" + i +
                                                         "++ < " + g.loops + ") goto flag_" + for_var + ";");
                                    }
                                    else
                                    {
                                        writer.WriteLine(space4 + "if(1) goto flag_" + for_var + ";");
                                    }
                                }
                            }
                        }
                        if (i != m.Events.Count - 1)
                        {
                            state_counter += 1;
                            next_action = state_counter.ToString();
                            writer.WriteLine(space4 + m.name + "::state = " + next_action + ";");
                        }
                        else
                        {
                            writer.WriteLine(space4 + "else\n    {");
                            next_action = m.name + "::IDLE";
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space4 + "  " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space4 + "  internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space4 + "  external_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space4 + "  " + m.name + "::state = " + next_action + ";");
                            writer.WriteLine(space4 + "}");
                        }
                        writer.WriteLine(space4 + "break;");
                    }
                }
                else if (m.Events[i] is ME_Trigger)
                {
                    ME_Trigger t = (ME_Trigger)m.Events[i];
                    writer.WriteLine(space + "case " + m.name + "::MOTION_" + i + ":");
                    for (int j = 0; j < ME_Motionlist.Count; j++) //reset goto_var of the triggered motion
                    {
                        ME_Motion tr_m = (ME_Motion)ME_Motionlist[j];
                        if (tr_m.name == t.name)
                        {
                            for (int k = 0; k < tr_m.goto_var.Count; k++)
                                writer.WriteLine(space4 + tr_m.name + "::" + tr_m.goto_var[k] + " = 0;");
                        }
                    }
                    if (t.method == (int)internal_trigger.call)
                    {
                        writer.WriteLine(space4 + m.name + "::state = " + m.name + "::WAIT_MOTION_" + i + ";");
                        writer.WriteLine(space4 + "internal_trigger[_" + t.name.ToUpper() + "] = true;");
                        writer.WriteLine(space4 + t.name + "::state = " + t.name + "::IDLE;");
                        writer.WriteLine(space + "case " + m.name + "::WAIT_MOTION_" + i + ":");
                        writer.WriteLine(space4 + "if(!internal_trigger[_" + t.name.ToUpper() + "])");
                        if (i != m.Events.Count - 1)
                        {
                            state_counter += 2;
                            next_action = state_counter.ToString();
                            writer.WriteLine(space4 + "  " + m.name + "::state = " + next_action + ";");
                        }
                        else
                        {
                            next_action = m.name + "::IDLE";
                            writer.WriteLine(space4 + "{\n      " + m.name + "::state = " + next_action + ";");
                            for (int j = 0; j < m.goto_var.Count; j++)
                                writer.WriteLine(space4 + "  " + m.name + "::" + m.goto_var[j] + " = 0;");
                            writer.WriteLine(space4 + "  internal_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space4 + "  external_trigger[_" + m.name.ToUpper() + "] = false;");
                            writer.WriteLine(space4 + "}");
                        }
                    }
                    else if (t.method == (int)internal_trigger.jump)
                    {
                        writer.WriteLine(space4 + m.name + "::state = " + m.name + "::IDLE;");
                        writer.WriteLine(space4 + "internal_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "external_trigger[_" + m.name.ToUpper() + "] = false;");
                        writer.WriteLine(space4 + "internal_trigger[_" + t.name.ToUpper() + "] = true;");
                        writer.WriteLine(space4 + t.name + "::state = " + t.name + "::IDLE;");
                        for (int j = 0; j < m.goto_var.Count; j++)
                            writer.WriteLine(space4 + m.name + "::" + m.goto_var[j] + " = 0;");
                    }
                    writer.WriteLine(space4 + "break;");
                }
            }
            writer.WriteLine(space + "default:");
            writer.WriteLine(space4 + "break;");
            writer.WriteLine("  }"); //switch
            writer.WriteLine("}");
        }

        public void generate_AllinOne()
        {
            string frm_name;
            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();
            string txtPath = path.SelectedPath;
            List<int> channels = new List<int>();
            List<int> angle = new List<int>();
            List<uint> home = new List<uint>();
            int count = 0;
            int processed = 0;
            bool add_channel = true;

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                string motion_sketch_name = "\\AllinOne_Motion_Sketch";
                Directory.CreateDirectory(path.SelectedPath + motion_sketch_name);
                nfilename = path.SelectedPath + motion_sketch_name + motion_sketch_name + ".ino";
                TextWriter writer = new StreamWriter(nfilename);
                // include and declare
                writer.WriteLine("#include <Servo86.h>");
                if (method_flag[1]) // keyboard
                    writer.WriteLine("#include <KeyboardController.h>");
                if (method_flag[3]) // ps2
                    writer.WriteLine("#include <PS2X_lib.h>");

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
                writer.WriteLine();

                for (int i = 0; i < channels.Count; i++)
                    writer.WriteLine("Servo myservo" + channels[i].ToString() + ";");

                writer.WriteLine();
                writer.Write("enum {");
                for (int i = 0; i < ME_Motionlist.Count; i++)
                    writer.Write("_" + ((ME_Motion)ME_Motionlist[i]).name.ToUpper() + ", ");
                writer.Write("_NONE");
                writer.WriteLine("};");
                writer.WriteLine("int _last_motion = _NONE;");
                writer.WriteLine("int _curr_motion = _NONE;");
                int triggers_num = ME_Motionlist.Count + 1;
                writer.WriteLine("bool internal_trigger[" + triggers_num + "] = {0};");
                writer.WriteLine("bool external_trigger[" + triggers_num + "] = {0};");
                writer.WriteLine();
                writer.WriteLine("ServoOffset myoffs;");
                writer.WriteLine();
                writer.WriteLine("ServoFrame _86ME_HOME;\n");// automatic homeframe

                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    frm_name = ((ME_Motion)ME_Motionlist[i]).name.ToString() + "_frm";
                    if (((ME_Motion)ME_Motionlist[i]).trigger_method == (int)mtest_method.always &&
                        ((ME_Motion)ME_Motionlist[i]).auto_method == (int)auto_method.title)
                        writer.WriteLine("int " + ((ME_Motion)ME_Motionlist[i]).name + "_title = 1;");
                    writer.WriteLine("ServoFrame " + frm_name + "[" + ((ME_Motion)ME_Motionlist[i]).frames + "];");
                }
                writer.WriteLine();

                if (method_flag[1]) // keyboard
                {
                    writer.WriteLine("USBHost usb;");
                    writer.WriteLine("KeyboardController keyboard(usb);");
                    writer.WriteLine("char current_key = 0;");
                    writer.WriteLine("void keyPressed(){current_key = keyboard.getOemKey();}");
                    writer.WriteLine("void keyReleased(){current_key = 0;}");
                    writer.WriteLine("static int keys_state[128];");
                    writer.WriteLine("static int key_press[128] = {0};\n" + "int key_state(int k)\n" +
                                     "{\n  if(current_key==k && !key_press[k])\n  {\n" + "	key_press[k] = 1;\n" +
                                     "	return 0;\n  }\n" + "  else if(current_key==k && key_press[k])\n  {\n" +
                                     "    key_press[k] = 1;\n" + "    return 1;\n  }\n" +
                                     "  else if(current_key!=k && key_press[k])\n  {\n" + "    key_press[k] = 0;\n" +
                                     "    return 2;\n  }\n" + "  return 3;\n}\n");
                }
                if (method_flag[2]) // bt
                    writer.WriteLine("int " + bt_port + "_Command;");
                if (method_flag[3]) // ps2
                    writer.WriteLine("PS2X ps2x;");
                writer.WriteLine();
                for (int i = 0; i < ME_Motionlist.Count; i++)
                    generate_variable((ME_Motion)ME_Motionlist[i], writer);
                generate_isBlocked(writer);
                generate_closeTriggers(writer);
                generate_updateTrigger(writer);
                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    frm_name = ((ME_Motion)ME_Motionlist[i]).name.ToString() + "_frm";
                    generate_motion(m, frm_name, writer);
                }

                // setup
                writer.WriteLine("void setup()");
                writer.WriteLine("{");
                if (method_flag[2]) // bt
                    writer.WriteLine("  " + bt_port + ".begin(" + bt_baud + ");");
                writer.WriteLine();
                if (method_flag[3]) // ps2
                    writer.WriteLine("  ps2x.config_gamepad(" + ps2_pins[3] + ", " + ps2_pins[1] +
                                     ", " + ps2_pins[2] + ", " + ps2_pins[0] + ", false, false);\n");
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
                for (int j = 0; j < channels.Count; j++)
                    writer.WriteLine("  _86ME_HOME.positions[" + j.ToString() + "] = " + home[j] + ";");
                writer.WriteLine();
                writer.WriteLine("  myoffs.setOffsets();");
                writer.WriteLine();
                writer.WriteLine("  _86ME_HOME.playPositions(0);");

                writer.WriteLine("}");
                writer.WriteLine();

                // loop
                writer.WriteLine("void loop()");
                writer.WriteLine("{");
                writer.WriteLine("  updateTrigger();");
                for (int j = 0; j < ME_Motionlist.Count; j++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[j];
                    writer.WriteLine("  " + m.name + "Update();");
                }
                writer.WriteLine("}");

                MessageBox.Show("The sketch is generated in " +
                                path.SelectedPath + motion_sketch_name + "\\");
                writer.Dispose();
                writer.Close();
            }
        }

        public void generate_ino(string path, List<int> channels, List<uint> home)
        {
            string frm_name = "_frm";
            string current_motion_name = "";
            string motion_sketch_name = "\\_86Duino_Motion_Sketch";
            nfilename = path + motion_sketch_name + motion_sketch_name + ".ino";
            TextWriter writer = new StreamWriter(nfilename);

            // include and declare
            writer.WriteLine("#include <Servo86.h>");
            if (method_flag[1]) // keyboard
                writer.WriteLine("#include <KeyboardController.h>");
            if (method_flag[3]) // ps2
                writer.WriteLine("#include <PS2X_lib.h>");
            writer.WriteLine();

            for (int i = 0; i < channels.Count; i++)
                writer.WriteLine("Servo myservo" + channels[i].ToString() + ";");

            writer.WriteLine();
            writer.Write("enum {");
            for (int i = 0; i < ME_Motionlist.Count; i++)
                writer.Write("_" + ((ME_Motion)ME_Motionlist[i]).name.ToUpper() + ", ");
            writer.Write("_NONE");
            writer.WriteLine("};");
            writer.WriteLine("int _last_motion = _NONE;");
            writer.WriteLine("int _curr_motion = _NONE;");
            int triggers_num = ME_Motionlist.Count + 1;
            writer.WriteLine("bool internal_trigger[" + triggers_num + "] = {0};");
            writer.WriteLine("bool external_trigger[" + triggers_num + "] = {0};");
            writer.WriteLine();
            writer.WriteLine("ServoOffset myoffs(\"" + "_86ME_settings\\\\" + "86offset.txt\");");
            writer.WriteLine();
            writer.WriteLine("ServoFrame _86ME_HOME;\n");// automatic homeframe

            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                int frame_count = ((ME_Motion)ME_Motionlist[i]).frames;
                current_motion_name = ((ME_Motion)ME_Motionlist[i]).name;
                frm_name = ((ME_Motion)ME_Motionlist[i]).name + "_frm";
                if (((ME_Motion)ME_Motionlist[i]).trigger_method == (int)mtest_method.always &&
                        ((ME_Motion)ME_Motionlist[i]).auto_method == (int)auto_method.title)
                    writer.WriteLine("int " + current_motion_name + "_title = 1;");
                writer.WriteLine("ServoFrame " + frm_name + "[" + frame_count + "];");
            }
            writer.WriteLine();

            if (method_flag[1]) // keyboard
            {
                writer.WriteLine("USBHost usb;");
                writer.WriteLine("KeyboardController keyboard(usb);");
                writer.WriteLine("char current_key = 0;");
                writer.WriteLine("void keyPressed(){current_key = keyboard.getOemKey();}");
                writer.WriteLine("void keyReleased(){current_key = 0;}");
                writer.WriteLine("static int keys_state[128];");
                writer.WriteLine("static int key_press[128] = {0};\n" + "int key_state(int k)\n" +
                                 "{\n  if(current_key==k && !key_press[k])\n  {\n" + "	key_press[k] = 1;\n" +
                                 "	return 0;\n  }\n" + "  else if(current_key==k && key_press[k])\n  {\n" +
                                 "    key_press[k] = 1;\n" + "    return 1;\n  }\n" +
                                 "  else if(current_key!=k && key_press[k])\n  {\n" + "    key_press[k] = 0;\n" +
                                 "    return 2;\n  }\n" + "  return 3;\n}\n");
            }
            if (method_flag[2]) // bt
                writer.WriteLine("int " + bt_port + "_Command;");
            if (method_flag[3]) // ps2
                writer.WriteLine("PS2X ps2x;");
            writer.WriteLine();
            for (int i = 0; i < ME_Motionlist.Count; i++)
                generate_variable((ME_Motion)ME_Motionlist[i], writer);
            generate_isBlocked(writer);
            generate_closeTriggers(writer);
            generate_updateTrigger(writer);
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[i];
                frm_name = ((ME_Motion)ME_Motionlist[i]).name.ToString() + "_frm";
                generate_motion(m, frm_name, writer);
            }
            //void setup {}
            writer.WriteLine("void setup()");
            writer.WriteLine("{");
            if (method_flag[2]) // bt
                writer.WriteLine("  " + bt_port + ".begin(" + bt_baud + ");");
            writer.WriteLine();
            if (method_flag[3]) // ps2
                writer.WriteLine("  ps2x.config_gamepad(" + ps2_pins[3] + ", " + ps2_pins[1] +
                                 ", " + ps2_pins[2] + ", " + ps2_pins[0] + ", false, false);\n");
            for (int i = 0; i < channels.Count; i++)
                writer.WriteLine("  myservo" + channels[i].ToString() + ".attach(" + channels[i].ToString() + ");");
            writer.WriteLine();
            for (int j = 0; j < channels.Count; j++)
                writer.WriteLine("  _86ME_HOME.positions[" + j.ToString() + "] = " + home[j] + ";");
            writer.WriteLine();
            writer.WriteLine("  myoffs.setOffsets();");
            writer.WriteLine();
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                int frame_count = ((ME_Motion)ME_Motionlist[i]).frames;
                current_motion_name = ((ME_Motion)ME_Motionlist[i]).name;
                frm_name = ((ME_Motion)ME_Motionlist[i]).name + "_frm";
                for (int j = 0; j < frame_count; j++)
                {
                    writer.WriteLine("  " + frm_name + "[" + j + "].load(\"" + "_86ME_settings\\\\" +
                                     current_motion_name + "_frm" + j + ".txt\");");
                }
            }
            writer.WriteLine();
            writer.WriteLine("  _86ME_HOME.playPositions(0);\n}");

            //void loop {}
            writer.WriteLine("void loop()");
            writer.WriteLine("{");
            writer.WriteLine("  updateTrigger();");
            for (int j = 0; j < ME_Motionlist.Count; j++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[j];
                writer.WriteLine("  " + m.name + "Update();");
            }
            writer.WriteLine("}");

            writer.Dispose();
            writer.Close();
        }

        public void generate_withFiles()
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();
            string txtPath = path.SelectedPath;
            List<int> channels = new List<int>();
            List<uint> home = new List<uint>();
            int count = 0;
            bool add_channel = true;
            TextWriter writer;
            string current_motion_name = "";

            if (dialogResult == DialogResult.OK && path.SelectedPath != null)
            {
                if (!Directory.Exists(path.SelectedPath))
                {
                    MessageBox.Show("The selected directory does not exist, please try again.");
                    return;
                }
                string motion_sketch_name = "\\_86Duino_Motion_Sketch";
                string motion_settings_path = motion_sketch_name + "\\" + "_86ME_settings";
                Directory.CreateDirectory(txtPath + motion_sketch_name);
                Directory.CreateDirectory(txtPath + motion_settings_path);
                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[i];
                    current_motion_name = ((ME_Motion)ME_Motionlist[i]).name;
                    count = 0;
                    for (int j = 0; j < m.Events.Count; j++)
                    {
                        if (m.Events[j] is ME_Frame)
                        {
                            int ch_count = 0;
                            nfilename = txtPath + motion_settings_path + "\\" +
                                        current_motion_name + "_frm" + count.ToString() + ".txt";
                            writer = new StreamWriter(nfilename);
                            ME_Frame f = (ME_Frame)m.Events[j];
                            for (int k = 0; k < 45; k++)
                            {
                                if (String.Compare(Motion.fbox[k].Text, "---noServo---") != 0)
                                {
                                    writer.Write("channel");
                                    writer.Write(ch_count.ToString() + "=");
                                    if (f.type == 1)
                                        writer.WriteLine(f.frame[k].ToString());
                                    else if (f.type == 0)
                                        writer.WriteLine(Motion.ftext2[k].Text.ToString());
                                    home.Add(uint.Parse(Motion.ftext2[k].Text));
                                    if (add_channel)
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
                    m.frames = count;
                }
                nfilename = txtPath + motion_settings_path + "\\86offset" + ".txt";
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
                generate_ino(txtPath, channels, home);
                MessageBox.Show("The sketch and setting files are generated in " +
                                txtPath + motion_sketch_name + "\\");
            }
        }
    }
}
