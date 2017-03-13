using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace _86ME_ver2
{
    public class ME_Motion
    {
        public ArrayList Events;
        public List<string> states;
        public List<string> goto_var;
        public List<int> used_servos;
        public string name;
        public int frames;
        public int trigger_method;
        public int auto_method;
        public int trigger_key;
        public int trigger_keyType;
        public string bt_key;
        public string bt_mode;
        public int wifi602_key;
        public string ps2_key;
        public int ps2_type;
        public int analog_pin;
        public int analog_cond;
        public int analog_value;
        public string esp8266_key;
        public string esp8266_mode;
        public int property;
        public int moton_layer;
        public int comp_range;
        public int control_method;
        public double[] acc_Settings; //LX, HX, LY, HY, LZ, HZ, D
        public ME_Motion()
        {
            this.name = null;
            this.Events = new ArrayList();
            this.frames = 0;
            this.trigger_method = 0;
            this.auto_method = 0;
            this.trigger_key = 0;
            this.trigger_keyType = 1;
            this.bt_key = "";
            this.bt_mode = "OneShot";
            this.wifi602_key = 0;
            this.ps2_key = "PSB_SELECT";
            this.ps2_type = 1;
            this.analog_pin = 0;
            this.analog_cond = 0;
            this.analog_value = 0;
            this.esp8266_key = "";
            this.esp8266_mode = "OneShot";
            this.acc_Settings = new double[7];
            this.property = 0;
            this.moton_layer = 0;
            this.comp_range = 180;
            this.control_method = 0;
            this.goto_var = new List<string>();
            this.states = new List<string>();
            this.used_servos = new List<int>();
        }

        public ME_Motion Copy()
        {
            ME_Motion ret = new ME_Motion();
            ret.name = this.name;
            for (int i = 0; i < this.Events.Count; i++)
            {
                if (this.Events[i] is ME_Frame)
                    ret.Events.Add(((ME_Frame)this.Events[i]).Copy());
                else if (this.Events[i] is ME_Delay)
                    ret.Events.Add(((ME_Delay)this.Events[i]).Copy());
                else if (this.Events[i] is ME_Flag)
                    ret.Events.Add(((ME_Flag)this.Events[i]).Copy());
                else if (this.Events[i] is ME_Goto)
                    ret.Events.Add(((ME_Goto)this.Events[i]).Copy());
                else if (this.Events[i] is ME_GotoMotion)
                    ret.Events.Add(((ME_GotoMotion)this.Events[i]).Copy());
                else if (this.Events[i] is ME_Release)
                    ret.Events.Add(((ME_Release)this.Events[i]).Copy());
                else if (this.Events[i] is ME_If)
                    ret.Events.Add(((ME_If)this.Events[i]).Copy());
                else if (this.Events[i] is ME_Compute)
                    ret.Events.Add(((ME_Compute)this.Events[i]).Copy());
            }
            ret.frames = this.frames;
            ret.trigger_method = this.trigger_method;
            ret.auto_method = this.auto_method;
            ret.trigger_key = this.trigger_key;
            ret.trigger_keyType = this.trigger_keyType;
            ret.bt_key = this.bt_key;
            ret.bt_mode = this.bt_mode;
            ret.wifi602_key = this.wifi602_key;
            ret.ps2_key = this.ps2_key;
            ret.ps2_type = this.ps2_type;
            ret.analog_pin = this.analog_pin;
            ret.analog_cond = this.analog_cond;
            ret.analog_value = this.analog_value;
            ret.esp8266_key = this.esp8266_key;
            ret.esp8266_mode = this.esp8266_mode;
            for (int i = 0; i < this.acc_Settings.Length; i++)
                ret.acc_Settings[i] = this.acc_Settings[i];
            ret.property = this.property;
            ret.moton_layer = this.moton_layer;
            ret.comp_range = this.comp_range;
            ret.control_method = this.control_method;
            ret.goto_var = this.goto_var.ToList();
            ret.states = this.states.ToList();
            ret.used_servos = this.used_servos.ToList();
            return ret;
        }
    }

    public class ME_Trigger
    {
        public ME_Trigger()
        {
        }
    }

    public class ME_GotoMotion
    {
        public string name;
        public int method;
        public ME_GotoMotion()
        {
            this.name = "";
            this.method = 1;
        }

        public ME_GotoMotion Copy()
        {
            ME_GotoMotion ret = new ME_GotoMotion();
            ret.name = this.name;
            ret.method = this.method;
            return ret;
        }
    }

    public class ME_Frame
    {
        public int[] frame;
        public int delay;
        public int num;
        public byte type;
        public ME_Frame()
        {
            this.frame = new int[45];
            this.delay = 1000;
            this.type = 1;
        }

        public ME_Frame Copy()
        {
            ME_Frame ret = new ME_Frame();
            this.frame.CopyTo(ret.frame, 0);
            ret.delay = this.delay;
            ret.type = this.type;
            return ret;
        }
    }

    public class ME_Delay 
    {
        public int delay;
        public ME_Delay()
        {
            this.delay = 1000;
        }

        public ME_Delay Copy()
        {
            ME_Delay ret = new ME_Delay();
            ret.delay = this.delay;
            return ret;
        }
    }

    public class ME_Goto
    {
        public string name;
        public bool is_goto;
        public string loops;
        public int current_loop;
        public bool infinite;
        public ME_Goto()
        {
            this.name = null;
            this.is_goto = false;
            this.loops = "0";
            this.current_loop = 0;
            this.infinite = false;
        }

        public ME_Goto Copy()
        {
            ME_Goto ret = new ME_Goto();
            ret.name = this.name;
            ret.is_goto = this.is_goto;
            ret.loops = this.loops;
            ret.current_loop = this.current_loop;
            ret.infinite = this.infinite;
            return ret;
        }
    }

    public class ME_Flag
    {
        public string name;
        public string var;
        public ME_Flag()
        {
            this.name = null;
            this.var = null;
        }

        public ME_Flag Copy()
        {
            ME_Flag ret = new ME_Flag();
            ret.name = this.name;
            ret.var = this.var;
            return ret;
        }
    }

    public class ME_Release
    {
        public ME_Release()
        {
        }
        public ME_Release Copy()
        {
            ME_Release ret = new ME_Release();
            return ret;
        }
    }

    public class ME_If
    {
        public int form;
        public int method;
        public int left_var;
        public int right_var;
        public double right_const;
        public string name;
        public ME_If()
        {
            this.name = null;
            this.form = 0;
            this.method = 0;
            this.left_var = 0;
            this.right_var = 0;
            this.right_const = 0.0;
        }

        public ME_If Copy()
        {
            ME_If ret = new ME_If();
            ret.name = this.name;
            ret.form = this.form;
            ret.method = this.method;
            ret.left_var = this.left_var;
            ret.right_var = this.right_var;
            ret.right_const = this.right_const;
            return ret;
        }
    }

    public class ME_Compute
    {
        public int left_var;
        public int form;
        public int f1_var1;
        public int f1_op;
        public int f1_var2;
        public int f2_op;
        public int f2_var;
        public int f3_var;
        public double f4_const;
        public ME_Compute()
        {
            this.left_var = 0;
            this.form = 0;
            this.f1_var1 = 0;
            this.f1_op = 0;
            this.f1_var2 = 0;
            this.f2_op = 0;
            this.f2_var = 0;
            this.f3_var = 0;
            this.f4_const = 0.0;
        }

        public ME_Compute Copy()
        {
            ME_Compute ret = new ME_Compute();
            ret.left_var = this.left_var;
            ret.form = this.form;
            ret.f1_var1 = this.f1_var1;
            ret.f1_op = this.f1_op;
            ret.f1_var2 = this.f1_var2;
            ret.f2_op = this.f2_op;
            ret.f2_var = this.f2_var;
            ret.f3_var = this.f3_var;
            ret.f4_const = this.f4_const;
            return ret;
        }
    }

    public class ME_RBM
    {
        public string version;
        public string board;
        public GlobalSettings gs = new GlobalSettings();
        public string[] servos;
        public int[] offsets;
        public uint[] home;
        public uint[] range;
        public string[] picture;
        public string mirror;
        public int sync;
        public string[] imu;
        public string[] pgain;
        public string[] sgain;
        public string[] gainsrc1;
        public string[] gainsrc2;
        public Dictionary<string, double> cmpvar;
        public ArrayList motions;
        public ME_RBM()
        {
            this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.board = "86Duino_One";
            this.servos = new string[45];
            this.offsets = new int[45];
            this.home = new uint[45];
            this.range = new uint[90];
            this.picture = new string[91];
            this.sync = 5;
            this.imu = new string[5];
            this.pgain = new string[45];
            this.sgain = new string[45];
            this.gainsrc1 = new string[45];
            this.gainsrc2 = new string[45];
            this.cmpvar = new Dictionary<string, double>();
        }
    }

    public class ME_MOT
    {
        public string version;
        public ME_Motion motion;
        public ME_MOT()
        {
            this.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
