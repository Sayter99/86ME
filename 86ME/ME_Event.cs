using System;
using System.Collections.Generic;
using System.Text;

namespace _86ME_ver1
{
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
        public bool parsed;
        public ME_Goto()
        {
            this.name = null;
            this.is_goto = false;
            this.loops = "0";
            this.current_loop = 0;
            this.parsed = false;
        }
    }
    public class ME_Flag
    {
        public string name;
        public ME_Flag()
        {
            this.name = null;
        }
    }
}
