using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace _86ME_ver1
{
    public class ME_Motion
    {
        public ArrayList Events;
        public List<string> states;
        public List<string> goto_var; 
        public List<int> used_servos;
        public string name;
        public int trigger_method;
        public int auto_method;
        public int trigger_key;
        public int trigger_keyType;
        public string bt_key;
        public string bt_mode;
        public int frames;
        public string ps2_key;
        public int ps2_type;
        public int property;
        public int moton_layer;
        public double[] acc_Settings; //LX, HX, LY, HY, LZ, HZ, D
        public ME_Motion()
        {
            this.name = null;
            this.Events = new ArrayList();
            this.trigger_method = 0;
            this.auto_method = 0;
            this.trigger_key = 0;
            this.trigger_keyType = 1;
            this.bt_key = "";
            this.bt_mode = "OneShot";
            this.frames = 0;
            this.ps2_key = "PSB_SELECT";
            this.ps2_type = 1;
            this.property = 0;
            this.moton_layer = 0;
            this.acc_Settings = new double[7];
            this.goto_var = new List<string>();
            this.states = new List<string>();
            this.used_servos = new List<int>();
        }
    }

    public class ME_Trigger
    {
        public string name;
        public int method;
        public ME_Trigger()
        {
            this.name = "";
            this.method = 1;
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
    }

    public class ME_Delay 
    {
        public int delay;
        public ME_Delay()
        {
            this.delay = 1000;
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
    }

    public class ME_Release
    {
        public ME_Release()
        {
        }
    }

    public class ME_If
    {
        public int method;
        public int left_var;
        public int right_var;
        public string name;
        public ME_If()
        {
            this.name = null;
            this.method = 0;
            this.left_var = 0;
            this.right_var = 0;
        }
    }

    public class ME_Operand
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
        public ME_Operand()
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
    }
}
