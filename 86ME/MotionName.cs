using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _86ME_ver1
{
    public partial class MotionName : Form
    {
        public string name;
        Dictionary<string, string> MotionName_lang_dic;

        public MotionName(Dictionary<string, string> lang_dic)
        {
            InitializeComponent();
            name = "";
            MotionName_lang_dic = lang_dic;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!(new System.Text.RegularExpressions.Regex("^[a-zA-Z][a-zA-Z0-9_]{0,20}$")).IsMatch(motionNameText.Text))
            {
                if (motionNameText.Text.Length < 20)
                    warningLabel.Text = MotionName_lang_dic["errorMsg12"];
                else
                    warningLabel.Text = MotionName_lang_dic["errorMsg13"];
                motionNameText.Focus();
            }
            else if (motionNameText.Text.IndexOf(" ") == -1) // add new motion successfully
            {
                name = motionNameText.Text;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                warningLabel.Text = MotionName_lang_dic["errorMsg14"];
                motionNameText.Focus();
            }
        }
    }
}
