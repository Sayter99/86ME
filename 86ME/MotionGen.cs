using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;

namespace _86ME_ver1
{
    enum mtest_method { always, keyboard };

    public class generate_sketches
    {
        private string nfilename = "";
        private ArrayList ME_Motionlist;
        private NewMotion Motion;
        private bool[] method_flag = new bool[16];
        private int[] offset = new int[45];

        public generate_sketches(NewMotion nMotion, int[] off, ArrayList motionlist)
        {
            this.Motion = nMotion;
            this.offset = off;
            this.ME_Motionlist = motionlist;
            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                method_flag[((ME_Motion)ME_Motionlist[i]).trigger_method] = true;
            }
        }

        private string trigger_condition(ME_Motion m)
        {
            switch(m.trigger_method)
            {
                case (int)mtest_method.always:
                    if (m.trigger_on)
                        return "1";
                    else
                        return "0";
                case (int)mtest_method.keyboard:
                    return "key[" + m.trigger_key + "]";
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

        public void generate_ino(string path, List<int> channels)
        {
            string frm_name = "_frm";
            string current_motion_name = "";
            string motion_sketch_name = "\\_86Duino_Motion_Sketch";
            nfilename = path + motion_sketch_name + motion_sketch_name + ".ino";
            TextWriter writer = new StreamWriter(nfilename);

            // include and declare
            writer.WriteLine("#include <Servo86.h>");
            // *** INCLUDE HEADERS FOR TRIGGER ***
            if (method_flag[1]) // keyboard
            {
                writer.WriteLine("#include <Arduino.h>");
                writer.WriteLine("#include <allegrokb.h>"); //tmp
                writer.WriteLine("#include <allegro.h>");
            }
            // *** INCLUDE HEADERS FOR TRIGGER ***
            writer.WriteLine();
            for (int i = 0; i < channels.Count; i++)
                writer.WriteLine("Servo myservo" + channels[i].ToString() + ";");

            writer.WriteLine();
            writer.WriteLine("ServoOffset myoffs(\"" + "settings\\\\" + "86offset.txt\");");
            writer.WriteLine();

            for (int i = 0; i < ME_Motionlist.Count; i++)
            {
                int frame_count = ((ME_Motion)ME_Motionlist[i]).frames;
                frm_name = ((ME_Motion)ME_Motionlist[i]).name + "_frm";
                current_motion_name = ((ME_Motion)ME_Motionlist[i]).name;
                for (int j = 0; j < frame_count; j++)
                {
                    string fc = j.ToString();
                    writer.WriteLine("ServoFrame " + frm_name + fc + "(\"" + "settings\\\\" +
                                     current_motion_name + "_frm" + fc + ".txt\");");
                }
                writer.WriteLine();
            }

            //void setup {}
            writer.WriteLine("void setup()");
            writer.WriteLine("{");
            // *** SETUP FOR TRIGGER ***
            if (method_flag[1]) // keyboard
            {
                writer.WriteLine("  allegro_init();");
                writer.WriteLine("  install_timer();"); //tmp
                writer.WriteLine("  install_keyboard();\n");
            }
            // *** SETUP FOR TRIGGER ***
            for (int i = 0; i < channels.Count; i++)
                writer.WriteLine("  myservo" + channels[i].ToString() + ".attach(" + channels[i].ToString() + ");");
            writer.WriteLine("  myoffs.setOffsets();");
            writer.WriteLine("}");
            writer.WriteLine();

            //void loop {}
            writer.WriteLine("void loop()");
            writer.WriteLine("{");
            int space_num = 2;
            string space = set_space(space_num);
            for (int j = 0; j < ME_Motionlist.Count; j++)
            {
                ME_Motion m = (ME_Motion)ME_Motionlist[j];
                frm_name = ((ME_Motion)ME_Motionlist[j]).name.ToString() + "_frm";
                // *** TRIGGER ***
                writer.WriteLine(space + "if(" + trigger_condition(m) + ")\n" + space + "{");
                space_num = 4;
                space = set_space(space_num);
                // *** TRIGGER ***
                for (int i = 0, flag_count = 0; i < m.Events.Count; i++)
                {
                    if (m.Events[i] is ME_Frame)
                    {
                        ME_Frame f = (ME_Frame)m.Events[i];
                        writer.WriteLine(space + frm_name + f.num.ToString() + ".playPositions(" + f.delay.ToString() + ");");
                        writer.WriteLine(space + "while(isServoMultiMoving() == true);");
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
                space_num -= 2;
                space = set_space(space_num);
                writer.WriteLine(space + "}");
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
                string motion_settings_path = motion_sketch_name + "\\" + "settings";
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
                                    writer.WriteLine(f.frame[k].ToString());
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
                generate_ino(txtPath, channels);
                MessageBox.Show("The sketch and setting files are generated in " +
                                txtPath + motion_sketch_name + "\\");
            }
        }

        public void generate_AllinOne()
        {
            string frm_name;
            FolderBrowserDialog path = new FolderBrowserDialog();
            var dialogResult = path.ShowDialog();
            string txtPath = path.SelectedPath;
            List<int> channels = new List<int>();
            List<int> angle = new List<int>();
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
                // *** INCLUDE HEADERS FOR TRIGGER ***
                if (method_flag[1]) // keyboard
                {
                    writer.WriteLine("#include <Arduino.h>");
                    writer.WriteLine("#include <allegrokb.h>"); //tmp
                    writer.WriteLine("#include <allegro.h>");
                }
                // *** INCLUDE HEADERS FOR TRIGGER ***
                writer.WriteLine();

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
                                    angle.Add(f.frame[k]);
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

                for (int i = 0; i < channels.Count; i++)
                    writer.WriteLine("Servo myservo" + channels[i].ToString() + ";");

                writer.WriteLine();
                writer.WriteLine("ServoOffset myoffs;");
                writer.WriteLine();

                for (int i = 0; i < ME_Motionlist.Count; i++)
                {
                    frm_name = ((ME_Motion)ME_Motionlist[i]).name.ToString() + "_frm";
                    for (int j = 0; j < ((ME_Motion)ME_Motionlist[i]).frames; j++)
                        writer.WriteLine("ServoFrame " + frm_name + j.ToString() + ";");
                    writer.WriteLine();
                }

                // setup
                writer.WriteLine("void setup()");
                writer.WriteLine("{");
                // *** SETUP FOR TRIGGER ***
                if (method_flag[1])//keyboard
                {
                    writer.WriteLine("  allegro_init();");
                    writer.WriteLine("  install_timer();");
                    writer.WriteLine("  install_keyboard();\n");
                }
                // *** SETUP FOR TRIGGER ***
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
                            writer.WriteLine("  " + frm_name + i.ToString() + ".positions[" + j.ToString() + "] = " +
                                             angle[processed + i * channels.Count + j] + ";");
                        writer.WriteLine();
                    }
                    processed += channels.Count * ((ME_Motion)ME_Motionlist[k]).frames;
                }

                writer.WriteLine("  myoffs.setOffsets();");

                writer.WriteLine("}");
                writer.WriteLine();

                // loop
                writer.WriteLine("void loop()");
                writer.WriteLine("{");
                int space_num = 2;
                string space = set_space(space_num);
                for (int j = 0; j < ME_Motionlist.Count; j++)
                {
                    ME_Motion m = (ME_Motion)ME_Motionlist[j];
                    frm_name = ((ME_Motion)ME_Motionlist[j]).name.ToString() + "_frm";
                    // *** TRIGGER ***
                    writer.WriteLine(space + "if(" + trigger_condition(m) + ")\n" + space + "{");
                    space_num = 4;
                    space = set_space(space_num);
                    // *** TRIGGER ***
                    for (int i = 0, flag_count = 0; i < m.Events.Count; i++)
                    {
                        if (m.Events[i] is ME_Frame)
                        {
                            ME_Frame f = (ME_Frame)m.Events[i];
                            writer.WriteLine(space + frm_name + f.num.ToString() + ".playPositions(" + f.delay.ToString() + ");");
                            writer.WriteLine(space + "while(isServoMultiMoving() == true);");
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
                    space_num -= 2;
                    space = set_space(space_num);
                    writer.WriteLine(space + "}");
                }
                writer.WriteLine("}");

                MessageBox.Show("The sketch is generated in " +
                                path.SelectedPath + motion_sketch_name + "\\");
                writer.Dispose();
                writer.Close();
            }
        }
    }
}
