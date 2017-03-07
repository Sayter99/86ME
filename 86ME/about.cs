using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace _86ME_ver2
{
    public partial class about : Form
    {
        public System.Diagnostics.Process p = new System.Diagnostics.Process();

        public about(Dictionary<string, string> lang_dic)
        {
            InitializeComponent();
            if (lang_dic.Count > 0)
            {
                richTextBox1.LanguageOption = RichTextBoxLanguageOptions.DualFont;
                this.Text = lang_dic["about_title"];
                richTextBox1.Text = lang_dic["content"];
                author_label.Text = lang_dic["author_label_Text"];
                license_label.Text = lang_dic["license_label_Text"];
                version_label.Text = lang_dic["version_label_Text"];
                richTextBox1.SelectionStart = richTextBox1.TextLength;
            }
        }

        private void richTextBox1_LinkClicked(object sender,
        System.Windows.Forms.LinkClickedEventArgs e)
        {
            p = System.Diagnostics.Process.Start(e.LinkText);
        }

        private void fb_button_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/groups/164926427017235/");
        }

        private void web_button_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.86duino.com");
        }

        private void github_button_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Sayter99");
        }
    }
}
