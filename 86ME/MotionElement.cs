using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace _86ME_ver1
{
    public class ME_Motion
    {
        public ArrayList Events;
        public string name;
        public int trigger_method;
        public int auto_method;
        public int trigger_key;
        public int trigger_keyType;
        public string bt_key;
        public int bt_port;
        public int frames;
        public string ps2_key;
        public int ps2_type;
        public ME_Motion()
        {
            this.name = null;
            this.Events = new ArrayList();
            this.trigger_method = 0;
            this.auto_method = 0;
            this.trigger_key = 0;
            this.trigger_keyType = 1;
            this.bt_key = "";
            this.bt_port = 0;
            this.frames = 0;
            this.ps2_key = "PSB_SELECT";
            this.ps2_type = 1;
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
            this.delay = 0;
        }
    }

    public class ME_Sound
    {
        public string filename;
        public int delay;
        public ME_Sound()
        {
            this.filename = null;
            this.delay = -1;
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
}
