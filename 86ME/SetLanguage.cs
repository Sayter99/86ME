using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace _86ME_ver1
{
    class SetLanguage
    {
        public Dictionary<string, string> lang_dic = new Dictionary<string, string>();
        string language;
        public SetLanguage(string lang)
        {
            language = lang;
            read_ini();
        }
        void read_ini()
        {
            lang_dic.Clear();
            char[] delimiterChar = { '=' };
            using (StreamReader reader = new StreamReader(language))
            {
                while (!reader.EndOfStream)
                {
                    string data = reader.ReadLine();
                    if (data.Length < 1)
                        continue;
                    if (data[0] == '#')
                        continue;
                    string[] cmd = data.Split(delimiterChar);
                    string rstring = "";
                    for (int i = 1; i < cmd.Length; i++)
                    {
                        if (i != 1)
                            rstring += '=';
                        rstring += cmd[i];
                    }
                    lang_dic.Add(cmd[0], convertNewLine(rstring));
                }
            }
        }
        private string convertNewLine(string input)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++ )
            {
                if (input[i] == '\\')
                {
                    if (i + 1 < input.Length)
                    {
                        ++i;
                        if (input[i] == 'n')
                            output += '\n';
                        else if (input[i] == 't')
                            output += '\t';
                    }
                }
                else
                    output += input[i];
            }
            return output;
        }
    }
}
