using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace _86ME_ver1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetForm sform = new SetForm();
            var execute = sform.ShowDialog();
            if (execute == DialogResult.Yes)
            {
                Form1 f1;
                if (args.Length > 0)
                    f1 = new Form1(args[0]);
                else
                    f1 = new Form1();
                f1.com_port = sform.com_port;
                f1.connect_comport();
                Application.Run(f1);
            }
        }
    }
}
