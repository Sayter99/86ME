using System;
using System.Collections.Generic;
using System.Text;

namespace _86ME_ver1._0
{
    public class ME_Frame
    {
        public int[] frame;
        public int delay;
        public ME_Frame()
        {
            this.frame = new int[45];
            this.delay = 0;
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
        public string key;
        public ME_Goto()
        {
            this.name = null;
            this.key = null;
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
    public class ME_COM
    {
        public int port;
        public string com;
        public ME_COM()
        {
            this.port = 0;
            com = null;
        }
    }
}
