using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Globalization;

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
            string lang = "en";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string locale_file = Application.StartupPath + "\\locale.ini";
            if (File.Exists(locale_file))
            {
                StreamReader reader = new StreamReader(locale_file);
                char[] delimiterChars = { ' ', '\t', '\r', '\n' };
                string[] datas = reader.ReadToEnd().Split(delimiterChars);
                reader.Dispose();
                reader.Close();
                if (datas.Length > 0)
                    lang = datas[0];
                if (!(String.Compare(lang, "en") == 0 || String.Compare(lang, "zh-TW") == 0 || String.Compare(lang, "zh-Hans")==0))
                    lang = "en";
            }
            lang = Application.StartupPath + "\\locales\\" + lang + ".ini";
            SetLanguage sl = new SetLanguage(lang);
            SetForm sform = new SetForm(sl.lang_dic);
            var execute = sform.ShowDialog();
            if (execute == DialogResult.Yes)
            {
                Main f1;
                if (args.Length > 0)
                    f1 = new Main(args[0], sl.lang_dic);
                else
                    f1 = new Main(sl.lang_dic);
                f1.com_port = sform.com_port;
                f1.connect_comport();
                Application.Run(f1);
            }
        }
    }
}
