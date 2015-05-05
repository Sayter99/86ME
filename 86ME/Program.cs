using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace _86ME_ver1._0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetForm sform = new SetForm();
            Form1 f1 = new Form1();
            f1.Text = "86Duino Motion Editor";
            sform.ShowDialog();
            f1.com_port = sform.com_port;
            Application.Run(f1);
        }
    }
}
